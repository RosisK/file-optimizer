#pragma once

#include <string>
#include <vector>
#include "FileInfo.h"

class FileService
{
public:
	std::vector<FileInfo> getDirectoryContent(const std::string& path);

	void copyFile(const std::string& src, const std::string& dest);
	void deleteFile(const std::string& path);
};