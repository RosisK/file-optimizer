#pragma once

#include <vector>
#include "FileInfo.h"

class FileSorter
{
public:
	static void sortByName(std::vector<FileInfo>& items);
	static void sortBySize(std::vector<FileInfo>& items);
};