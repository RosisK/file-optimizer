# File Optimizer

File Optimizer is a desktop file manager project built with a split architecture:

- C++ for the core file-system logic and utility services
- C# WinForms for the desktop UI
- a native DLL bridge between them

The goal is to keep the heavy file operations in native code while using C# for a faster and more comfortable Windows desktop interface.

## Project Structure

- `C:\dev\Semester_Project\File-Optimizer`
  Main C++ source project containing file services, sorting, search, compression support, duplicate detection, and supporting logic.
- `C:\dev\Semester_Project\CoreEngine`
  Native C++ DLL project that exposes a C-style API for the UI.
- `C:\dev\Semester_Project\FileOptimizer.UI`
  C# WinForms desktop application that calls `CoreEngine.dll` through P/Invoke.

## Architecture

The UI does not call C++ classes directly.

Instead, the flow is:

1. The C# UI calls exported native functions from `CoreEngine.dll`
2. `CoreEngine.dll` translates those calls into the existing C++ services
3. The native layer returns plain structs or success/failure results
4. C# converts those results into UI-friendly objects and displays them

This keeps the UI and the core loosely coupled and makes the project easier to explain and extend.

## Current Features

### File Manager Basics

- Browse folders
- Enter folders by double-click
- Open files with the default Windows application
- Go up to the parent folder
- Refresh the current folder
- Rename files and folders
- Create new empty files
- Create new folders
- Right-click context menu for common actions

### Search and Sorting

- Search in the current directory
- Case-insensitive token-based search
- Partial token matching
- Sort by name, size, or modified time
- Ascending/descending sorting
- Directories-first sorting

### Clipboard-Style File Operations

- Copy intent and paste later
- Cut intent and paste later
- Paste into the current folder
- Works for both files and folders
- Delete files and folders

### Compression and Extraction

- Compress to `zst`
- Compress to `zip`
- Decompress `zst`
- Extract `zip`
- Folder compression through ZIP

### Analysis Panel

- Storage usage summary
- Search benchmark
- Sort benchmark
- Compression benchmark
- Duplicate-name detection
- Duplicate-content detection using file-size grouping and SHA-256 hashing

## Keyboard Shortcuts

- `Ctrl + C` copy selected item into the app clipboard
- `Ctrl + X` cut selected item into the app clipboard
- `Ctrl + V` paste into the current folder
- `Delete` delete selected item
- `F2` rename selected item
- `Ctrl + L` focus the address/path bar
- `Ctrl + Shift + N` create a new folder
- `Alt + Left` navigate back in folder history
- `Alt + Right` navigate forward in folder history
- `Alt + Up` go to parent folder
- `Backspace` go to parent folder when not typing in a text field
- `Enter` open the selected file or folder
- `Escape` move focus from text inputs back to the file grid

## Duplicate Detection Design

Duplicate detection is implemented separately from search.

- Name duplicates are grouped by exact filename
- Content duplicates are grouped by file size first
- Files in same-size groups are hashed with SHA-256
- Matching hashes are reported as duplicate-content groups

This keeps duplicate detection clean and avoids overloading the search system with a different responsibility.

## Build and Run

Recommended build order:

1. Build `CoreEngine` as `Debug|x64` or `Release|x64`
2. Build `FileOptimizer.UI` with the same configuration
3. Run `FileOptimizer.UI`

The C# project copies `CoreEngine.dll` into its output folder when the DLL already exists.

## Notes and Current Limitations

- The UI currently behaves like a single-selection file manager, not full multi-select Explorer behavior
- Paste currently stops if the destination already contains an item with the same name
- Duplicate detection skips broken/unreadable paths instead of failing the full scan
- ZIP support is implemented through built-in Windows PowerShell archive commands from the native layer

## Build Environment Note

In this environment, the native build may report a final cleanup error involving a temporary `.tlog` file after linking.

Important detail:

- `CoreEngine.dll` is still produced correctly even when that cleanup error appears

If Visual Studio on your machine does not show that cleanup issue, you can ignore it here. The functional DLL output is the important part.
