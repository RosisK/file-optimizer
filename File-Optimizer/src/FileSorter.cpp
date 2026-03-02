#include "FileSorter.h"
#include <algorithm>

void FileSorter::sortByName(std::vector<FileInfo>& items)
{
    std::sort(items.begin(), items.end(),
        [](const FileInfo& a, const FileInfo& b)
        {
            // Directories listed first
            if (a.isDirectory != b.isDirectory)
                return a.isDirectory > b.isDirectory;
            return a.name < b.name;
        });
}


void FileSorter::sortBySize(std::vector<FileInfo>& items)
{
    std::sort(items.begin(), items.end(),
        [](const FileInfo& a, const FileInfo& b)
        {
            if (a.isDirectory != b.isDirectory)
                return a.isDirectory > b.isDirectory;
            return a.size < b.size;
        });
}