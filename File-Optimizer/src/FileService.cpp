#include <filesystem>
#include <fstream>
#include <iostream>
#include <chrono>

#include "OperationLogger.h"
#include "FileService.h"

namespace fs = std::filesystem;

std::vector<FileInfo> FileService::getDirectoryContent(const std::string& path)
{
	const auto startedAt = std::chrono::steady_clock::now();
	std::vector<FileInfo> items;

	try
	{
		for (const auto& entry : fs::directory_iterator(path))
		{
			FileInfo info;

			info.path = entry.path().string();
			info.name = entry.path().filename().string();
			info.isDirectory = entry.is_directory();

			if (!info.isDirectory)
				info.size = entry.file_size();
			else
				info.size = 0;

			// Get the file's last write time (uses file_clock internally)
			auto ftime= fs::last_write_time(entry);

			// Convert from filesystem clock (file_clock) to system_clock
			auto sctp = std::chrono::clock_cast<std::chrono::system_clock>(ftime);

			//Convert system_clock::time_point -> time_t (calender time)
			std::time_t cftime = std::chrono::system_clock::to_time_t(sctp);
			info.modifiedTime = cftime;

			items.push_back(info);

		}
	}
	catch (const std::exception& ex)
	{
		OperationLogger::log("FileService", "List failed for \"" + path + "\": " + std::string(ex.what()));
	}

	const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
		std::chrono::steady_clock::now() - startedAt);
	OperationLogger::log(
		"FileService",
		"List \"" + path + "\" -> " + std::to_string(items.size()) + " item(s), " + std::to_string(elapsed.count()) + " ms.",
		OperationLogger::Detail::Detailed);

	return items;
}

bool FileService::copyFile(const std::string& src, const std::string& dest)
{
	try
	{
		return copyPath(src, dest);
	}
	catch (const std::exception& ex)
	{
		OperationLogger::log("FileService", "Copy failed: " + std::string(ex.what()));
		return false;
	}
}

bool FileService::deleteFile(const std::string& path)
{
	try
	{
		return deletePath(path);
	}
	catch(const std::exception& ex) 
	{
		OperationLogger::log("FileService", "Delete failed: " + std::string(ex.what()));
		return false;
	}
}

bool FileService::copyPath(const std::string& src, const std::string& dest)
{
	const auto startedAt = std::chrono::steady_clock::now();
	try
	{
		const fs::path source(src);
		const fs::path destination(dest);

		if (!fs::exists(source) || fs::exists(destination))
		{
			OperationLogger::log("FileService", "Copy rejected: source missing or destination already exists.");
			return false;
		}

		const bool isDirectory = fs::is_directory(source);
		if (isDirectory)
		{
			fs::copy(source, destination, fs::copy_options::recursive);
		}
		else
		{
			fs::copy_file(source, destination, fs::copy_options::overwrite_existing);
		}

		const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
			std::chrono::steady_clock::now() - startedAt);
		OperationLogger::log(
			"FileService",
			std::string(isDirectory ? "Copy folder" : "Copy file") + " \"" + src + "\" -> \"" + dest +
			"\" | " + std::to_string(elapsed.count()) + " ms.",
			OperationLogger::Detail::Detailed);
		return true;
	}
	catch (const std::exception& ex)
	{
		OperationLogger::log("FileService", "Copy failed: " + std::string(ex.what()));
		return false;
	}
}

bool FileService::deletePath(const std::string& path)
{
	const auto startedAt = std::chrono::steady_clock::now();
	try
	{
		const fs::path target(path);

		if (!fs::exists(target))
		{
			OperationLogger::log("FileService", "Delete rejected: target does not exist.");
			return false;
		}

		const bool isDirectory = fs::is_directory(target);
		const bool removed = fs::is_directory(target)
			? fs::remove_all(target) > 0
			: fs::remove(target);

		const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
			std::chrono::steady_clock::now() - startedAt);
		OperationLogger::log(
			"FileService",
			std::string(isDirectory ? "Delete folder" : "Delete file") + " \"" + path + "\" -> " +
			(removed ? "ok" : "no change") + ", " + std::to_string(elapsed.count()) + " ms.",
			OperationLogger::Detail::Detailed);
		return removed;
	}
	catch (const std::exception& ex)
	{
		OperationLogger::log("FileService", "Delete failed: " + std::string(ex.what()));
		return false;
	}
}

