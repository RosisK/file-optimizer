#include "FileService.h"
#include <filesystem>
#include <iostream>

namespace fs = std::filesystem;

std::vector<std::string> FileService::getFiles(const std::string& path)
{
	std::vector<std::string> files;

	try
	{
		for (const auto& entry : fs::directory_iterator(path))
		{
			if (entry.is_regular_file())
				files.push_back(entry.path().string());
		}
	}
	catch (const std::exception& e)
	{
		std::cerr << "Error: " << e.what() << std::endl;
		return { "Error: Invalid Path" };
	}

	return files;
}

std::vector<std::string> FileService::getDirectories(const std::string& path)
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