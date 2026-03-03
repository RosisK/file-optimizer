#pragma once

#include "FileInfo.h"

enum class FileSortKey
{
	Name,
	Size,
	ModifiedTime
};

struct FileSortOptions
{
	FileSortKey key = FileSortKey::Name;
	bool ascending = true;
	bool directoriesFirst = true;
};