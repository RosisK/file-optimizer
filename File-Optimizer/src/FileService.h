#pragma once

#include <string>
#include <vector>
#include "FileInfo.h"

class FileService
{
public:
	std::vector<FileInfo> getDirectoryContent(const std::string& path);

	
	//std::vector<FileInfo> getFiles(const std::string& path);
	//std::vector<FileInfo> getDirectories(const std::string& path);
};