#pragma once

#include <vector>
#include "FileInfo.h"
#include "FileSort.h"

class FileSorter
{
public:
	static void sort(std::vector<FileInfo>& items, const FileSortOptions& options);
};