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

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct NativeDuplicateEntry
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string Name;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        public string Path;

        public ulong Size;
        public int GroupId;
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

    [DllImport(DllName, EntryPoint = "Core_CopyPath", CharSet = CharSet.Unicode)]
    private static extern int CoreCopyPath(string sourcePath, string destinationPath);

    [DllImport(DllName, EntryPoint = "Core_DeletePath", CharSet = CharSet.Unicode)]
    private static extern int CoreDeletePath(string path);

    [DllImport(DllName, EntryPoint = "Core_RenamePath", CharSet = CharSet.Unicode)]
    private static extern int CoreRenamePath(string sourcePath, string destinationPath);

    [DllImport(DllName, EntryPoint = "Core_MovePath", CharSet = CharSet.Unicode)]
    private static extern int CoreMovePath(string sourcePath, string destinationPath);

    [DllImport(DllName, EntryPoint = "Core_CreateEmptyFile", CharSet = CharSet.Unicode)]
    private static extern int CoreCreateEmptyFile(string path);

    [DllImport(DllName, EntryPoint = "Core_CreateDirectory", CharSet = CharSet.Unicode)]
    private static extern int CoreCreateDirectory(string path);

    [DllImport(DllName, EntryPoint = "Core_FindDuplicateNames", CharSet = CharSet.Unicode)]
    private static extern int CoreFindDuplicateNames(string rootPath, [Out] NativeDuplicateEntry[] items, int maxItems);

    [DllImport(DllName, EntryPoint = "Core_FindDuplicateContents", CharSet = CharSet.Unicode)]
    private static extern int CoreFindDuplicateContents(string rootPath, [Out] NativeDuplicateEntry[] items, int maxItems);

    [DllImport(DllName, EntryPoint = "Core_CompressFile", CharSet = CharSet.Unicode)]
    private static extern int CoreCompressFile(string sourcePath, string destinationPath);

    [DllImport(DllName, EntryPoint = "Core_DecompressFile", CharSet = CharSet.Unicode)]
    private static extern int CoreDecompressFile(string sourcePath, string destinationPath);

    [DllImport(DllName, EntryPoint = "Core_CompressPath", CharSet = CharSet.Unicode)]
    private static extern int CoreCompressPath(string sourcePath, string destinationPath, int format);

    [DllImport(DllName, EntryPoint = "Core_DecompressPath", CharSet = CharSet.Unicode)]
    private static extern int CoreDecompressPath(string sourcePath, string destinationPath, int format);

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

    public static void CopyPath(string sourcePath, string destinationPath)
    {
        ExecuteBooleanCall(() => CoreCopyPath(sourcePath, destinationPath), "Copy failed.");
    }

    public static void DeletePath(string path)
    {
        ExecuteBooleanCall(() => CoreDeletePath(path), "Delete failed.");
    }

    public static void RenamePath(string sourcePath, string destinationPath)
    {
        ExecuteBooleanCall(() => CoreRenamePath(sourcePath, destinationPath), "Rename failed.");
    }

    public static void MovePath(string sourcePath, string destinationPath)
    {
        ExecuteBooleanCall(() => CoreMovePath(sourcePath, destinationPath), "Move failed.");
    }

    public static void CreateEmptyFile(string path)
    {
        ExecuteBooleanCall(() => CoreCreateEmptyFile(path), "File creation failed.");
    }

    public static void CreateDirectory(string path)
    {
        ExecuteBooleanCall(() => CoreCreateDirectory(path), "Folder creation failed.");
    }

    public static List<DuplicateEntryView> FindDuplicateNames(string rootPath)
    {
        return ExecuteDuplicateCall((items, maxItems) => CoreFindDuplicateNames(rootPath, items, maxItems));
    }

    public static List<DuplicateEntryView> FindDuplicateContents(string rootPath)
    {
        return ExecuteDuplicateCall((items, maxItems) => CoreFindDuplicateContents(rootPath, items, maxItems));
    }

    public static void CompressFile(string sourcePath, string destinationPath)
    {
        ExecuteBooleanCall(() => CoreCompressFile(sourcePath, destinationPath), "Compression failed.");
    }

    public static void DecompressFile(string sourcePath, string destinationPath)
    {
        ExecuteBooleanCall(() => CoreDecompressFile(sourcePath, destinationPath), "Decompression failed.");
    }

    public static void CompressPath(string sourcePath, string destinationPath, CompressionFormat format)
    {
        ExecuteBooleanCall(() => CoreCompressPath(sourcePath, destinationPath, (int)format), "Compression failed.");
    }

    public static void DecompressPath(string sourcePath, string destinationPath, CompressionFormat format)
    {
        ExecuteBooleanCall(() => CoreDecompressPath(sourcePath, destinationPath, (int)format), "Decompression failed.");
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

    private static List<DuplicateEntryView> ExecuteDuplicateCall(Func<NativeDuplicateEntry[], int, int> nativeCall)
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

        var items = new NativeDuplicateEntry[count];
        var filled = nativeCall(items, items.Length);
        if (filled < 0)
        {
            throw new InvalidOperationException(GetLastErrorMessage());
        }

        return items
            .Take(filled)
            .Select(item => new DuplicateEntryView(item.GroupId, item.Name, item.Path, item.Size))
            .ToList();
    }
}

internal enum SortOption
{
    Name = 0,
    Size = 1,
    ModifiedTime = 2
}

internal enum CompressionFormat
{
    Zstd = 0,
    Zip = 1
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

internal sealed class DuplicateEntryView
{
    public DuplicateEntryView(int groupId, string name, string path, ulong size)
    {
        GroupId = groupId;
        Name = name;
        Path = path;
        Size = size;
    }

    public int GroupId { get; }
    public string Name { get; }
    public string Path { get; }
    public ulong Size { get; }
    public string DisplaySize => $"{Size:N0} bytes";
}
