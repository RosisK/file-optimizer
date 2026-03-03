#include <iostream>
#include <limits>
#include "FileService.h"
#include "FileSorter.h"

void showItems(const std::vector<FileInfo>& items)
{
    std::cout << "\n---- Directory Content ----\n";

    for (size_t i = 0; i < items.size(); i++)
    {
        const auto& item = items[i];

        std::cout << i << ": " << (item.isDirectory ? "[DIR]  " : "[FILE] ");
        std::cout << item.name
            << " | Size: " << item.size
            << " | Path: " << item.path
            << std::endl;
    }

    std::cout << "---------------------------\n";
}

void browseDirectory(FileService& service)
{
    std::string path;
    std::cout << "Enter directory path: ";
    std::getline(std::cin, path);

    auto items = service.getDirectoryContent(path);

    if (items.empty())
    {
        std::cout << "No items found or invalid path.\n";
        return;
    }

    showItems(items);

    int sortChoice;
    std::cout << "\nSort options:\n";
    std::cout << "1. Sort by name\n";
    std::cout << "2. Sort by size\n";
    std::cout << "0. No sorting\n";
    std::cout << "Choice: ";
    std::cin >> sortChoice;
    std::cin.ignore(std::numeric_limits<std::streamsize>::max(), '\n');

    if (sortChoice == 1)
        FileSorter::sortByName(items);
    else if (sortChoice == 2)
        FileSorter::sortBySize(items);

    if (sortChoice != 0)
    {
        std::cout << "\nAfter sorting:\n";
        showItems(items);
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