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
        AnalysisReportFormatter.AppendTitle(report, "Storage Summary");
        AnalysisReportFormatter.AppendKeyValueTable(report,
        [
            ("Path", path),
            ("Folders", $"{directoryCount:N0}"),
            ("Files", $"{fileCount:N0}"),
            ("Total size", FormatBytes(totalBytes)),
            ("Scan time", $"{stopwatch.Elapsed.TotalMilliseconds:N2} ms")
        ]);
        AppConsole.Log("Analysis", $"Storage summary | files={fileCount}, folders={directoryCount}, size={FormatBytes(totalBytes)}, time={stopwatch.Elapsed.TotalMilliseconds:N2} ms");
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
        AnalysisReportFormatter.AppendTitle(report, "Search Benchmark");
        AnalysisReportFormatter.AppendKeyValueTable(report,
        [
            ("Path", path),
            ("Query", query),
            ("Iterations", iterations.ToString()),
            ("Last result count", resultCount.ToString()),
            ("Total time", $"{stopwatch.Elapsed.TotalMilliseconds:N2} ms"),
            ("Average time", $"{avgMs:N2} ms")
        ]);
        AppConsole.Log("Analysis", $"Search benchmark \"{query}\" | results={resultCount}, iterations={iterations}, avg={avgMs:N2} ms");
        return report.ToString();
    }

    public static string RunSortBenchmark(string path, int iterations)
    {
        var report = new StringBuilder();
        AnalysisReportFormatter.AppendTitle(report, "Sort Benchmark");
        AnalysisReportFormatter.AppendKeyValueTable(report,
        [
            ("Path", path),
            ("Iterations per case", iterations.ToString())
        ]);

        var rows = new List<string[]>();

        foreach (var option in Enum.GetValues<SortOption>())
        {
            var stopwatch = Stopwatch.StartNew();
            int itemCount = 0;

            for (int i = 0; i < iterations; i++)
            {
                itemCount = NativeMethods.GetDirectoryContents(path, new NativeSortOptions(option, true, true)).Count;
            }

            stopwatch.Stop();
            rows.Add(
            [
                option.ToString(),
                itemCount.ToString(),
                $"{stopwatch.Elapsed.TotalMilliseconds / iterations:N2} ms"
            ]);
            AppConsole.Log("Analysis", $"Sort benchmark {option} | items={itemCount}, avg={stopwatch.Elapsed.TotalMilliseconds / iterations:N2} ms");
        }

        AnalysisReportFormatter.AppendTable(report, ["Sort Key", "Items", "Average"], rows);

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
            AnalysisReportFormatter.AppendTitle(report, "Compression Benchmark");
            AnalysisReportFormatter.AppendKeyValueTable(report,
            [
                ("Source file", source.FullName),
                ("Source size", FormatBytes(source.Length))
            ]);

            var zstd = RunSingleCompressionBenchmark(source.FullName, source.Length, CompressionFormat.Zstd, Path.Combine(tempRoot, $"{source.Name}.zst"));
            var zip = RunSingleCompressionBenchmark(source.FullName, source.Length, CompressionFormat.Zip, Path.Combine(tempRoot, $"{source.Name}.zip"));

            AnalysisReportFormatter.AppendTable(
                report,
                ["Format", "Output file", "Compressed size", "Ratio", "Time"],
                [zstd.ToRow(), zip.ToRow()]);

            AppConsole.Log("Analysis", $"Compression benchmark complete for '{source.Name}'.");
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
        AppConsole.Log("Analysis", $"Duplicate names | rows={duplicates.Count}, time={stopwatch.Elapsed.TotalMilliseconds:N2} ms");

        return FormatDuplicateReport("Duplicate Name Analysis", path, duplicates, stopwatch.Elapsed);
    }

    public static string RunDuplicateContentAnalysis(string path)
    {
        var stopwatch = Stopwatch.StartNew();
        var duplicates = NativeMethods.FindDuplicateContents(path);
        stopwatch.Stop();
        AppConsole.Log("Analysis", $"Duplicate contents | rows={duplicates.Count}, time={stopwatch.Elapsed.TotalMilliseconds:N2} ms");

        return FormatDuplicateReport("Duplicate Content Analysis", path, duplicates, stopwatch.Elapsed);
    }

    private static CompressionBenchmarkResult RunSingleCompressionBenchmark(string sourcePath, long sourceSize, CompressionFormat format, string outputPath)
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
        AppConsole.Log("Analysis", $"{format} benchmark | ratio={ratio:P2}, time={stopwatch.Elapsed.TotalMilliseconds:N2} ms");

        return new CompressionBenchmarkResult(
            format.ToString(),
            outputInfo.FullName,
            FormatBytes(outputInfo.Length),
            $"{ratio:P2}",
            $"{stopwatch.Elapsed.TotalMilliseconds:N2} ms");
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
        AnalysisReportFormatter.AppendTitle(report, title);

        if (duplicates.Count == 0)
        {
            AnalysisReportFormatter.AppendKeyValueTable(report,
            [
                ("Path", path),
                ("Scan time", $"{elapsed.TotalMilliseconds:N2} ms"),
                ("Status", "No duplicate groups found")
            ]);
            report.AppendLine("No duplicate groups found.");
            return report.ToString();
        }

        var groups = duplicates
            .GroupBy(item => item.GroupId)
            .OrderBy(group => group.Key)
            .ToList();

        AnalysisReportFormatter.AppendKeyValueTable(report,
        [
            ("Path", path),
            ("Scan time", $"{elapsed.TotalMilliseconds:N2} ms"),
            ("Duplicate groups", groups.Count.ToString()),
            ("Files in duplicate groups", duplicates.Count.ToString())
        ]);

        AnalysisReportFormatter.AppendTable(
            report,
            ["Group", "Name", "Size", "Path"],
            groups.SelectMany(group => group.Select(item => new[]
            {
                group.Key.ToString(),
                item.Name,
                item.DisplaySize,
                item.Path
            })));

        return report.ToString();
    }

    private readonly record struct CompressionBenchmarkResult(
        string Format,
        string OutputFile,
        string CompressedSize,
        string Ratio,
        string Time)
    {
        public string[] ToRow() => [Format, OutputFile, CompressedSize, Ratio, Time];
    }
}
