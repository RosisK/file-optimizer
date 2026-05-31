#include "FileSorter.h"
#include "OperationLogger.h"
#include <algorithm>
#include <chrono>

namespace
{
	std::string sortKeyToString(FileSortKey key)
	{
		switch (key)
		{
		case FileSortKey::Size:
			return "Size";
		case FileSortKey::ModifiedTime:
			return "ModifiedTime";
		case FileSortKey::Name:
		default:
			return "Name";
		}
	}
}

void FileSorter::sort(std::vector<FileInfo>& items, const FileSortOptions& options)
{
	const auto startedAt = std::chrono::steady_clock::now();

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

	const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
		std::chrono::steady_clock::now() - startedAt);
	OperationLogger::log(
		"FileSorter",
		"Sort " + std::to_string(items.size()) + " item(s) by " + sortKeyToString(options.key) +
		" (" + (options.ascending ? "asc" : "desc") +
		(options.directoriesFirst ? ", dirs first" : "") + ") -> " +
		std::to_string(elapsed.count()) + " ms.");
}
