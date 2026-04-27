using System.Text;

namespace FileOptimizer.UI;

internal static class AnalysisReportFormatter
{
    public static void AppendTitle(StringBuilder builder, string title)
    {
        builder.AppendLine(title);
        builder.AppendLine(new string('=', title.Length));
        builder.AppendLine();
    }

    public static void AppendTable(StringBuilder builder, string[] headers, IEnumerable<string[]> rows)
    {
        var materializedRows = rows.ToList();
        var widths = new int[headers.Length];

        for (int index = 0; index < headers.Length; index++)
        {
            widths[index] = headers[index].Length;
        }

        foreach (var row in materializedRows)
        {
            for (int index = 0; index < headers.Length && index < row.Length; index++)
            {
                widths[index] = Math.Max(widths[index], row[index].Length);
            }
        }

        builder.AppendLine(Border(widths));
        builder.AppendLine(RenderRow(headers, widths));
        builder.AppendLine(Border(widths));

        foreach (var row in materializedRows)
        {
            builder.AppendLine(RenderRow(row, widths));
        }

        builder.AppendLine(Border(widths));
        builder.AppendLine();
    }

    public static void AppendKeyValueTable(StringBuilder builder, IEnumerable<(string Label, string Value)> rows)
    {
        AppendTable(
            builder,
            ["Metric", "Value"],
            rows.Select(row => new[] { row.Label, row.Value }));
    }

    public static void AppendNoteBlock(StringBuilder builder, string heading, IEnumerable<string> lines)
    {
        builder.AppendLine(heading);
        builder.AppendLine(new string('-', heading.Length));

        foreach (var line in lines)
        {
            builder.AppendLine($"- {line}");
        }

        builder.AppendLine();
    }

    private static string Border(IReadOnlyList<int> widths)
    {
        return "+" + string.Join("+", widths.Select(width => new string('-', width + 2))) + "+";
    }

    private static string RenderRow(IReadOnlyList<string> values, IReadOnlyList<int> widths)
    {
        var cells = widths
            .Select((width, index) => " " + (index < values.Count ? values[index] : string.Empty).PadRight(width) + " ")
            .ToArray();

        return "|" + string.Join("|", cells) + "|";
    }
}
