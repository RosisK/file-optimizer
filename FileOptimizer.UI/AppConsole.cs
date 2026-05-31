using System.Runtime.InteropServices;
using System.Text;

namespace FileOptimizer.UI;

internal static class AppConsole
{
    private static readonly object Sync = new();
    private static bool initialized;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool SetConsoleTitle(string lpConsoleTitle);

    public static void Initialize()
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
        SetConsoleTitle("File Pilot Trace Console");

        initialized = true;
        Log("App", "Trace console initialized. GUI behavior is unchanged; detailed operation logs will appear here.");
    }

    public static void Log(string source, string message)
    {
        if (!initialized)
        {
            return;
        }

        lock (Sync)
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} | {source,-10} | {message}");
        }
    }

    private static void RebindStandardStreams()
    {
        var stdout = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
        var stderr = new StreamWriter(Console.OpenStandardError()) { AutoFlush = true };

        Console.SetOut(TextWriter.Synchronized(stdout));
        Console.SetError(TextWriter.Synchronized(stderr));
    }
}
