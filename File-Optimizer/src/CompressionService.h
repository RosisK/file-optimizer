#pragma once

#include <string>

enum class CompressionFormat
{
	Zstd = 0,
	Zip = 1
};

bool compressPath(
	const std::string& inputPath,
	const std::string& outputPath,
	CompressionFormat format,
	std::string& errorMessage);

bool decompressPath(
	const std::string& inputPath,
	const std::string& outputPath,
	CompressionFormat format,
	std::string& errorMessage);
