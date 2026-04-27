using System.Diagnostics;
using System.Text;

namespace FileOptimizer.UI;

internal static class AlgorithmLabService
{
    public static string RunSortAlgorithmStudy(string path, SortOption sortBy, int iterations)
    {
        var source = LoadDataset(path);
        if (source.Count < 2)
        {
            throw new InvalidOperationException("Choose a folder with at least two items for sort analysis.");
        }

        var comparison = CreateComparison(sortBy);
        var algorithms = new[]
        {
            new SortAlgorithmDefinition("Bubble Sort", "O(n^2)", "Very simple, but slow for larger sets.", BubbleSort),
            new SortAlgorithmDefinition("Insertion Sort", "O(n^2)", "Good on tiny or nearly sorted inputs.", InsertionSort),
            new SortAlgorithmDefinition("Merge Sort", "O(n log n)", "Stable and predictable, but uses extra memory.", MergeSort),
            new SortAlgorithmDefinition("StdSort (.NET)", "O(n log n)", "Fast general-purpose library sort.", StdSort)
        };

        var results = algorithms
            .Select(definition => BenchmarkSortAlgorithm(definition, source, comparison, iterations))
            .OrderBy(result => result.AverageTimeMs)
            .ToList();

        var fastest = results[0];
        var lowestAllocation = results.MinBy(result => result.AverageAllocatedBytes)!;

        var report = new StringBuilder();
        AnalysisReportFormatter.AppendTitle(report, "Sort Algorithm Lab");
        AnalysisReportFormatter.AppendKeyValueTable(report,
        [
            ("Path", path),
            ("Dataset size", $"{source.Count} item(s)"),
            ("Sort by", sortBy.ToString()),
            ("Iterations", iterations.ToString()),
            ("Input policy", "Each algorithm receives the same shuffled dataset per iteration.")
        ]);

        AnalysisReportFormatter.AppendTable(
            report,
            ["Algorithm", "Avg Time", "Avg Alloc", "Complexity", "Trade-off"],
            results.Select(result => new[]
            {
                result.Name,
                $"{result.AverageTimeMs:N3} ms",
                FormatBytes(result.AverageAllocatedBytes),
                result.Complexity,
                result.TradeOff
            }));

        AnalysisReportFormatter.AppendNoteBlock(report, "Highlights",
        [
            $"Fastest: {fastest.Name}",
            $"Lowest extra allocation: {lowestAllocation.Name}",
            "Quadratic algorithms are useful to demonstrate scaling behavior, while merge/library sorts show what production-ready approaches gain in speed."
        ]);

        AppConsole.Log("Analysis", $"Sort lab {sortBy} | fastest={fastest.Name}, dataset={source.Count}, iterations={iterations}");
        return report.ToString();
    }

