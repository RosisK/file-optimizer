#include <sstream>
#include <algorithm>
#include <cctype>
#include <unordered_set>

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
	std::unordered_set<std::string> seenPaths;

	auto tokens = tokenize(query);

	for (const auto& token : tokens)
	{
		if (token.empty())
			continue;

		// Exact token matches first.
		auto exactMatch = index.find(token);
		if (exactMatch != index.end())
		{
			for (const auto& item : exactMatch->second)
			{
				if (seenPaths.insert(item.path).second)
					results.push_back(item);
			}
		}

		// Then allow partial token matches like "da" -> "dad_songs".
		for (const auto& [indexedToken, indexedItems] : index)
		{
			if (indexedToken == token || indexedToken.find(token) == std::string::npos)
				continue;

			for (const auto& item : indexedItems)
			{
				if (seenPaths.insert(item.path).second)
					results.push_back(item);
			}
		}
	}

	return results;
}
