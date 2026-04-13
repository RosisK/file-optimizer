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
			int comparison = 0;
			switch (options.key)
			{
				case FileSortKey::Name:
					comparison = a.name.compare(b.name);
					break;

				case FileSortKey::Size:
					if (a.size < b.size)
						comparison = -1;
					else if (a.size > b.size)
						comparison = 1;
					break;

				case FileSortKey::ModifiedTime:
					if (a.modifiedTime < b.modifiedTime)
						comparison = -1;
					else if (a.modifiedTime > b.modifiedTime)
						comparison = 1;
					break;
			}

			if (comparison == 0)
				comparison = a.name.compare(b.name);

			// 3. Ascending / Descending
			return options.ascending ? comparison < 0 : comparison > 0;
		});
}
