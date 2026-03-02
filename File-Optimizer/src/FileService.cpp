#include "FileService.h"
#include <filesystem>
#include <iostream>

namespace fs = std::filesystem;

std::vector<FileInfo> FileService::getFiles(const std::string& path)
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

			auto time = fs::last_write_time(entry);
			info.modifiedTime = 
		}
	}
	catch (const std::exception& e)
	{
		std::cerr << "Error: " << e.what() << std::endl;
		return { "Error: Invalid Path" };
	}

	return files;
}

std::vector<FileInfo> FileService::getDirectories(const std::string& path)
{
	std::vector<std::string> dirs;

	try
	{
		for (const auto& entry : fs::directory_iterator(path))
		{
			if (entry.is_directory())
				dirs.push_back(entry.path().string());
		}
	}
	catch (const std::exception& e)
	{
		std::cerr << "Error: " << e.what() << std::endl;
		return { "Error: Invalid Path" };
	}
	return dirs;
}