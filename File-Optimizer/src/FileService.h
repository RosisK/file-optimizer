#pragma once

#include <string>
#include <vector>

class FileService
{
public:
	std::vector<std::string> getFiles(const std::string& path);
	std::vector<std::string> getDirectories(const std::string& path);
};