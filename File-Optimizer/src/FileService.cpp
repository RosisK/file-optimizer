#include <filesystem>
#include <fstream>
#include <iostream>

#include "FileService.h"

namespace fs = std::filesystem;

std::vector<FileInfo> FileService::getDirectoryContent(const std::string& path)
{
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
	catch (const std::exception&)
	{
		// return empty list
	}

	return items;
}

bool FileService::copyFile(const std::string& src, const std::string& dest)
{
	try
	{
		return copyPath(src, dest);
	}
	catch (const std::exception&)
	{
		return false;
	}
}

bool FileService::deleteFile(const std::string& path)
{
	try
	{
		return deletePath(path);
	}
	catch(const std::exception&) 
	{
		return false;
	}
}

bool FileService::copyPath(const std::string& src, const std::string& dest)
{
	try
	{
		const fs::path source(src);
		const fs::path destination(dest);

		if (!fs::exists(source) || fs::exists(destination))
			return false;

		if (fs::is_directory(source))
		{
			fs::copy(source, destination, fs::copy_options::recursive);
		}
		else
		{
			fs::copy_file(source, destination, fs::copy_options::overwrite_existing);
		}

		return true;
	}
	catch (const std::exception&)
	{
		return false;
	}
}

bool FileService::deletePath(const std::string& path)
{
	try
	{
		const fs::path target(path);

		if (!fs::exists(target))
			return false;

		if (fs::is_directory(target))
			return fs::remove_all(target) > 0;

		return fs::remove(target);
	}
	catch (const std::exception&)
	{
		return false;
	}
}

bool FileService::renamePath(const std::string& sourcePath, const std::string& destinationPath)
{
	try
	{
		fs::rename(sourcePath, destinationPath);
		return true;
	}
	catch (const std::exception&)
	{
		return false;
	}
}

bool FileService::movePath(const std::string& sourcePath, const std::string& destinationPath)
{
	try
	{
		const fs::path source(sourcePath);
		const fs::path destination(destinationPath);

		if (!fs::exists(source) || fs::exists(destination))
			return false;

		std::error_code errorCode;
		fs::rename(source, destination, errorCode);
		if (!errorCode)
			return true;

		if (!copyPath(sourcePath, destinationPath))
			return false;

		return deletePath(sourcePath);
	}
	catch (const std::exception&)
	{
		return false;
	}
}

bool FileService::createEmptyFile(const std::string& path)
{
	try
	{
		std::ofstream output(path, std::ios::binary);
		return output.good();
	}
	catch (const std::exception&)
	{
		return false;
	}
}

bool FileService::createDirectory(const std::string& path)
{
	try
	{
		return fs::create_directory(path);
	}
	catch (const std::exception&)
	{
		return false;
	}
}
