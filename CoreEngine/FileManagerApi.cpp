#include "FileManagerApi.h"

#include <algorithm>
#include <filesystem>
#include <string>
#include <vector>

#ifndef NOMINMAX
#define NOMINMAX
#endif

#include <windows.h>

#include "..\File-Optimizer\src\FileInfo.h"
#include "..\File-Optimizer\src\FileService.h"
#include "..\File-Optimizer\src\FileSort.h"
#include "..\File-Optimizer\src\FileSorter.h"
#include "..\File-Optimizer\src\SearchIndex.h"
#include "..\File-Optimizer\src\ZstdCompressor.h"

namespace
{
	thread_local std::wstring g_lastError;

	std::string wideToUtf8(const wchar_t* text)
	{
		if (text == nullptr)
		{
			return {};
		}

		const int requiredSize = WideCharToMultiByte(CP_UTF8, 0, text, -1, nullptr, 0, nullptr, nullptr);
		if (requiredSize <= 0)
		{
			return {};
		}

		std::string buffer(requiredSize, '\0');
		WideCharToMultiByte(CP_UTF8, 0, text, -1, buffer.data(), requiredSize, nullptr, nullptr);
		buffer.pop_back();
		return buffer;
	}

	std::wstring utf8ToWide(const std::string& text)
	{
		if (text.empty())
		{
			return {};
		}

		const int requiredSize = MultiByteToWideChar(CP_UTF8, 0, text.c_str(), -1, nullptr, 0);
		if (requiredSize <= 0)
		{
			return {};
		}

		std::wstring buffer(requiredSize, L'\0');
		MultiByteToWideChar(CP_UTF8, 0, text.c_str(), -1, buffer.data(), requiredSize);
		buffer.pop_back();
		return buffer;
	}

	void setLastError(const std::wstring& message)
	{
		g_lastError = message;
	}

	void clearLastError()
	{
		g_lastError.clear();
	}

	void copyWideString(wchar_t* destination, size_t capacity, const std::wstring& source)
	{
		if (destination == nullptr || capacity == 0)
		{
			return;
		}

		wcsncpy_s(destination, capacity, source.c_str(), _TRUNCATE);
	}

	FileSortOptions toSortOptions(int sortKey, int ascending, int directoriesFirst)
	{
		FileSortOptions options;
		options.ascending = ascending != 0;
		options.directoriesFirst = directoriesFirst != 0;

		switch (sortKey)
		{
		case CoreSortBySize:
			options.key = FileSortKey::Size;
			break;
		case CoreSortByModifiedTime:
			options.key = FileSortKey::ModifiedTime;
			break;
		case CoreSortByName:
		default:
			options.key = FileSortKey::Name;
			break;
		}

		return options;
	}

	int copyResults(const std::vector<FileInfo>& source, CoreFileInfo* items, int maxItems)
	{
		if (items == nullptr || maxItems <= 0)
		{
			return static_cast<int>(source.size());
		}

		const int count = std::min(maxItems, static_cast<int>(source.size()));

		for (int i = 0; i < count; ++i)
		{
			const auto& item = source[i];
			copyWideString(items[i].name, CORE_NAME_CAPACITY, utf8ToWide(item.name));
			copyWideString(items[i].path, CORE_PATH_CAPACITY, utf8ToWide(item.path));
			items[i].size = static_cast<unsigned long long>(item.size);
			items[i].modifiedTime = static_cast<long long>(item.modifiedTime);
			items[i].isDirectory = item.isDirectory ? 1 : 0;
		}

		return count;
	}

	std::vector<FileInfo> getSortedItems(const wchar_t* path, int sortKey, int ascending, int directoriesFirst)
	{
		if (path == nullptr || *path == L'\0')
		{
			throw std::runtime_error("Path is required.");
		}

		const std::filesystem::path fsPath(path);
		if (!std::filesystem::exists(fsPath))
		{
			throw std::runtime_error("Path does not exist.");
		}

		if (!std::filesystem::is_directory(fsPath))
		{
			throw std::runtime_error("Path is not a directory.");
		}

		FileService service;
		auto items = service.getDirectoryContent(wideToUtf8(path));
		FileSorter::sort(items, toSortOptions(sortKey, ascending, directoriesFirst));
		return items;
	}
}

int Core_GetDirectoryContents(
	const wchar_t* path,
	CoreFileInfo* items,
	int maxItems,
	int sortKey,
	int ascending,
	int directoriesFirst)
{
	try
	{
		clearLastError();
		auto results = getSortedItems(path, sortKey, ascending, directoriesFirst);
		return copyResults(results, items, maxItems);
	}
	catch (const std::exception& ex)
	{
		setLastError(utf8ToWide(ex.what()));
		return -1;
	}
}

int Core_SearchDirectoryContents(
	const wchar_t* path,
	const wchar_t* query,
	CoreFileInfo* items,
	int maxItems,
	int sortKey,
	int ascending,
	int directoriesFirst)
{
	try
	{
		clearLastError();

		auto directoryItems = getSortedItems(path, sortKey, ascending, directoriesFirst);
		SearchIndex index;
		index.buildIndex(directoryItems);

		auto results = index.search(wideToUtf8(query));
		FileSorter::sort(results, toSortOptions(sortKey, ascending, directoriesFirst));
		return copyResults(results, items, maxItems);
	}
	catch (const std::exception& ex)
	{
		setLastError(utf8ToWide(ex.what()));
		return -1;
	}
}

int Core_CopyFile(const wchar_t* sourcePath, const wchar_t* destinationPath)
{
	try
	{
		clearLastError();
		FileService service;
		if (!service.copyFile(wideToUtf8(sourcePath), wideToUtf8(destinationPath)))
		{
			setLastError(L"Copy failed.");
			return 0;
		}

		return 1;
	}
	catch (const std::exception& ex)
	{
		setLastError(utf8ToWide(ex.what()));
		return 0;
	}
}

int Core_DeleteFile(const wchar_t* path)
{
	try
	{
		clearLastError();
		FileService service;
		if (!service.deleteFile(wideToUtf8(path)))
		{
			setLastError(L"Delete failed.");
			return 0;
		}

		return 1;
	}
	catch (const std::exception& ex)
	{
		setLastError(utf8ToWide(ex.what()));
		return 0;
	}
}

int Core_CompressFile(const wchar_t* sourcePath, const wchar_t* destinationPath)
{
	clearLastError();
	if (!compressFile(wideToUtf8(sourcePath), wideToUtf8(destinationPath)))
	{
		setLastError(L"Compression failed.");
		return 0;
	}

	return 1;
}

int Core_DecompressFile(const wchar_t* sourcePath, const wchar_t* destinationPath)
{
	clearLastError();
	if (!decompressFile(wideToUtf8(sourcePath), wideToUtf8(destinationPath)))
	{
		setLastError(L"Decompression failed.");
		return 0;
	}

	return 1;
}

int Core_GetLastErrorMessage(wchar_t* buffer, int bufferLength)
{
	if (buffer == nullptr || bufferLength <= 0)
	{
		return static_cast<int>(g_lastError.size());
	}

	wcsncpy_s(buffer, bufferLength, g_lastError.c_str(), _TRUNCATE);
	return static_cast<int>(g_lastError.size());
}
