#include <iostream>
#include "FileService.h"

int main()
{
	FileService service;

	std::string path;
	std::cout << "Enter path: ";
	std::getline(std::cin, path);

	auto items = service.getDirectoryContent(path);
	std::string src = path + "text.txt";
	std::string dest = path + "copy.txt";
	service.copyFile(src, dest);

	for (auto& item : items)
	{
		std::cout << (item.isDirectory ? "[DIR] " : "[FILE] ");
		std::cout << item.name << " | Size: " << item.size << " Last Modified: " << item.modifiedTime << std::endl;
	}
}