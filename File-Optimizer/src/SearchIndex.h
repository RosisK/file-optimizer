#pragma once

#include <string>
#include <vector>
#include <unordered_map>
#include "FileInfo.h"

class SearchIndex
{
private:
	std::unordered_map<std::string, std::vector<FileInfo>> index;
	
	std::vector<std::string> tokenize(const std::string& text);

public:
	void buildIndex(const std::vector<FileInfo>& items);
	
	std::vector<FileInfo> search(const std::string& query);
};