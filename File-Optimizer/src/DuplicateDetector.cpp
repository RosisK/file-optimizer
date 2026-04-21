#include "DuplicateDetector.h"

#include <array>
#include <filesystem>
#include <fstream>
#include <iomanip>
#include <sstream>
#include <unordered_map>

#include "OperationLogger.h"

#ifndef NOMINMAX
#define NOMINMAX
#endif

#include <windows.h>
#include <bcrypt.h>

#ifndef BCRYPT_SUCCESS
#define BCRYPT_SUCCESS(Status) (((NTSTATUS)(Status)) >= 0)
#endif

namespace
{
	namespace fs = std::filesystem;

	std::string computeFileSha256(const fs::path& path)
	{
		BCRYPT_ALG_HANDLE algorithmHandle = nullptr;
		BCRYPT_HASH_HANDLE hashHandle = nullptr;

		DWORD objectLength = 0;
		DWORD dataLength = 0;
		DWORD hashLength = 0;

		if (!BCRYPT_SUCCESS(BCryptOpenAlgorithmProvider(&algorithmHandle, BCRYPT_SHA256_ALGORITHM, nullptr, 0)))
		{
			throw std::runtime_error("Failed to open SHA-256 algorithm provider.");
		}

		if (!BCRYPT_SUCCESS(BCryptGetProperty(
			algorithmHandle,
			BCRYPT_OBJECT_LENGTH,
			reinterpret_cast<PUCHAR>(&objectLength),
			sizeof(objectLength),
			&dataLength,
			0)))
		{
			BCryptCloseAlgorithmProvider(algorithmHandle, 0);
			throw std::runtime_error("Failed to query hash object length.");
		}

		if (!BCRYPT_SUCCESS(BCryptGetProperty(
			algorithmHandle,
			BCRYPT_HASH_LENGTH,
			reinterpret_cast<PUCHAR>(&hashLength),
			sizeof(hashLength),
			&dataLength,
			0)))
		{
			BCryptCloseAlgorithmProvider(algorithmHandle, 0);
			throw std::runtime_error("Failed to query hash length.");
		}

		std::vector<UCHAR> hashObject(objectLength);
		std::vector<UCHAR> hash(hashLength);

		if (!BCRYPT_SUCCESS(BCryptCreateHash(
			algorithmHandle,
			&hashHandle,
			hashObject.data(),
			static_cast<ULONG>(hashObject.size()),
			nullptr,
			0,
			0)))
		{
			BCryptCloseAlgorithmProvider(algorithmHandle, 0);
			throw std::runtime_error("Failed to create hash object.");
		}

		std::ifstream input(path, std::ios::binary);
		if (!input)
		{
			BCryptDestroyHash(hashHandle);
			BCryptCloseAlgorithmProvider(algorithmHandle, 0);
			throw std::runtime_error("Failed to open file for hashing.");
		}

		std::array<char, 64 * 1024> buffer{};
		while (input)
		{
			input.read(buffer.data(), static_cast<std::streamsize>(buffer.size()));
			const std::streamsize bytesRead = input.gcount();
			if (bytesRead <= 0)
			{
				continue;
			}

			if (!BCRYPT_SUCCESS(BCryptHashData(
				hashHandle,
				reinterpret_cast<PUCHAR>(buffer.data()),
				static_cast<ULONG>(bytesRead),
				0)))
			{
				BCryptDestroyHash(hashHandle);
				BCryptCloseAlgorithmProvider(algorithmHandle, 0);
				throw std::runtime_error("Failed while hashing file contents.");
			}
		}

		if (!BCRYPT_SUCCESS(BCryptFinishHash(hashHandle, hash.data(), static_cast<ULONG>(hash.size()), 0)))
		{
			BCryptDestroyHash(hashHandle);
			BCryptCloseAlgorithmProvider(algorithmHandle, 0);
			throw std::runtime_error("Failed to finalize file hash.");
		}

		BCryptDestroyHash(hashHandle);
		BCryptCloseAlgorithmProvider(algorithmHandle, 0);

		std::ostringstream stream;
		stream << std::hex << std::setfill('0');
		for (const auto byte : hash)
		{
			stream << std::setw(2) << static_cast<int>(byte);
		}

		return stream.str();
	}
}

