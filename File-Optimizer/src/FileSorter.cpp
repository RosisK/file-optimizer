#include "FileSorter.h"
#include <algorithm>

void FileSorter::sortByName(std::vector<FileInfo>& items)
{
    std::sort(items.begin(), items.end(),
        [](const FileInfo& a, const FileInfo& b)
        {
            return a.name < b.name;
        });
}


void FileSorter::sortBySize(std::vector<FileInfo>& items)
{
    std::sort(items.begin(), items.end(),
        [](const FileInfo& a, const FileInfo& b)
        {
            return a.size < b.size;
        });
}