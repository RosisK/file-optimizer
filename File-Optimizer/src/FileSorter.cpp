#include "FileSorter.h"
#include <algorithm>

void FileSorter::sort(std::vector<FileInfo>& items, const FileSortOptions& options)
{
	std::sort(items.begin(), items.end(),
		[&](const FileInfo& a, const FileInfo& b)
		{
			// 1. Directories first
			if (options.directoriesFirst && a.isDirectory != b.isDirectory)
				return a.isDirectory > b.isDirectory;

			// 2. Compare by selected key
			bool result = false;

			switch (options.key)
			{
				case FileSortKey::Name:
					result = a.name < b.name;
					break;

				case FileSortKey::Size:
					result = a.size < b.size;
					break;

				case FileSortKey::ModifiedTime:
					result = a.modifiedTime < b.modifiedTime;
					break;
			}

			// 3.Ascending / Descending
			return options.ascending ? result : !result;
		});
}