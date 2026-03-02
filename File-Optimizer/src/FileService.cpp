#include <filesystem>


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

void FileService::copyFile(const std::string& src, const std::string& dest)
{
	try
	{
		fs::copy(src, dest, fs::copy_options::overwrite_existing);
	}
	catch (const std::exception&)
	{
	}
}

void FileService::deleteFile(const std::string& path)
{
	try
	{
		fs::remove(path);
	}
	catch(const std::exception&) 
	{
	}
}