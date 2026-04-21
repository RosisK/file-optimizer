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
	OperationLogger::log("FileService", "Listing directory contents for \"" + path + "\".");

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
		OperationLogger::log("FileService", "Directory listing failed: " + std::string(ex.what()));
	}

	const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
		std::chrono::steady_clock::now() - startedAt);
	OperationLogger::log(
		"FileService",
		"Directory listing returned " + std::to_string(items.size()) + " item(s) in " + std::to_string(elapsed.count()) + " ms.");

	return items;
}

bool FileService::copyFile(const std::string& src, const std::string& dest)
{
	OperationLogger::log("FileService", "copyFile requested from \"" + src + "\" to \"" + dest + "\".");
	try
	{
		return copyPath(src, dest);
	}
	catch (const std::exception& ex)
	{
		OperationLogger::log("FileService", "copyFile failed with exception: " + std::string(ex.what()));
		return false;
	}
}

bool FileService::deleteFile(const std::string& path)
{
	OperationLogger::log("FileService", "deleteFile requested for \"" + path + "\".");
	try
	{
		return deletePath(path);
	}
	catch(const std::exception& ex) 
	{
		OperationLogger::log("FileService", "deleteFile failed with exception: " + std::string(ex.what()));
		return false;
	}
}

bool FileService::copyPath(const std::string& src, const std::string& dest)
{
	const auto startedAt = std::chrono::steady_clock::now();
	OperationLogger::log("FileService", "Copy requested from \"" + src + "\" to \"" + dest + "\".");
	try
	{
		const fs::path source(src);
		const fs::path destination(dest);

		if (!fs::exists(source) || fs::exists(destination))
		{
			OperationLogger::log("FileService", "Copy rejected because the source is missing or the destination already exists.");
			return false;
		}

		if (fs::is_directory(source))
		{
			OperationLogger::log("FileService", "Source is a directory. Performing recursive copy.");
			fs::copy(source, destination, fs::copy_options::recursive);
		}
		else
		{
			OperationLogger::log("FileService", "Source is a file. Performing file copy.");
			fs::copy_file(source, destination, fs::copy_options::overwrite_existing);
		}

		const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
			std::chrono::steady_clock::now() - startedAt);
		OperationLogger::log("FileService", "Copy succeeded in " + std::to_string(elapsed.count()) + " ms.");
		return true;
	}
	catch (const std::exception& ex)
	{
		OperationLogger::log("FileService", "Copy failed with exception: " + std::string(ex.what()));
		return false;
	}
}

bool FileService::deletePath(const std::string& path)
{
	const auto startedAt = std::chrono::steady_clock::now();
	OperationLogger::log("FileService", "Delete requested for \"" + path + "\".");
	try
	{
		const fs::path target(path);

		if (!fs::exists(target))
		{
			OperationLogger::log("FileService", "Delete rejected because the target does not exist.");
			return false;
		}

		const bool removed = fs::is_directory(target)
			? fs::remove_all(target) > 0
			: fs::remove(target);

		const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
			std::chrono::steady_clock::now() - startedAt);
		OperationLogger::log(
			"FileService",
			std::string("Delete ") + (removed ? "succeeded" : "did not remove anything") +
			" in " + std::to_string(elapsed.count()) + " ms.");
		return removed;
	}
	catch (const std::exception& ex)
	{
		OperationLogger::log("FileService", "Delete failed with exception: " + std::string(ex.what()));
		return false;
	}
}

bool FileService::renamePath(const std::string& sourcePath, const std::string& destinationPath)
{
	const auto startedAt = std::chrono::steady_clock::now();
	OperationLogger::log("FileService", "Rename requested from \"" + sourcePath + "\" to \"" + destinationPath + "\".");
	try
	{
		fs::rename(sourcePath, destinationPath);
		const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
			std::chrono::steady_clock::now() - startedAt);
		OperationLogger::log("FileService", "Rename succeeded in " + std::to_string(elapsed.count()) + " ms.");
		return true;
	}
	catch (const std::exception& ex)
	{
		OperationLogger::log("FileService", "Rename failed with exception: " + std::string(ex.what()));
		return false;
	}
}

bool FileService::movePath(const std::string& sourcePath, const std::string& destinationPath)
{
	const auto startedAt = std::chrono::steady_clock::now();
	OperationLogger::log("FileService", "Move requested from \"" + sourcePath + "\" to \"" + destinationPath + "\".");
	try
	{
		const fs::path source(sourcePath);
		const fs::path destination(destinationPath);

		if (!fs::exists(source) || fs::exists(destination))
		{
			OperationLogger::log("FileService", "Move rejected because the source is missing or the destination already exists.");
			return false;
		}

		std::error_code errorCode;
		fs::rename(source, destination, errorCode);
		if (!errorCode)
		{
			const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
				std::chrono::steady_clock::now() - startedAt);
			OperationLogger::log("FileService", "Move completed via filesystem rename in " + std::to_string(elapsed.count()) + " ms.");
			return true;
		}

		OperationLogger::log("FileService", "Direct rename failed, falling back to copy + delete. Reason: " + errorCode.message());

		if (!copyPath(sourcePath, destinationPath))
			return false;

		const bool deleted = deletePath(sourcePath);
		const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
			std::chrono::steady_clock::now() - startedAt);
		OperationLogger::log(
			"FileService",
			std::string("Move fallback ") + (deleted ? "succeeded" : "failed during delete phase") +
			" in " + std::to_string(elapsed.count()) + " ms.");
		return deleted;
	}
	catch (const std::exception& ex)
	{
		OperationLogger::log("FileService", "Move failed with exception: " + std::string(ex.what()));
		return false;
	}
}

bool FileService::createEmptyFile(const std::string& path)
{
	const auto startedAt = std::chrono::steady_clock::now();
	OperationLogger::log("FileService", "Create file requested for \"" + path + "\".");
	try
	{
		std::ofstream output(path, std::ios::binary);
		const bool created = output.good();
		const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
			std::chrono::steady_clock::now() - startedAt);
		OperationLogger::log(
			"FileService",
			std::string("Create file ") + (created ? "succeeded" : "failed") +
			" in " + std::to_string(elapsed.count()) + " ms.");
		return created;
	}
	catch (const std::exception& ex)
	{
		OperationLogger::log("FileService", "Create file failed with exception: " + std::string(ex.what()));
		return false;
	}
}

bool FileService::createDirectory(const std::string& path)
{
	const auto startedAt = std::chrono::steady_clock::now();
	OperationLogger::log("FileService", "Create folder requested for \"" + path + "\".");
	try
	{
		const bool created = fs::create_directory(path);
		const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
			std::chrono::steady_clock::now() - startedAt);
		OperationLogger::log(
			"FileService",
			std::string("Create folder ") + (created ? "succeeded" : "did not create a new folder") +
			" in " + std::to_string(elapsed.count()) + " ms.");
		return created;
	}
	catch (const std::exception& ex)
	{
		OperationLogger::log("FileService", "Create folder failed with exception: " + std::string(ex.what()));
		return false;
	}
}
