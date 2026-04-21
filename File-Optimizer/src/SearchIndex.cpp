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

	OperationLogger::log(
		"SearchIndex",
		"Tokenized " + quoteText(text) + " -> [" + OperationLogger::join(tokens) + "]");

	return tokens;
}

void SearchIndex::buildIndex(const std::vector<FileInfo>& items)
{
	const auto startedAt = std::chrono::steady_clock::now();
	index.clear();
	OperationLogger::log("SearchIndex", "Building inverted index for " + std::to_string(items.size()) + " item(s).");

	size_t loggedItems = 0;
	for (const auto& file : items)
	{
		auto tokens = tokenize(file.name);
		if (loggedItems < 12)
		{
			OperationLogger::log(
				"SearchIndex",
				"Indexed item " + quoteText(file.name) + " with token(s): [" + OperationLogger::join(tokens) + "]");
		}
		++loggedItems;

		for (const auto& token : tokens)
		{
			index[token].push_back(file);
		}
	}

	if (items.size() > 12)
	{
		OperationLogger::log(
			"SearchIndex",
			"Skipped detailed token logs for " + std::to_string(items.size() - 12) + " additional item(s) to keep the trace readable.");
	}

	size_t postingCount = 0;
	size_t loggedTokens = 0;
	for (const auto& [token, files] : index)
	{
		postingCount += files.size();
		if (loggedTokens < 10)
		{
			OperationLogger::log(
				"SearchIndex",
				"Posting list preview: token " + quoteText(token) + " -> " + std::to_string(files.size()) + " match(es).");
		}

		++loggedTokens;
	}

	const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
		std::chrono::steady_clock::now() - startedAt);
	OperationLogger::log(
		"SearchIndex",
		"Inverted index ready with " + std::to_string(index.size()) + " unique token(s), " +
		std::to_string(postingCount) + " posting(s), built in " + std::to_string(elapsed.count()) + " ms.");
}

std::vector<FileInfo> SearchIndex::search(const std::string& query)
{
	const auto startedAt = std::chrono::steady_clock::now();
	std::vector<FileInfo> results;
	std::unordered_set<std::string> seenPaths;

	auto tokens = tokenize(query);
	OperationLogger::log(
		"SearchIndex",
		"Searching for query " + quoteText(query) + " using token(s): [" + OperationLogger::join(tokens) + "]");

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

		OperationLogger::log(
			"SearchIndex",
			"Token " + quoteText(token) + " produced " + std::to_string(exactHits) +
			" exact and " + std::to_string(partialHits) + " partial new match(es).");
	}

	const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
		std::chrono::steady_clock::now() - startedAt);
	OperationLogger::log(
		"SearchIndex",
		"Search completed with " + std::to_string(results.size()) + " result(s) in " +
		std::to_string(elapsed.count()) + " ms.");

	return results;
}
