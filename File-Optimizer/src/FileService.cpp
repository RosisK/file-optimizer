#include "FileService.h"
#include <filesystem>

namespace fs = std::filesystem;

std::vector<std::string> FileService::getFiles(const std::string& path)
{
	std::vector<std::string> files;

	for (const auto& entry : fs::directory_iterator(path))
	{
		if (entry.is_regular_file())
			files.push_back(entry.path().string());
	}

	return files;
}

std::vector<std::string> FileService::getDirectories(const std::string& path)
{
	std::vector<std::string> dirs;

	for (const auto& entry : fs::directory_iterator(path))
	{
		if (entry.is_directory())
			dirs.push_back(entry.path().string());
	}

	return dirs;
}