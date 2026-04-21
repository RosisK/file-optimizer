using System.Runtime.InteropServices;
using System.Text;

namespace FileOptimizer.UI;

internal static class AppConsole
{
    private enum TraceMode
    {
        Minimal,
        Detailed
    }

    private enum TraceDetail
    {
        Normal,
        Detailed
    }

    private const string TraceModeEnvironmentVariable = "FILEOPT_TRACE_MODE";
    private static readonly object Sync = new();
    private static TraceMode currentMode = TraceMode.Minimal;
    private static bool initialized;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool SetConsoleTitle(string lpConsoleTitle);

    public static void Initialize(string[] args)
    {
        if (initialized)
        {
            return;
        }

        if (GetConsoleWindow() == IntPtr.Zero)
        {
            AllocConsole();
        }

        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        RebindStandardStreams();
        SetConsoleTitle("File Optimizer Trace Console");
        currentMode = ResolveMode(args);
        Environment.SetEnvironmentVariable(
            TraceModeEnvironmentVariable,
            currentMode == TraceMode.Detailed ? "detailed" : "minimal",
            EnvironmentVariableTarget.Process);

        initialized = true;
        WriteSeparator();
        Log("App", $"Trace mode: {currentMode}. Run with --trace detailed for algorithm-level logs.");
        WriteSeparator();
    }

    public static void Log(string source, string message)
    {
        Log(source, message, TraceDetail.Normal);
    }

    public static void LogDetailed(string source, string message)
    {
        Log(source, message, TraceDetail.Detailed);
    }

    private static void Log(string source, string message, TraceDetail detail)
    {
        if (!initialized)
        {
            return;
        }

        if (detail == TraceDetail.Detailed && currentMode != TraceMode.Detailed)
        {
            return;
        }

        lock (Sync)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = GetColor(source);
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} | {source,-10} | {message}");
            Console.ForegroundColor = previousColor;
        }
    }

    private static void RebindStandardStreams()
    {
        var stdout = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
        var stderr = new StreamWriter(Console.OpenStandardError()) { AutoFlush = true };

        Console.SetOut(TextWriter.Synchronized(stdout));
        Console.SetError(TextWriter.Synchronized(stderr));
    }

    private static TraceMode ResolveMode(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (!args[i].Equals("--trace", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (i + 1 < args.Length && args[i + 1].Equals("detailed", StringComparison.OrdinalIgnoreCase))
            {
                return TraceMode.Detailed;
            }

            if (i + 1 < args.Length && args[i + 1].Equals("minimal", StringComparison.OrdinalIgnoreCase))
            {
                return TraceMode.Minimal;
            }
        }

        return TraceMode.Minimal;
    }

    private static ConsoleColor GetColor(string source)
    {
        return source switch
        {
            "App" => ConsoleColor.Cyan,
            "UI" => ConsoleColor.Green,
            "Analysis" => ConsoleColor.Yellow,
            _ => ConsoleColor.DarkGray
        };
    }

    private static void WriteSeparator()
    {
        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(new string('-', 88));
        Console.ForegroundColor = previousColor;
    }
}
