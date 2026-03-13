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