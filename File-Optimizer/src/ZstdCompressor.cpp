#include "ZstdCompressor.h"
#include <fstream>
#include <vector>
#include <zstd.h>

// Compress the file from inputPath to outputPath
bool compressFile(const std::string& inputPath, const std::string& outputPath) {
	std::ifstream input(inputPath, std::ios::binary);
	if (!input) return false;
	
	std::vector<char> data((std::istreambuf_iterator<char>(input)), std::istreambuf_iterator<char>());

	// Reserving space for compressed data
	std::vector<char> compressed(ZSTD_compressBound(data.size()));

	size_t compressedSize = ZSTD_compress(compressed.data(), compressed.size(), data.data(), data.size(), 1);

	if (ZSTD_isError(compressedSize)) return false;

	std::ofstream output(outputPath, std::ios::binary);
	output.write(compressed.data(), compressedSize);
	return true;
}

// Decompress the file from inputPath to outputPath
bool decompressFile(const std::string& inputPath, const std::string& outputPath) {
	std::ifstream input(inputPath, std::ios::binary);
	if (!input) return false;

	// Read the compressed data
	std::vector<char> compressed((std::istreambuf_iterator<char>(input)), std::istreambuf_iterator<char>());

	// Get the size fo the decompressed data
	unsigned long long decompressedSize = ZSTD_getFrameContentSize(compressed.data(), compressed.size());
	if (decompressedSize == ZSTD_CONTENTSIZE_ERROR || decompressedSize == ZSTD_CONTENTSIZE_UNKNOWN) return false;
	
	std::vector<char> decompressed(decompressedSize);
	size_t result = ZSTD_decompress(decompressed.data(), decompressed.size(), compressed.data(), compressed.size());

	if (ZSTD_isError(result)) return false;

	// Write the decompressed data to a file
	std::ofstream output(outputPath, std::ios::binary);
	output.write(decompressed.data(), decompressed.size());
	return true;
}