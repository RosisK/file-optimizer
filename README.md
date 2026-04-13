# file-optimizer

This repository now uses a simple native-plus-managed desktop architecture:

- `C:\dev\Semester_Project\File-Optimizer`
  This is your existing C++ core/test project.
- `C:\dev\Semester_Project\CoreEngine`
  This is a native C++ DLL that exports a small C-style API.
- `C:\dev\Semester_Project\FileOptimizer.UI`
  This is a C# WinForms desktop app that calls `CoreEngine.dll` with P/Invoke.

## How the pieces connect

The UI does not call C++ classes directly.

Instead, the flow is:

1. C# calls exported functions like `Core_GetDirectoryContents(...)`
2. `CoreEngine.dll` converts that call into your existing C++ classes like `FileService`, `SearchIndex`, and `FileSorter`
3. The DLL returns plain structs that C# can marshal safely

That is a very normal way to mix C# and C++ in a Windows desktop app.

## Current starter features

- Browse a directory
- Go up to parent folder
- Search within the current directory
- Sort by name, size, or modified time
- Copy a selected file
- Delete a selected file
- Compress and decompress a selected file with the existing zstd-based core

## Build order

Build these in this order:

1. `CoreEngine` as `Debug|x64` or `Release|x64`
2. `FileOptimizer.UI` as the same configuration

The C# project copies `CoreEngine.dll` into its output folder after build when the DLL already exists.

## Important note

The native build currently produces `CoreEngine.dll` correctly, but in this environment MSBuild reports a final cleanup error on one temporary `.tlog` file after linking. The DLL is still generated at:

- `C:\dev\Semester_Project\CoreEngine\x64\Debug\CoreEngine.dll`

If Visual Studio on your machine does not show that cleanup issue, you can ignore it here. The important part is that the DLL itself builds and the C# UI project compiles successfully.
