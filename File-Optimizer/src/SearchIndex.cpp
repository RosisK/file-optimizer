#include <sstream>
#include <algorithm>
#include <cctype>

#include "SearchIndex.h"

std::vector<std::string> SearchIndex::tokenize(const std::string& text)
{
	std::vector<std::string> tokens;
	std::string cleaned = text;

	// Convert to lowercase
	std::transform(cleaned.begin(), cleaned.end(), cleaned.begin(), [](unsigned char c) { return std::tolower(c); });

	// Replace separators with space
	for (char& c : cleaned)
	{
		if (c == '_' || c == '-' || c == '.')
			c = ' ';
	}

	std::stringstream ss(cleaned);
	std::string word;

	while (ss >> word)
		tokens.push_back(word);

	return tokens;
}

void SearchIndex::buildIndex(const std::vector<FileInfo>& items)
{
	index.clear();

	for (const auto& file : items)
	{
		auto tokens = tokenize(file.name);

		for (const auto& token : tokens)
		{
			index[token].push_back(file);
		}
	}
}

std::vector<FileInfo> SearchIndex::search(const std::string& query)
{
	std::vector<FileInfo> results;

	auto tokens = tokenize(query);

	for (const auto& token : tokens)
	{
		auto it = index.find(token);

		if (it != index.end())
		{
			results.insert(results.end(), it->second.begin(), it->second.end());
		}
	}

	return results;
}