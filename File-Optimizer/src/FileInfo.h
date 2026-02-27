#pragma once
#include <string>
#include <ctime>

struct FileInfo
{
	std::string name;
	std::string path;
	uintmax_t size;
	std::time_t modifiedTime;
	bool isDirectory;
};