    public static string RunSearchAlgorithmStudy(string path, string query, int iterations)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new InvalidOperationException("Enter a search query first.");
        }

        var source = LoadDataset(path);
        if (source.Count == 0)
        {
            throw new InvalidOperationException("Choose a folder with at least one item for search analysis.");
        }

        var strategies = new[]
        {
            MeasureLinearTokenSearch(source, query, iterations),
            MeasurePretokenizedLinearSearch(source, query, iterations),
            MeasureInvertedIndexSearch(source, query, iterations),
            MeasureNativeCoreSearch(path, query, iterations)
        }
        .OrderBy(result => result.AverageQueryMs)
        .ToList();

        var fastest = strategies[0];
        var lightestPrep = strategies.MinBy(result => result.BuildTimeMs)!;

        var report = new StringBuilder();
        AnalysisReportFormatter.AppendTitle(report, "Search Algorithm Lab");
        AnalysisReportFormatter.AppendKeyValueTable(report,
        [
            ("Path", path),
            ("Dataset size", $"{source.Count} item(s)"),
            ("Query", query),
            ("Iterations", iterations.ToString()),
            ("Search semantics", "Lowercase token matching with partial-token support.")
        ]);

        AnalysisReportFormatter.AppendTable(
            report,
            ["Algorithm", "Prep", "Avg Query", "Avg Alloc", "Results", "Trade-off"],
            strategies.Select(result => new[]
            {
                result.Name,
                $"{result.BuildTimeMs:N3} ms",
                $"{result.AverageQueryMs:N3} ms",
                FormatBytes(result.AverageAllocatedBytes),
                result.ResultCount.ToString(),
                result.TradeOff
            }));

        AnalysisReportFormatter.AppendNoteBlock(report, "Highlights",
        [
            $"Fastest repeated-query path: {fastest.Name}",
            $"Lowest preprocessing cost: {lightestPrep.Name}",
            "Linear scans are simple and cheap to set up, while token caches and inverted indexes pay more upfront to answer repeated queries faster."
        ]);

        AppConsole.Log("Analysis", $"Search lab \"{query}\" | fastest={fastest.Name}, dataset={source.Count}, iterations={iterations}");
        return report.ToString();
    }

    private static List<AnalysisFileRecord> LoadDataset(string path)
    {
        return NativeMethods
            .GetDirectoryContents(path, new NativeSortOptions(SortOption.Name, true, false))
            .Select(item => new AnalysisFileRecord(item.Name, item.Path, item.Size, item.Modified, item.IsDirectory))
            .ToList();
    }

    private static Comparison<AnalysisFileRecord> CreateComparison(SortOption sortBy)
    {
        return (left, right) =>
        {
            if (left.IsDirectory != right.IsDirectory)
            {
                return left.IsDirectory ? -1 : 1;
            }

            var comparison = sortBy switch
            {
                SortOption.Size => left.Size.CompareTo(right.Size),
                SortOption.ModifiedTime => left.Modified.CompareTo(right.Modified),
                _ => string.Compare(left.Name, right.Name, StringComparison.OrdinalIgnoreCase)
            };

            return comparison != 0
                ? comparison
                : string.Compare(left.Name, right.Name, StringComparison.OrdinalIgnoreCase);
        };
    }

    private static SortAlgorithmResult BenchmarkSortAlgorithm(
        SortAlgorithmDefinition definition,
        IReadOnlyList<AnalysisFileRecord> source,
        Comparison<AnalysisFileRecord> comparison,
        int iterations)
    {
        double totalTimeMs = 0;
        long totalAllocatedBytes = 0;

        for (int iteration = 0; iteration < iterations; iteration++)
        {
            var working = CreateShuffledCopy(source, 1000 + iteration);
            var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
            var stopwatch = Stopwatch.StartNew();
            definition.Sort(working, comparison);
            stopwatch.Stop();

            totalTimeMs += stopwatch.Elapsed.TotalMilliseconds;
            totalAllocatedBytes += GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;

            if (!IsSorted(working, comparison))
            {
                throw new InvalidOperationException($"{definition.Name} did not sort the dataset correctly.");
            }
        }

        return new SortAlgorithmResult(
            definition.Name,
            definition.Complexity,
            definition.TradeOff,
            totalTimeMs / iterations,
            totalAllocatedBytes / iterations);
    }

    private static SearchAlgorithmResult MeasureLinearTokenSearch(
        IReadOnlyList<AnalysisFileRecord> source,
        string query,
        int iterations)
    {
        var queryTokens = Tokenize(query);
        double totalQueryMs = 0;
        long totalAllocatedBytes = 0;
        int resultCount = 0;

        for (int iteration = 0; iteration < iterations; iteration++)
        {
            var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
            var stopwatch = Stopwatch.StartNew();
            resultCount = source.Count(item => MatchesQuery(queryTokens, Tokenize(item.Name)));
            stopwatch.Stop();

            totalQueryMs += stopwatch.Elapsed.TotalMilliseconds;
            totalAllocatedBytes += GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;
        }

        return new SearchAlgorithmResult(
            "Linear retokenize",
            0,
            totalQueryMs / iterations,
            totalAllocatedBytes / iterations,
            resultCount,
            "No setup cost, but tokenizes every name for every query.");
    }

    private static SearchAlgorithmResult MeasurePretokenizedLinearSearch(
        IReadOnlyList<AnalysisFileRecord> source,
        string query,
        int iterations)
    {
        var buildStopwatch = Stopwatch.StartNew();
        var tokenized = source
            .Select(item => new TokenizedRecord(item, Tokenize(item.Name)))
            .ToList();
        buildStopwatch.Stop();

        var queryTokens = Tokenize(query);
        double totalQueryMs = 0;
        long totalAllocatedBytes = 0;
        int resultCount = 0;

        for (int iteration = 0; iteration < iterations; iteration++)
        {
            var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
            var stopwatch = Stopwatch.StartNew();
            resultCount = tokenized.Count(item => MatchesQuery(queryTokens, item.Tokens));
            stopwatch.Stop();

            totalQueryMs += stopwatch.Elapsed.TotalMilliseconds;
            totalAllocatedBytes += GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;
        }

        return new SearchAlgorithmResult(
            "Pretokenized linear",
            buildStopwatch.Elapsed.TotalMilliseconds,
            totalQueryMs / iterations,
            totalAllocatedBytes / iterations,
            resultCount,
            "Pays a one-time tokenization cost, then scans lightweight token arrays.");
    }

    private static SearchAlgorithmResult MeasureInvertedIndexSearch(
        IReadOnlyList<AnalysisFileRecord> source,
        string query,
        int iterations)
    {
        var buildStopwatch = Stopwatch.StartNew();
        var index = BuildInvertedIndex(source);
        buildStopwatch.Stop();

        var queryTokens = Tokenize(query);
        double totalQueryMs = 0;
        long totalAllocatedBytes = 0;
        int resultCount = 0;

        for (int iteration = 0; iteration < iterations; iteration++)
        {
            var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
            var stopwatch = Stopwatch.StartNew();
            resultCount = SearchInvertedIndex(index, queryTokens);
            stopwatch.Stop();

            totalQueryMs += stopwatch.Elapsed.TotalMilliseconds;
            totalAllocatedBytes += GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;
        }

        return new SearchAlgorithmResult(
            "Inverted index",
            buildStopwatch.Elapsed.TotalMilliseconds,
            totalQueryMs / iterations,
            totalAllocatedBytes / iterations,
            resultCount,
            "Best for repeated queries, but uses extra index memory.");
    }

    private static SearchAlgorithmResult MeasureNativeCoreSearch(string path, string query, int iterations)
    {
        var sort = new NativeSortOptions(SortOption.Name, true, true);
        double totalQueryMs = 0;
        long totalAllocatedBytes = 0;
        int resultCount = 0;

        for (int iteration = 0; iteration < iterations; iteration++)
        {
            var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
            var stopwatch = Stopwatch.StartNew();
            resultCount = NativeMethods.SearchDirectoryContents(path, query, sort).Count;
            stopwatch.Stop();

            totalQueryMs += stopwatch.Elapsed.TotalMilliseconds;
            totalAllocatedBytes += GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;
        }

        return new SearchAlgorithmResult(
            "Native core path",
            0,
            totalQueryMs / iterations,
            totalAllocatedBytes / iterations,
            resultCount,
            "Uses the real C++ bridge, so it reflects the current product path end to end.");
    }

    private static List<AnalysisFileRecord> CreateShuffledCopy(IReadOnlyList<AnalysisFileRecord> source, int seed)
    {
        var copy = source.ToList();
        var random = new Random(seed);

        for (int index = copy.Count - 1; index > 0; index--)
        {
            var swapIndex = random.Next(index + 1);
            (copy[index], copy[swapIndex]) = (copy[swapIndex], copy[index]);
        }

        return copy;
    }

    private static bool IsSorted(IReadOnlyList<AnalysisFileRecord> items, Comparison<AnalysisFileRecord> comparison)
    {
        for (int index = 1; index < items.Count; index++)
        {
            if (comparison(items[index - 1], items[index]) > 0)
            {
                return false;
            }
        }

        return true;
    }

    private static void BubbleSort(List<AnalysisFileRecord> items, Comparison<AnalysisFileRecord> comparison)
    {
        for (int outer = 0; outer < items.Count - 1; outer++)
        {
            var swapped = false;

            for (int inner = 0; inner < items.Count - outer - 1; inner++)
            {
                if (comparison(items[inner], items[inner + 1]) <= 0)
                {
                    continue;
                }

                (items[inner], items[inner + 1]) = (items[inner + 1], items[inner]);
                swapped = true;
            }

            if (!swapped)
            {
                break;
            }
        }
    }

    private static void InsertionSort(List<AnalysisFileRecord> items, Comparison<AnalysisFileRecord> comparison)
    {
        for (int index = 1; index < items.Count; index++)
        {
            var current = items[index];
            var insertionIndex = index - 1;

            while (insertionIndex >= 0 && comparison(items[insertionIndex], current) > 0)
            {
                items[insertionIndex + 1] = items[insertionIndex];
                insertionIndex--;
            }

            items[insertionIndex + 1] = current;
        }
    }

    private static void MergeSort(List<AnalysisFileRecord> items, Comparison<AnalysisFileRecord> comparison)
    {
        var buffer = new AnalysisFileRecord[items.Count];
        MergeSort(items, buffer, 0, items.Count - 1, comparison);
    }

    private static void MergeSort(
        List<AnalysisFileRecord> items,
        AnalysisFileRecord[] buffer,
        int left,
        int right,
        Comparison<AnalysisFileRecord> comparison)
    {
        if (left >= right)
        {
            return;
        }

        var middle = left + ((right - left) / 2);
        MergeSort(items, buffer, left, middle, comparison);
        MergeSort(items, buffer, middle + 1, right, comparison);
        Merge(items, buffer, left, middle, right, comparison);
    }

    private static void Merge(
        List<AnalysisFileRecord> items,
        AnalysisFileRecord[] buffer,
        int left,
        int middle,
        int right,
        Comparison<AnalysisFileRecord> comparison)
    {
        int leftIndex = left;
        int rightIndex = middle + 1;
        int bufferIndex = left;

        while (leftIndex <= middle && rightIndex <= right)
        {
            if (comparison(items[leftIndex], items[rightIndex]) <= 0)
            {
                buffer[bufferIndex++] = items[leftIndex++];
            }
            else
            {
                buffer[bufferIndex++] = items[rightIndex++];
            }
        }

        while (leftIndex <= middle)
        {
            buffer[bufferIndex++] = items[leftIndex++];
        }

        while (rightIndex <= right)
        {
            buffer[bufferIndex++] = items[rightIndex++];
        }

        for (int index = left; index <= right; index++)
        {
            items[index] = buffer[index];
        }
    }

    private static void StdSort(List<AnalysisFileRecord> items, Comparison<AnalysisFileRecord> comparison)
    {
        items.Sort(comparison);
    }

    private static Dictionary<string, HashSet<string>> BuildInvertedIndex(IReadOnlyList<AnalysisFileRecord> source)
    {
        var index = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in source)
        {
            foreach (var token in Tokenize(item.Name))
            {
                if (!index.TryGetValue(token, out var entries))
                {
                    entries = [];
                    index[token] = entries;
                }

                entries.Add(item.Path);
            }
        }

        return index;
    }

    private static int SearchInvertedIndex(Dictionary<string, HashSet<string>> index, IReadOnlyList<string> queryTokens)
    {
        var matches = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var queryToken in queryTokens)
        {
            if (index.TryGetValue(queryToken, out var exactMatches))
            {
                matches.UnionWith(exactMatches);
            }

            foreach (var (token, tokenMatches) in index)
            {
                if (token.Equals(queryToken, StringComparison.OrdinalIgnoreCase) ||
                    !token.Contains(queryToken, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                matches.UnionWith(tokenMatches);
            }
        }

        return matches.Count;
    }

    private static bool MatchesQuery(IReadOnlyList<string> queryTokens, IReadOnlyList<string> fileTokens)
    {
        foreach (var queryToken in queryTokens)
        {
            foreach (var fileToken in fileTokens)
            {
                if (fileToken.Equals(queryToken, StringComparison.OrdinalIgnoreCase) ||
                    fileToken.Contains(queryToken, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static string[] Tokenize(string text)
    {
        return text
            .ToLowerInvariant()
            .Replace('_', ' ')
            .Replace('-', ' ')
            .Replace('.', ' ')
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private static string FormatBytes(long bytes)
    {
        if (bytes < 1024)
        {
            return $"{bytes} B";
        }

        if (bytes < 1024 * 1024)
        {
            return $"{bytes / 1024d:N1} KB";
        }

        return $"{bytes / 1024d / 1024d:N1} MB";
    }

    private readonly record struct AnalysisFileRecord(string Name, string Path, ulong Size, DateTime Modified, bool IsDirectory);
    private readonly record struct TokenizedRecord(AnalysisFileRecord Item, string[] Tokens);
    private readonly record struct SortAlgorithmDefinition(
        string Name,
        string Complexity,
        string TradeOff,
        Action<List<AnalysisFileRecord>, Comparison<AnalysisFileRecord>> Sort);
    private readonly record struct SortAlgorithmResult(
        string Name,
        string Complexity,
        string TradeOff,
        double AverageTimeMs,
        long AverageAllocatedBytes);
    private readonly record struct SearchAlgorithmResult(
        string Name,
        double BuildTimeMs,
        double AverageQueryMs,
        long AverageAllocatedBytes,
        int ResultCount,
        string TradeOff);
}
