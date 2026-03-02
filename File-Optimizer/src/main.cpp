#include <iostream>
#include "FileService.h"

int main()
{
	FileService service;

	std::string path;
	std::cout << "Enter path: ";
	std::getline(std::cin, path);

	auto items = service.getDirectoryContent(path);

	for (auto& item : items)
	{
		std::cout << (item.isDirectory ? "[DIR] " : "[FILE] ");
		std::cout << item.name << " | Size: " << item.size << " Last Modified: " << item.modifiedTime << std::endl;
	}
}