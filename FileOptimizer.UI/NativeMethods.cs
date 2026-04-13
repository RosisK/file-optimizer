using System.Runtime.InteropServices;
using System.Text;

namespace FileOptimizer.UI;

internal static class NativeMethods
{
    private const string DllName = "CoreEngine.dll";

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct NativeFileInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string Name;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        public string Path;

        public ulong Size;
        public long ModifiedTime;
        public int IsDirectory;
    }

    [DllImport(DllName, EntryPoint = "Core_GetDirectoryContents", CharSet = CharSet.Unicode)]
    private static extern int CoreGetDirectoryContents(
        string path,
        [Out] NativeFileInfo[] items,
        int maxItems,
        int sortKey,
        int ascending,
        int directoriesFirst);

    [DllImport(DllName, EntryPoint = "Core_SearchDirectoryContents", CharSet = CharSet.Unicode)]
    private static extern int CoreSearchDirectoryContents(
        string path,
        string query,
        [Out] NativeFileInfo[] items,
        int maxItems,
        int sortKey,
        int ascending,
        int directoriesFirst);

    [DllImport(DllName, EntryPoint = "Core_CopyFile", CharSet = CharSet.Unicode)]
    private static extern int CoreCopyFile(string sourcePath, string destinationPath);

    [DllImport(DllName, EntryPoint = "Core_DeleteFile", CharSet = CharSet.Unicode)]
    private static extern int CoreDeleteFile(string path);

    [DllImport(DllName, EntryPoint = "Core_CompressFile", CharSet = CharSet.Unicode)]
    private static extern int CoreCompressFile(string sourcePath, string destinationPath);

    [DllImport(DllName, EntryPoint = "Core_DecompressFile", CharSet = CharSet.Unicode)]
    private static extern int CoreDecompressFile(string sourcePath, string destinationPath);

    [DllImport(DllName, EntryPoint = "Core_GetLastErrorMessage", CharSet = CharSet.Unicode)]
    private static extern int CoreGetLastErrorMessage(StringBuilder buffer, int bufferLength);

    public static List<FileItemView> GetDirectoryContents(string path, NativeSortOptions sort)
    {
        return ExecuteListCall((items, maxItems) =>
            CoreGetDirectoryContents(path, items, maxItems, (int)sort.SortBy, sort.Ascending ? 1 : 0, sort.DirectoriesFirst ? 1 : 0));
    }

    public static List<FileItemView> SearchDirectoryContents(string path, string query, NativeSortOptions sort)
    {
        return ExecuteListCall((items, maxItems) =>
            CoreSearchDirectoryContents(path, query, items, maxItems, (int)sort.SortBy, sort.Ascending ? 1 : 0, sort.DirectoriesFirst ? 1 : 0));
    }

    public static void CopyFile(string sourcePath, string destinationPath)
    {
        ExecuteBooleanCall(() => CoreCopyFile(sourcePath, destinationPath), "Copy failed.");
    }

    public static void DeleteFile(string path)
    {
        ExecuteBooleanCall(() => CoreDeleteFile(path), "Delete failed.");
    }

    public static void CompressFile(string sourcePath, string destinationPath)
    {
        ExecuteBooleanCall(() => CoreCompressFile(sourcePath, destinationPath), "Compression failed.");
    }

    public static void DecompressFile(string sourcePath, string destinationPath)
    {
        ExecuteBooleanCall(() => CoreDecompressFile(sourcePath, destinationPath), "Decompression failed.");
    }

    private static List<FileItemView> ExecuteListCall(Func<NativeFileInfo[], int, int> nativeCall)
    {
        var count = nativeCall([], 0);
        if (count < 0)
        {
            throw new InvalidOperationException(GetLastErrorMessage());
        }

        if (count == 0)
        {
            return [];
        }

        var items = new NativeFileInfo[count];
        var filled = nativeCall(items, items.Length);
        if (filled < 0)
        {
            throw new InvalidOperationException(GetLastErrorMessage());
        }

        return items
            .Take(filled)
            .Select(item => new FileItemView(
                item.Name,
                item.Path,
                item.Size,
                DateTimeOffset.FromUnixTimeSeconds(item.ModifiedTime).LocalDateTime,
                item.IsDirectory != 0))
            .ToList();
    }

    private static void ExecuteBooleanCall(Func<int> nativeCall, string fallbackMessage)
    {
        if (nativeCall() == 0)
        {
            throw new InvalidOperationException(GetLastErrorMessage(fallbackMessage));
        }
    }

    private static string GetLastErrorMessage(string fallbackMessage = "The native operation failed.")
    {
        var buffer = new StringBuilder(1024);
        var written = CoreGetLastErrorMessage(buffer, buffer.Capacity);
        return written > 0 ? buffer.ToString() : fallbackMessage;
    }
}

internal enum SortOption
{
    Name = 0,
    Size = 1,
    ModifiedTime = 2
}

internal readonly record struct NativeSortOptions(SortOption SortBy, bool Ascending, bool DirectoriesFirst);

internal sealed class FileItemView
{
    public FileItemView(string name, string path, ulong size, DateTime modified, bool isDirectory)
    {
        Name = name;
        Path = path;
        Size = size;
        Modified = modified;
        IsDirectory = isDirectory;
    }

    public string Name { get; }
    public string Path { get; }
    public ulong Size { get; }
    public DateTime Modified { get; }
    public bool IsDirectory { get; }
    public string Type => IsDirectory ? "Folder" : "File";
    public string DisplaySize => IsDirectory ? "-" : $"{Size:N0} bytes";
}
