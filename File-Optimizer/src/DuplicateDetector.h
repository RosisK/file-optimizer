#pragma once

#include <string>
#include <vector>

#include "FileInfo.h"

struct DuplicateGroup
{
	std::string key;
	std::vector<FileInfo> items;
};

class DuplicateDetector
{
public:
	std::vector<DuplicateGroup> findDuplicateNames(const std::string& rootPath);
	std::vector<DuplicateGroup> findDuplicateContents(const std::string& rootPath);

private:
	std::vector<FileInfo> collectFiles(const std::string& rootPath);
};