std::vector<FileInfo> DuplicateDetector::collectFiles(const std::string& rootPath)
{
	const auto startedAt = std::chrono::steady_clock::now();
	std::vector<FileInfo> files;
	const fs::path root(rootPath);
	std::error_code errorCode;
	OperationLogger::log("DuplicateDetector", "Scanning recursively for files under \"" + rootPath + "\".");

	if (!fs::exists(root, errorCode) || errorCode || !fs::is_directory(root, errorCode) || errorCode)
	{
		OperationLogger::log("DuplicateDetector", "Root path is not an accessible directory. Scan aborted.");
		return files;
	}

	fs::recursive_directory_iterator iterator(root, fs::directory_options::skip_permission_denied, errorCode);
	fs::recursive_directory_iterator end;

	if (errorCode)
	{
		OperationLogger::log("DuplicateDetector", "Could not start recursive scan: " + errorCode.message());
		return files;
	}

	while (iterator != end)
	{
		try
		{
			const auto& entry = *iterator;

			if (entry.is_symlink())
			{
				iterator.disable_recursion_pending();
			}

			if (!entry.is_regular_file())
			{
				errorCode.clear();
				iterator.increment(errorCode);
				continue;
			}

			FileInfo info;
			info.path = entry.path().string();
			info.name = entry.path().filename().string();
			info.isDirectory = false;
			info.size = entry.file_size();

			auto ftime = fs::last_write_time(entry);
			auto sctp = std::chrono::clock_cast<std::chrono::system_clock>(ftime);
			info.modifiedTime = std::chrono::system_clock::to_time_t(sctp);

			files.push_back(info);
		}
		catch (...)
		{
			OperationLogger::log("DuplicateDetector", "Skipped an unreadable path while scanning.");
		}

		errorCode.clear();
		iterator.increment(errorCode);
	}

	const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
		std::chrono::steady_clock::now() - startedAt);
	OperationLogger::log(
		"DuplicateDetector",
		"File collection completed with " + std::to_string(files.size()) + " file(s) in " + std::to_string(elapsed.count()) + " ms.");

	return files;
}

std::vector<DuplicateGroup> DuplicateDetector::findDuplicateNames(const std::string& rootPath)
{
	const auto startedAt = std::chrono::steady_clock::now();
	std::unordered_map<std::string, std::vector<FileInfo>> groupsByName;
	for (const auto& file : collectFiles(rootPath))
	{
		groupsByName[file.name].push_back(file);
	}

	std::vector<DuplicateGroup> results;
	for (auto& [name, items] : groupsByName)
	{
		if (items.size() < 2)
		{
			continue;
		}

		results.push_back({ name, items });
	}

	const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
		std::chrono::steady_clock::now() - startedAt);
	OperationLogger::log(
		"DuplicateDetector",
		"Name duplicate scan produced " + std::to_string(results.size()) + " group(s) from " +
		std::to_string(groupsByName.size()) + " distinct file name(s) in " + std::to_string(elapsed.count()) + " ms.");

	return results;
}

std::vector<DuplicateGroup> DuplicateDetector::findDuplicateContents(const std::string& rootPath)
{
	const auto startedAt = std::chrono::steady_clock::now();
	std::unordered_map<uintmax_t, std::vector<FileInfo>> groupsBySize;
	for (const auto& file : collectFiles(rootPath))
	{
		groupsBySize[file.size].push_back(file);
	}

	std::vector<DuplicateGroup> results;

	for (const auto& [size, sameSizeFiles] : groupsBySize)
	{
		if (sameSizeFiles.size() < 2)
		{
			continue;
		}

		OperationLogger::log(
			"DuplicateDetector",
			"Hashing " + std::to_string(sameSizeFiles.size()) + " file(s) in same-size bucket " + std::to_string(size) + " byte(s).");

		std::unordered_map<std::string, std::vector<FileInfo>> groupsByHash;
		for (const auto& file : sameSizeFiles)
		{
			try
			{
				const std::string hash = computeFileSha256(fs::path(file.path));
				groupsByHash[hash].push_back(file);
			}
			catch (...)
			{
				OperationLogger::log("DuplicateDetector", "Skipped a file that could not be hashed: \"" + file.path + "\".");
			}
		}

		for (auto& [hash, items] : groupsByHash)
		{
			if (items.size() < 2)
			{
				continue;
			}

			results.push_back({ hash, items });
		}
	}

	const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
		std::chrono::steady_clock::now() - startedAt);
	OperationLogger::log(
		"DuplicateDetector",
		"Content duplicate scan produced " + std::to_string(results.size()) + " group(s) across " +
		std::to_string(groupsBySize.size()) + " size bucket(s) in " + std::to_string(elapsed.count()) + " ms.");

	return results;
}
