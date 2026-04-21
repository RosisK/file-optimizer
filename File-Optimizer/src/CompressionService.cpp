#include "CompressionService.h"

#include <chrono>
#include <filesystem>
#include <string>
#include <vector>

#include "OperationLogger.h"
#include "ZstdCompressor.h"

#ifndef NOMINMAX
#define NOMINMAX
#endif

#include <windows.h>

namespace
{
	namespace fs = std::filesystem;

	std::wstring utf8ToWide(const std::string& text)
	{
		if (text.empty())
		{
			return {};
		}

		const int requiredSize = MultiByteToWideChar(CP_UTF8, 0, text.c_str(), -1, nullptr, 0);
		if (requiredSize <= 0)
		{
			return {};
		}

		std::wstring buffer(requiredSize, L'\0');
		MultiByteToWideChar(CP_UTF8, 0, text.c_str(), -1, buffer.data(), requiredSize);
		buffer.pop_back();
		return buffer;
	}

	std::string wideToUtf8(const std::wstring& text)
	{
		if (text.empty())
		{
			return {};
		}

		const int requiredSize = WideCharToMultiByte(CP_UTF8, 0, text.c_str(), -1, nullptr, 0, nullptr, nullptr);
		if (requiredSize <= 0)
		{
			return {};
		}

		std::string buffer(requiredSize, '\0');
		WideCharToMultiByte(CP_UTF8, 0, text.c_str(), -1, buffer.data(), requiredSize, nullptr, nullptr);
		buffer.pop_back();
		return buffer;
	}

	std::wstring escapePowerShellSingleQuoted(const std::wstring& value)
	{
		std::wstring escaped;
		escaped.reserve(value.size());

		for (const wchar_t ch : value)
		{
			if (ch == L'\'')
			{
				escaped += L"''";
			}
			else
			{
				escaped += ch;
			}
		}

		return escaped;
	}

	bool runPowerShellScript(const std::wstring& script, std::string& errorMessage)
	{
		OperationLogger::log("Compression", "Launching PowerShell archive command.");
		SECURITY_ATTRIBUTES securityAttributes{};
		securityAttributes.nLength = sizeof(securityAttributes);
		securityAttributes.bInheritHandle = TRUE;

		HANDLE readPipe = nullptr;
		HANDLE writePipe = nullptr;
		if (!CreatePipe(&readPipe, &writePipe, &securityAttributes, 0))
		{
			errorMessage = "Failed to create process pipe.";
			return false;
		}

		SetHandleInformation(readPipe, HANDLE_FLAG_INHERIT, 0);

		STARTUPINFOW startupInfo{};
		startupInfo.cb = sizeof(startupInfo);
		startupInfo.dwFlags = STARTF_USESHOWWINDOW | STARTF_USESTDHANDLES;
		startupInfo.wShowWindow = SW_HIDE;
		startupInfo.hStdOutput = writePipe;
		startupInfo.hStdError = writePipe;

		PROCESS_INFORMATION processInfo{};
		std::wstring commandLine =
			L"\"C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe\" "
			L"-NoLogo -NoProfile -NonInteractive -ExecutionPolicy Bypass -Command "
			L"\"& { " + script + L" }\"";

		std::vector<wchar_t> commandBuffer(commandLine.begin(), commandLine.end());
		commandBuffer.push_back(L'\0');

		const BOOL created = CreateProcessW(
			nullptr,
			commandBuffer.data(),
			nullptr,
			nullptr,
			TRUE,
			CREATE_NO_WINDOW,
			nullptr,
			nullptr,
			&startupInfo,
			&processInfo);

		CloseHandle(writePipe);

		if (!created)
		{
			CloseHandle(readPipe);
			errorMessage = "Failed to start PowerShell process.";
			return false;
		}

		std::string capturedOutput;
		char buffer[512];
		DWORD bytesRead = 0;

		while (ReadFile(readPipe, buffer, sizeof(buffer), &bytesRead, nullptr) && bytesRead > 0)
		{
			capturedOutput.append(buffer, bytesRead);
		}

		WaitForSingleObject(processInfo.hProcess, INFINITE);

		DWORD exitCode = 1;
		GetExitCodeProcess(processInfo.hProcess, &exitCode);

		CloseHandle(readPipe);
		CloseHandle(processInfo.hThread);
		CloseHandle(processInfo.hProcess);

		if (exitCode != 0)
		{
			errorMessage = capturedOutput.empty() ? "PowerShell archive command failed." : capturedOutput;
			OperationLogger::log("Compression", "PowerShell archive command failed: " + errorMessage);
			return false;
		}

		OperationLogger::log("Compression", "PowerShell archive command completed successfully.");
		return true;
	}