bool FileService::renamePath(const std::string& sourcePath, const std::string& destinationPath)
{
	const auto startedAt = std::chrono::steady_clock::now();
	try
	{
		fs::rename(sourcePath, destinationPath);
		const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
			std::chrono::steady_clock::now() - startedAt);
		OperationLogger::log("FileService", "Rename \"" + sourcePath + "\" -> \"" + destinationPath + "\" | " + std::to_string(elapsed.count()) + " ms.", OperationLogger::Detail::Detailed);
		return true;
	}
	catch (const std::exception& ex)
	{
		OperationLogger::log("FileService", "Rename failed: " + std::string(ex.what()));
		return false;
	}
}

bool FileService::movePath(const std::string& sourcePath, const std::string& destinationPath)
{
	const auto startedAt = std::chrono::steady_clock::now();
	try
	{
		const fs::path source(sourcePath);
		const fs::path destination(destinationPath);

		if (!fs::exists(source) || fs::exists(destination))
		{
			OperationLogger::log("FileService", "Move rejected: source missing or destination already exists.");
			return false;
		}

		std::error_code errorCode;
		fs::rename(source, destination, errorCode);
		if (!errorCode)
		{
			const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
				std::chrono::steady_clock::now() - startedAt);
			OperationLogger::log("FileService", "Move \"" + sourcePath + "\" -> \"" + destinationPath + "\" | " + std::to_string(elapsed.count()) + " ms.", OperationLogger::Detail::Detailed);
			return true;
		}

		if (!copyPath(sourcePath, destinationPath))
			return false;

		const bool deleted = deletePath(sourcePath);
		const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
			std::chrono::steady_clock::now() - startedAt);
		OperationLogger::log(
			"FileService",
			std::string("Move fallback ") + (deleted ? "ok" : "failed") +
			" for \"" + sourcePath + "\" -> \"" + destinationPath + "\" | " + std::to_string(elapsed.count()) + " ms.",
			OperationLogger::Detail::Detailed);
		return deleted;
	}
	catch (const std::exception& ex)
	{
		OperationLogger::log("FileService", "Move failed: " + std::string(ex.what()));
		return false;
	}
}

bool FileService::createEmptyFile(const std::string& path)
{
	const auto startedAt = std::chrono::steady_clock::now();
	try
	{
		std::ofstream output(path, std::ios::binary);
		const bool created = output.good();
		const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
			std::chrono::steady_clock::now() - startedAt);
		OperationLogger::log(
			"FileService",
			std::string("Create file \"") + path + "\" -> " + (created ? "ok" : "failed") +
			", " + std::to_string(elapsed.count()) + " ms.",
			OperationLogger::Detail::Detailed);
		return created;
	}
	catch (const std::exception& ex)
	{
		OperationLogger::log("FileService", "Create file failed: " + std::string(ex.what()));
		return false;
	}
}

bool FileService::createDirectory(const std::string& path)
{
	const auto startedAt = std::chrono::steady_clock::now();
	try
	{
		const bool created = fs::create_directory(path);
		const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
			std::chrono::steady_clock::now() - startedAt);
		OperationLogger::log(
			"FileService",
			std::string("Create folder \"") + path + "\" -> " + (created ? "ok" : "no change") +
			", " + std::to_string(elapsed.count()) + " ms.",
			OperationLogger::Detail::Detailed);
		return created;
	}
	catch (const std::exception& ex)
	{
		OperationLogger::log("FileService", "Create folder failed: " + std::string(ex.what()));
		return false;
	}
}
