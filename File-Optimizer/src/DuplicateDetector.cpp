#include "DuplicateDetector.h"

#include <array>
#include <filesystem>
#include <fstream>
#include <iomanip>
#include <sstream>
#include <unordered_map>

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
	std::vector<FileInfo> files;
	const fs::path root(rootPath);
	std::error_code errorCode;

	if (!fs::exists(root, errorCode) || errorCode || !fs::is_directory(root, errorCode) || errorCode)
	{
		return files;
	}

	fs::recursive_directory_iterator iterator(root, fs::directory_options::skip_permission_denied, errorCode);
	fs::recursive_directory_iterator end;

	if (errorCode)
	{
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
			// Skip unreadable files and continue scanning.
		}

		errorCode.clear();
		iterator.increment(errorCode);
	}

	return files;
}

std::vector<DuplicateGroup> DuplicateDetector::findDuplicateNames(const std::string& rootPath)
{
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

	return results;
}

std::vector<DuplicateGroup> DuplicateDetector::findDuplicateContents(const std::string& rootPath)
{
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
				// Skip files that cannot be hashed.
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

	return results;
}
