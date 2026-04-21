#pragma once

#include <chrono>
#include <iomanip>
#include <mutex>
#include <sstream>
#include <string>
#include <vector>

#ifndef NOMINMAX
#define NOMINMAX
#endif

#include <windows.h>

namespace OperationLogger
{
	enum class Detail
	{
		Normal,
		Detailed
	};

	inline std::wstring utf8ToWide(const std::string& text)
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

	inline std::wstring timestamp()
	{
		const auto now = std::chrono::system_clock::now();
		const auto milliseconds = std::chrono::duration_cast<std::chrono::milliseconds>(now.time_since_epoch()) % 1000;
		const std::time_t timeValue = std::chrono::system_clock::to_time_t(now);

		std::tm localTime{};
		localtime_s(&localTime, &timeValue);

		std::wostringstream stream;
		stream << std::put_time(&localTime, L"%H:%M:%S")
			<< L'.'
			<< std::setw(3)
			<< std::setfill(L'0')
			<< milliseconds.count();
		return stream.str();
	}

	inline bool isDetailedMode()
	{
		static int cachedValue = -1;
		if (cachedValue >= 0)
		{
			return cachedValue == 1;
		}

		constexpr DWORD bufferLength = 32;
		wchar_t buffer[bufferLength]{};
		const DWORD length = GetEnvironmentVariableW(L"FILEOPT_TRACE_MODE", buffer, bufferLength);
		if (length == 0)
		{
			cachedValue = 0;
			return false;
		}

		const std::wstring mode(buffer);
		cachedValue = _wcsicmp(mode.c_str(), L"detailed") == 0 ? 1 : 0;
		return cachedValue == 1;
	}

	inline WORD getColor(const std::string& component)
	{
		if (component == "CoreEngine")
		{
			return FOREGROUND_BLUE | FOREGROUND_GREEN;
		}

		if (component == "SearchIndex")
		{
			return FOREGROUND_GREEN | FOREGROUND_INTENSITY;
		}

		if (component == "Compression")
		{
			return FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_INTENSITY;
		}

		if (component == "DuplicateDetector")
		{
			return FOREGROUND_RED | FOREGROUND_BLUE | FOREGROUND_INTENSITY;
		}

		return FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE;
	}

	inline void writeLine(const std::wstring& line, WORD color)
	{
		static std::mutex mutex;
		std::lock_guard<std::mutex> lock(mutex);

		HANDLE handle = GetStdHandle(STD_OUTPUT_HANDLE);
		if (handle == nullptr || handle == INVALID_HANDLE_VALUE)
		{
			OutputDebugStringW((line + L"\r\n").c_str());
			return;
		}

		DWORD bytesWritten = 0;
		const std::wstring withNewline = line + L"\r\n";
		CONSOLE_SCREEN_BUFFER_INFO consoleInfo{};
		const bool hasConsoleInfo = GetConsoleScreenBufferInfo(handle, &consoleInfo) != 0;
		if (hasConsoleInfo)
		{
			SetConsoleTextAttribute(handle, color);
		}

		if (!WriteConsoleW(handle, withNewline.c_str(), static_cast<DWORD>(withNewline.size()), &bytesWritten, nullptr))
		{
			OutputDebugStringW(withNewline.c_str());
		}

		if (hasConsoleInfo)
		{
			SetConsoleTextAttribute(handle, consoleInfo.wAttributes);
		}
	}

	inline void log(const std::string& component, const std::string& message, Detail detail = Detail::Normal)
	{
		if (detail == Detail::Detailed && !isDetailedMode())
		{
			return;
		}

		std::wstring line = timestamp();
		line += L" | ";
		line += utf8ToWide(component);
		line += L" | ";
		line += utf8ToWide(message);
		writeLine(line, getColor(component));
	}

	inline std::string join(const std::vector<std::string>& values, const std::string& separator = ", ")
	{
		std::ostringstream stream;
		for (size_t i = 0; i < values.size(); ++i)
		{
			if (i > 0)
			{
				stream << separator;
			}

			stream << values[i];
		}

		return stream.str();
	}
}
