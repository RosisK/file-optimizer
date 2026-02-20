#include <iostream>
#include "FileService.h"

int main()
{
	FileService service;

	std::string path;
	std::cout << "Enter path: ";
	std::getline(std::cin, path);

	auto dirs = service.getDirectories(path);
	auto files = service.getFiles(path);

	std::cout << "\nDirectories:\n";
	for (std::string& d : dirs)
		std::cout << d << std::endl;

	std::cout << "\nFiles:\n";
	for (auto& f : files)
		std::cout << f << std::endl;
}