#include <iostream>
#include <limits>
#include <filesystem>

#include "FileService.h"
#include "FileSorter.h"

namespace fs = std::filesystem;

void showItems(const std::vector<FileInfo>& items)
{
    std::cout << "\n---- Directory Content ----\n";

    for (size_t i = 0; i < items.size(); i++)
    {
        const auto& item = items[i];

        std::cout << i << ": "
            << (item.isDirectory ? "[DIR]  " : "[FILE] ")
            << item.name
            << " | Size: " << item.size
            << '\n';
    }

    std::cout << "---------------------------\n";
}


void browseDirectory(FileService& service)
{
    std::string currentPath;
    std::cout << "Enter starting directory path: ";
    std::getline(std::cin, currentPath);

    FileSortOptions sortOptions;

    while (true)
    {
        auto items = service.getDirectoryContent(currentPath);

        if (items.empty())
        {
            std::cout << "Cannot open directory.\n";
            return;
        }

        FileSorter::sort(items, sortOptions);

        std::cout << "\nCurrent path: " << currentPath << '\n';
        showItems(items);

        std::cout << "\nOptions:\n";
        std::cout << "  [number] Enter directory\n";
        std::cout << "  u        Go up\n";
        std::cout << "  s        Change sorting\n";
        std::cout << "  q        Exit browser\n";
        std::cout << "Choice: ";

        std::string input;
        std::getline(std::cin, input);

        if (input == "q")
            break;

        if (input == "u")
        {
            fs::path p(currentPath);
            if (p.has_parent_path())
                currentPath = p.parent_path().string();
            continue;
        }

        if (input == "s")
        {
            std::cout << "\nSort by:\n";
            std::cout << "1. Name\n";
            std::cout << "2. Size\n";
            std::cout << "3. Modified time\n";
            std::cout << "Choice: ";

            int keyChoice;
            std::cin >> keyChoice;
            std::cin.ignore(std::numeric_limits<std::streamsize>::max(), '\n');

            switch (keyChoice)
            {
                case 1:
                    sortOptions.key = FileSortKey::Name;
                    break;
                case 2:
                    sortOptions.key = FileSortKey::Size;
                    break;
                case 3:
                    sortOptions.key = FileSortKey::ModifiedTime;
                    break;
                default:
                    std::cout << "Invalid sort key.\n";
                    continue;
            }

            std::cout << "Ascending? (1 = yes, 0 = no): ";
            std::cin >> sortOptions.ascending;
            std::cin.ignore(std::numeric_limits<std::streamsize>::max(), '\n');

            std::cout << "Directories first? (1 = yes, 0 = no): ";
            std::cin >> sortOptions.directoriesFirst;
            std::cin.ignore(std::numeric_limits<std::streamsize>::max(), '\n');

            continue;
        }

        try
        {
            int index = std::stoi(input);
            if (index >= 0 && index < static_cast<int>(items.size()))
            {
                if (items[index].isDirectory)
                    currentPath = items[index].path;
                else
                    std::cout << "Not a directory.\n";
            }
        }
        catch (...)
        {
            std::cout << "Invalid input.\n";
        }
    }
}

void copyFileTest(FileService& service)
{
    std::string src, dest;

    std::cout << "Enter source file path: ";
    std::getline(std::cin, src);

    std::cout << "Enter destination path: ";
    std::getline(std::cin, dest);

    service.copyFile(src, dest);

    std::cout << "Copy attempted.\n";
}

void deleteFileTest(FileService& service)
{
    std::string path;

    std::cout << "Enter file path to delete: ";
    std::getline(std::cin, path);

    service.deleteFile(path);

    std::cout << "Delete attempted.\n";
}

int main()
{
    FileService service;
    int choice = -1;

    while (choice != 0)
    {
        std::cout << "\n===== File Manager Test Console =====\n";
        std::cout << "1. Browse directory\n";
        std::cout << "2. Copy file\n";
        std::cout << "3. Delete file\n";
        std::cout << "0. Exit\n";
        std::cout << "Select option: ";

        std::cin >> choice;
        std::cin.ignore(std::numeric_limits<std::streamsize>::max(), '\n');

        switch (choice)
        {
        case 1:
            browseDirectory(service);
            break;
        case 2:
            copyFileTest(service);
            break;
        case 3:
            deleteFileTest(service);
            break;
        case 0:
            break;
        default:
            std::cout << "Invalid choice\n";
        }
    }

    return 0;
}