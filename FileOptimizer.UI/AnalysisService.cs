using System.Diagnostics;
using System.Text;

namespace FileOptimizer.UI;

internal static class AnalysisService
{
    public static string GetStorageSummary(string path)
    {
        var stopwatch = Stopwatch.StartNew();

        long totalBytes = 0;
        int fileCount = 0;
        int directoryCount = 0;

        var root = new DirectoryInfo(path);
        if (!root.Exists)
        {
            throw new DirectoryNotFoundException("The selected analysis path does not exist.");
        }

        CountDirectory(root, ref fileCount, ref directoryCount, ref totalBytes);
        stopwatch.Stop();

        var report = new StringBuilder();
        report.AppendLine("Storage Summary");
        report.AppendLine($"Path: {path}");
        report.AppendLine($"Folders: {directoryCount:N0}");
        report.AppendLine($"Files: {fileCount:N0}");
        report.AppendLine($"Total size: {FormatBytes(totalBytes)}");
        report.AppendLine($"Scan time: {stopwatch.Elapsed.TotalMilliseconds:N2} ms");
        return report.ToString();
    }

    public static string RunSearchBenchmark(string path, string query, int iterations)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new InvalidOperationException("Enter a search query first.");
        }

        var sort = new NativeSortOptions(SortOption.Name, true, true);
        var stopwatch = Stopwatch.StartNew();
        int resultCount = 0;

        for (int i = 0; i < iterations; i++)
        {
            resultCount = NativeMethods.SearchDirectoryContents(path, query, sort).Count;
        }

        stopwatch.Stop();

        var avgMs = stopwatch.Elapsed.TotalMilliseconds / iterations;
        var report = new StringBuilder();
        report.AppendLine("Search Benchmark");
        report.AppendLine($"Path: {path}");
        report.AppendLine($"Query: {query}");
        report.AppendLine($"Iterations: {iterations}");
        report.AppendLine($"Last result count: {resultCount}");
        report.AppendLine($"Total time: {stopwatch.Elapsed.TotalMilliseconds:N2} ms");
        report.AppendLine($"Average time: {avgMs:N2} ms");
        return report.ToString();
    }

    public static string RunSortBenchmark(string path, int iterations)
    {
        var report = new StringBuilder();
        report.AppendLine("Sort Benchmark");
        report.AppendLine($"Path: {path}");
        report.AppendLine($"Iterations per case: {iterations}");

        foreach (var option in Enum.GetValues<SortOption>())
        {
            var stopwatch = Stopwatch.StartNew();
            int itemCount = 0;

            for (int i = 0; i < iterations; i++)
            {
                itemCount = NativeMethods.GetDirectoryContents(path, new NativeSortOptions(option, true, true)).Count;
            }

            stopwatch.Stop();
            report.AppendLine($"{option}: {stopwatch.Elapsed.TotalMilliseconds / iterations:N2} ms avg over {itemCount} item(s)");
        }

        return report.ToString();
    }

    public static string RunCompressionBenchmark(string filePath)
    {
        var source = new FileInfo(filePath);
        if (!source.Exists)
        {
            throw new FileNotFoundException("Choose an existing file for compression benchmarking.", filePath);
        }

        var tempRoot = Path.Combine(Path.GetTempPath(), "FileOptimizerBench", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        try
        {
            var report = new StringBuilder();
            report.AppendLine("Compression Benchmark");
            report.AppendLine($"Source file: {source.FullName}");
            report.AppendLine($"Source size: {FormatBytes(source.Length)}");
            report.AppendLine();

            report.AppendLine(RunSingleCompressionBenchmark(source.FullName, source.Length, CompressionFormat.Zstd, Path.Combine(tempRoot, $"{source.Name}.zst")));
            report.AppendLine();
            report.AppendLine(RunSingleCompressionBenchmark(source.FullName, source.Length, CompressionFormat.Zip, Path.Combine(tempRoot, $"{source.Name}.zip")));

            return report.ToString();
        }
        finally
        {
            try
            {
                Directory.Delete(tempRoot, true);
            }
            catch
            {
                // Best-effort cleanup for temporary benchmark files.
            }
        }
    }

    public static string RunDuplicateNameAnalysis(string path)
    {
        var stopwatch = Stopwatch.StartNew();
        var duplicates = NativeMethods.FindDuplicateNames(path);
        stopwatch.Stop();

        return FormatDuplicateReport("Duplicate Name Analysis", path, duplicates, stopwatch.Elapsed);
    }

    public static string RunDuplicateContentAnalysis(string path)
    {
        var stopwatch = Stopwatch.StartNew();
        var duplicates = NativeMethods.FindDuplicateContents(path);
        stopwatch.Stop();

        return FormatDuplicateReport("Duplicate Content Analysis", path, duplicates, stopwatch.Elapsed);
    }

    private static string RunSingleCompressionBenchmark(string sourcePath, long sourceSize, CompressionFormat format, string outputPath)
    {
        var stopwatch = Stopwatch.StartNew();
        NativeMethods.CompressPath(sourcePath, outputPath, format);
        stopwatch.Stop();

        var outputInfo = new FileInfo(outputPath);
        if (!outputInfo.Exists)
        {
            throw new InvalidOperationException($"{format} benchmark output was not created.");
        }

        var ratio = sourceSize == 0 ? 0 : (double)outputInfo.Length / sourceSize;

        return $"{format}\n" +
               $"Output file: {outputInfo.FullName}\n" +
               $"Compressed size: {FormatBytes(outputInfo.Length)}\n" +
               $"Compression ratio: {ratio:P2}\n" +
               $"Compression time: {stopwatch.Elapsed.TotalMilliseconds:N2} ms";
    }

    private static void CountDirectory(DirectoryInfo directory, ref int fileCount, ref int directoryCount, ref long totalBytes)
    {
        IEnumerable<FileInfo> files;
        try
        {
            files = directory.EnumerateFiles();
        }
        catch
        {
            return;
        }

        foreach (var file in files)
        {
            fileCount++;
            totalBytes += file.Length;
        }

        IEnumerable<DirectoryInfo> directories;
        try
        {
            directories = directory.EnumerateDirectories();
        }
        catch
        {
            return;
        }

        foreach (var child in directories)
        {
            directoryCount++;
            CountDirectory(child, ref fileCount, ref directoryCount, ref totalBytes);
        }
    }

    private static string FormatBytes(long bytes)
    {
        string[] units = ["B", "KB", "MB", "GB", "TB"];
        double value = bytes;
        int unitIndex = 0;

        while (value >= 1024 && unitIndex < units.Length - 1)
        {
            value /= 1024;
            unitIndex++;
        }

        return $"{value:N2} {units[unitIndex]}";
    }

    private static string FormatDuplicateReport(string title, string path, List<DuplicateEntryView> duplicates, TimeSpan elapsed)
    {
        var report = new StringBuilder();
        report.AppendLine(title);
        report.AppendLine($"Path: {path}");
        report.AppendLine($"Scan time: {elapsed.TotalMilliseconds:N2} ms");

        if (duplicates.Count == 0)
        {
            report.AppendLine("No duplicate groups found.");
            return report.ToString();
        }

        var groups = duplicates
            .GroupBy(item => item.GroupId)
            .OrderBy(group => group.Key)
            .ToList();

        report.AppendLine($"Duplicate groups: {groups.Count}");
        report.AppendLine($"Files in duplicate groups: {duplicates.Count}");

        foreach (var group in groups)
        {
            report.AppendLine();
            report.AppendLine($"Group {group.Key}");

            foreach (var item in group)
            {
                report.AppendLine($"{item.Name} | {item.DisplaySize} | {item.Path}");
            }
        }

        return report.ToString();
    }
}
