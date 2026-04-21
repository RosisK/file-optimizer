#include <sstream>
#include <algorithm>
#include <cctype>
#include <chrono>
#include <unordered_set>

#include "OperationLogger.h"
#include "SearchIndex.h"

namespace
{
	std::string quoteText(const std::string& text)
	{
		return "\"" + text + "\"";
	}
}

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
	const auto startedAt = std::chrono::steady_clock::now();
	index.clear();
	for (const auto& file : items)
	{
		auto tokens = tokenize(file.name);

		for (const auto& token : tokens)
		{
			index[token].push_back(file);
		}
	}

	size_t postingCount = 0;
	for (const auto& [token, files] : index)
	{
		postingCount += files.size();
	}

	const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
		std::chrono::steady_clock::now() - startedAt);
	OperationLogger::log(
		"SearchIndex",
		"Index built for " + std::to_string(items.size()) + " item(s) -> " +
		std::to_string(index.size()) + " token(s), " + std::to_string(postingCount) +
		" posting(s), " + std::to_string(elapsed.count()) + " ms. Rule: lowercase + split on _, -, .");
}

std::vector<FileInfo> SearchIndex::search(const std::string& query)
{
	const auto startedAt = std::chrono::steady_clock::now();
	std::vector<FileInfo> results;
	std::unordered_set<std::string> seenPaths;
	std::vector<std::string> tokenBreakdown;

	auto tokens = tokenize(query);

	for (const auto& token : tokens)
	{
		if (token.empty())
			continue;

		size_t exactHits = 0;
		size_t partialHits = 0;

		// Exact token matches first.
		auto exactMatch = index.find(token);
		if (exactMatch != index.end())
		{
			for (const auto& item : exactMatch->second)
			{
				if (seenPaths.insert(item.path).second)
				{
					results.push_back(item);
					++exactHits;
				}
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
				{
					results.push_back(item);
					++partialHits;
				}
			}
		}

		tokenBreakdown.push_back(
			token + " (exact=" + std::to_string(exactHits) +
			", partial=" + std::to_string(partialHits) + ")");
	}

	const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
		std::chrono::steady_clock::now() - startedAt);
	OperationLogger::log(
		"SearchIndex",
		"Query " + quoteText(query) + " -> tokens [" + OperationLogger::join(tokens) + "]" +
		(tokenBreakdown.empty() ? std::string() : " | matches: " + OperationLogger::join(tokenBreakdown, "; ")) +
		" | results=" + std::to_string(results.size()) +
		" | " + std::to_string(elapsed.count()) + " ms.");

	return results;
}
