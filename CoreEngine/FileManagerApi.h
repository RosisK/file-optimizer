#pragma once

#include <cstdint>

#ifdef COREENGINE_EXPORTS
#define CORE_API extern "C" __declspec(dllexport)
#else
#define CORE_API extern "C" __declspec(dllimport)
#endif

constexpr int CORE_NAME_CAPACITY = 260;
constexpr int CORE_PATH_CAPACITY = 1024;

enum CoreSortKey
{
	CoreSortByName = 0,
	CoreSortBySize = 1,
	CoreSortByModifiedTime = 2
};

enum CoreCompressionFormat
{
	CoreCompressionZstd = 0,
	CoreCompressionZip = 1
};

struct CoreFileInfo
{
	wchar_t name[CORE_NAME_CAPACITY];
	wchar_t path[CORE_PATH_CAPACITY];
	unsigned long long size;
	long long modifiedTime;
	int isDirectory;
};

CORE_API int Core_GetDirectoryContents(
	const wchar_t* path,
	CoreFileInfo* items,
	int maxItems,
	int sortKey,
	int ascending,
	int directoriesFirst);

CORE_API int Core_SearchDirectoryContents(
	const wchar_t* path,
	const wchar_t* query,
	CoreFileInfo* items,
	int maxItems,
	int sortKey,
	int ascending,
	int directoriesFirst);

CORE_API int Core_CopyFile(const wchar_t* sourcePath, const wchar_t* destinationPath);
CORE_API int Core_DeleteFile(const wchar_t* path);
CORE_API int Core_CompressFile(const wchar_t* sourcePath, const wchar_t* destinationPath);
CORE_API int Core_DecompressFile(const wchar_t* sourcePath, const wchar_t* destinationPath);
CORE_API int Core_CompressPath(const wchar_t* sourcePath, const wchar_t* destinationPath, int format);
CORE_API int Core_DecompressPath(const wchar_t* sourcePath, const wchar_t* destinationPath, int format);
CORE_API int Core_GetLastErrorMessage(wchar_t* buffer, int bufferLength);