	bool compressZipPath(const std::string& inputPath, const std::string& outputPath, std::string& errorMessage)
	{
		OperationLogger::log("Compression", "ZIP compression requested from \"" + inputPath + "\" to \"" + outputPath + "\".");
		const fs::path source(utf8ToWide(inputPath));
		const fs::path destination(utf8ToWide(outputPath));

		if (!fs::exists(source))
		{
			errorMessage = "Source path does not exist.";
			return false;
		}

		if (destination.extension() != ".zip")
		{
			errorMessage = "ZIP output must use the .zip extension.";
			return false;
		}

		const std::wstring escapedSource = escapePowerShellSingleQuoted(source.wstring());
		const std::wstring escapedDestination = escapePowerShellSingleQuoted(destination.wstring());

		return runPowerShellScript(
			L"Compress-Archive -LiteralPath '" + escapedSource + L"' -DestinationPath '" + escapedDestination + L"' -Force",
			errorMessage);
	}

	bool decompressZipPath(const std::string& inputPath, const std::string& outputPath, std::string& errorMessage)
	{
		OperationLogger::log("Compression", "ZIP extraction requested from \"" + inputPath + "\" to \"" + outputPath + "\".");
		const fs::path source(utf8ToWide(inputPath));
		const fs::path destination(utf8ToWide(outputPath));

		if (!fs::exists(source))
		{
			errorMessage = "Archive path does not exist.";
			return false;
		}

		if (!fs::is_regular_file(source))
		{
			errorMessage = "ZIP extraction expects a file archive.";
			return false;
		}

		if (!fs::exists(destination))
		{
			fs::create_directories(destination);
		}

		const std::wstring escapedSource = escapePowerShellSingleQuoted(source.wstring());
		const std::wstring escapedDestination = escapePowerShellSingleQuoted(destination.wstring());

		return runPowerShellScript(
			L"Expand-Archive -LiteralPath '" + escapedSource + L"' -DestinationPath '" + escapedDestination + L"' -Force",
			errorMessage);
	}
}

bool compressPath(
	const std::string& inputPath,
	const std::string& outputPath,
	CompressionFormat format,
	std::string& errorMessage)
{
	const auto startedAt = std::chrono::steady_clock::now();
	errorMessage.clear();
	OperationLogger::log("Compression", "compressPath started. Input=\"" + inputPath + "\", Output=\"" + outputPath + "\".");

	bool success = false;
	switch (format)
	{
	case CompressionFormat::Zstd:
	{
		const fs::path source(utf8ToWide(inputPath));
		if (!fs::exists(source))
		{
			errorMessage = "Source path does not exist.";
			return false;
		}

		if (!fs::is_regular_file(source))
		{
			errorMessage = "Zstandard compression currently supports files only.";
			return false;
		}

		if (!compressFile(inputPath, outputPath))
		{
			errorMessage = "Zstandard compression failed.";
			break;
		}

		success = true;
		break;
	}

	case CompressionFormat::Zip:
		success = compressZipPath(inputPath, outputPath, errorMessage);
		break;

	default:
		errorMessage = "Unsupported compression format.";
		break;
	}

	const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
		std::chrono::steady_clock::now() - startedAt);
	OperationLogger::log(
		"Compression",
		std::string("compressPath ") + (success ? "succeeded" : "failed") +
		" using format " + (format == CompressionFormat::Zip ? "ZIP" : "Zstd") +
		" in " + std::to_string(elapsed.count()) + " ms." +
		(errorMessage.empty() ? std::string() : " Details: " + errorMessage));
	return success;
}

bool decompressPath(
	const std::string& inputPath,
	const std::string& outputPath,
	CompressionFormat format,
	std::string& errorMessage)
{
	const auto startedAt = std::chrono::steady_clock::now();
	errorMessage.clear();
	OperationLogger::log("Compression", "decompressPath started. Input=\"" + inputPath + "\", Output=\"" + outputPath + "\".");

	bool success = false;
	switch (format)
	{
	case CompressionFormat::Zstd:
		if (!decompressFile(inputPath, outputPath))
		{
			errorMessage = "Zstandard decompression failed.";
			break;
		}

		success = true;
		break;

	case CompressionFormat::Zip:
		success = decompressZipPath(inputPath, outputPath, errorMessage);
		break;

	default:
		errorMessage = "Unsupported compression format.";
		break;
	}

	const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
		std::chrono::steady_clock::now() - startedAt);
	OperationLogger::log(
		"Compression",
		std::string("decompressPath ") + (success ? "succeeded" : "failed") +
		" using format " + (format == CompressionFormat::Zip ? "ZIP" : "Zstd") +
		" in " + std::to_string(elapsed.count()) + " ms." +
		(errorMessage.empty() ? std::string() : " Details: " + errorMessage));
	return success;
}
