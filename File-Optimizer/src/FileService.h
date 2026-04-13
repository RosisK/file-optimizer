#pragma once

#include <string>
#include <vector>
#include "FileInfo.h"

class FileService
{
public:
	std::vector<FileInfo> getDirectoryContent(const std::string& path);

	bool copyFile(const std::string& src, const std::string& dest);
	bool deleteFile(const std::string& path);
};
