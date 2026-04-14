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
	bool copyPath(const std::string& src, const std::string& dest);
	bool deletePath(const std::string& path);
	bool renamePath(const std::string& sourcePath, const std::string& destinationPath);
	bool movePath(const std::string& sourcePath, const std::string& destinationPath);
	bool createEmptyFile(const std::string& path);
	bool createDirectory(const std::string& path);
};
