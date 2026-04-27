# File Optimizer Defense Q&A

This document is written in first-person style so it can be used directly in a project defense.

## 1. Overview & Purpose

### Q1. What does this application do?
This application is a desktop file manager for Windows. It lets me browse folders, open files, search within the current directory, sort items, create and rename files and folders, delete items, use copy/cut/paste behavior, compress and decompress files, and run analysis features such as benchmarks and duplicate detection.

The main user-facing entry point is the WinForms UI in `C:\dev\Semester_Project\FileOptimizer.UI\Form1.cs`.

### Q2. What problem does it solve?
The project solves two problems at once:

1. It provides a usable file management interface similar to a simplified File Explorer.
2. It demonstrates how a real desktop application can separate native system logic from the UI.

I wanted the file-system-heavy work to live in C++, while the desktop interface lives in C# for faster UI development.

### Q3. What are the core features?
The core features are:

- Folder browsing and navigation
- File/folder listing with sorting
- Search within the current directory
- File open on double-click
- Rename, new file, new folder
- Clipboard-style copy, cut, and paste
- Delete for files and folders
- Compression to `.zst` and `.zip`
- Decompression of `.zst` and extraction of `.zip`
- Analysis panel with storage usage, benchmarks, and duplicate detection
- Keyboard shortcuts and right-click context menu
- Console trace output for informative intermediate results

## 2. Architecture & Design

### Q4. How is the project structured?
The project has three layers:

1. `C:\dev\Semester_Project\File-Optimizer`
   This is the native C++ core. It contains classes such as `FileService`, `FileSorter`, `SearchIndex`, `CompressionService`, and `DuplicateDetector`.
2. `C:\dev\Semester_Project\CoreEngine`
   This is a native C++ DLL bridge. It exposes a plain C-style API that C# can call.
3. `C:\dev\Semester_Project\FileOptimizer.UI`
   This is the C# WinForms desktop UI. It calls the DLL through P/Invoke and displays the results.

The solution wiring is visible in `C:\dev\Semester_Project\File-Optimizer\File-Optimizer.sln`.

### Q5. Why did you choose this architecture?
I chose this architecture because direct interop between C# and ordinary C++ classes is difficult and fragile. C++ classes use things like STL containers, name mangling, and compiler-specific ABI behavior. C# P/Invoke works best with exported C-style functions and fixed-layout structs.

So I kept the business logic in C++, then introduced a DLL bridge layer in `C:\dev\Semester_Project\CoreEngine\FileManagerApi.h` and `C:\dev\Semester_Project\CoreEngine\FileManagerApi.cpp`.

### Q6. How do the components interact?
The interaction flow is:

1. The user clicks a button or triggers an action in the WinForms UI.
2. The C# code in `Form1.cs` or `AnalysisForm.cs` calls a helper method in `NativeMethods.cs`.
3. `NativeMethods.cs` uses `DllImport` to call an exported function from `CoreEngine.dll`.
4. `CoreEngine.dll` receives wide strings from C#, converts them to UTF-8 strings, and calls the native C++ services.
5. The native service returns results.
6. The bridge converts those results into plain structs.
7. The C# side converts the structs into view objects such as `FileItemView` or `DuplicateEntryView`.
8. The UI binds those objects to the grid or displays analysis text.

### Q7. Why is there a separate DLL instead of calling the C++ code directly from the UI?
Because the DLL acts as a stable boundary. It hides internal C++ class design from the UI and exposes only simple exported functions. This reduces coupling and makes the project easier to maintain and easier to explain.

### Q8. Why is the UI forced to x64?
The UI project is explicitly built as x64 in `C:\dev\Semester_Project\FileOptimizer.UI\FileOptimizer.UI.csproj`.

That is necessary because the native DLL is also built as x64. A 64-bit C# process must load a 64-bit DLL. Mixing x86 and x64 would fail at runtime.

### Q9. How does the DLL get copied into the UI output?
The C# project file has an MSBuild target named `CopyNativeCore` in `C:\dev\Semester_Project\FileOptimizer.UI\FileOptimizer.UI.csproj`. After the UI build completes, it copies `CoreEngine.dll` and the matching `.pdb` into the UI output folder.

## 3. Data Structures & Algorithms

### Q10. What core data structure represents a file or folder?
The core native model is `FileInfo` in `C:\dev\Semester_Project\File-Optimizer\src\FileInfo.h`.

It stores:

- `name`
- `path`
- `size`
- `modifiedTime`
- `isDirectory`

This is a simple and appropriate structure because every feature needs some or all of those fields.

### Q11. How are directory contents stored in memory?
Directory contents are stored in `std::vector<FileInfo>`. I used a vector because directory listings are naturally sequential collections, and vectors work well for iteration, sorting, and transferring results.

### Q12. How is sorting implemented?
Sorting is implemented in `C:\dev\Semester_Project\File-Optimizer\src\FileSorter.cpp` by `FileSorter::sort`.

It uses `std::sort` with a custom comparator that:

1. Optionally puts directories first
2. Compares by the chosen key: name, size, or modified time
3. Uses name as a tiebreaker
4. Applies ascending or descending order

This is an `O(n log n)` comparison sort, which is the expected complexity for general-purpose sorting.

### Q13. How is search implemented?
Search is implemented by `SearchIndex` in:

- `C:\dev\Semester_Project\File-Optimizer\src\SearchIndex.h`
- `C:\dev\Semester_Project\File-Optimizer\src\SearchIndex.cpp`

The algorithm is:

1. Tokenize file names
2. Build an inverted index: `unordered_map<string, vector<FileInfo>>`
3. Tokenize the query
4. Do exact token matches first
5. Then do partial token matches by checking whether indexed tokens contain the query token
6. Deduplicate results with `unordered_set<string> seenPaths`

### Q14. What exactly does tokenization mean in your search implementation?
Tokenization means splitting a file or folder name into searchable words.

In `SearchIndex::tokenize`:

- the text is converted to lowercase
- `_`, `-`, and `.` are replaced with spaces
- a `stringstream` splits the cleaned text into tokens

For example, `dad_songs-2025.zip` becomes tokens roughly like:

- `dad`
- `songs`
- `2025`
- `zip`

### Q15. Why did you choose an inverted index for search?
I chose an inverted index because it is a standard and efficient way to support token-based lookup. Instead of scanning every name from scratch for every token, I pre-group token-to-file mappings.

For a directory-sized search space, it is a good balance between simplicity and performance.

### Q16. How does partial matching work?
Partial matching happens after exact matching in `SearchIndex::search`.

For each query token:

- it first checks `index.find(token)` for exact matches
- then it loops through all indexed tokens and checks `indexedToken.find(token) != npos`

So `da` can match the indexed token `dad`.

### Q17. What data structures are used for duplicate detection?
Duplicate detection uses several hash-based groupings:

- `unordered_map<string, vector<FileInfo>>` for exact filename groups
- `unordered_map<uintmax_t, vector<FileInfo>>` for file-size buckets
- `unordered_map<string, vector<FileInfo>>` for SHA-256 content hash groups

These are used in `C:\dev\Semester_Project\File-Optimizer\src\DuplicateDetector.cpp`.

### Q18. Why do you group duplicates by size before hashing?
Because files with different sizes cannot be identical in content. Grouping by size is a cheap filter that prevents unnecessary hashing work.

This reduces the total cost significantly when scanning many files.

### Q19. How is file hashing implemented?
Content hashing is implemented with the Windows BCrypt API in `DuplicateDetector.cpp`, inside `computeFileSha256`.

The file is read in 64 KB chunks and streamed into the hash function. That means duplicate hashing is chunked and not all-at-once.

### Q20. What traversal algorithm do you use for recursive duplicate detection?
I use `std::filesystem::recursive_directory_iterator` in `DuplicateDetector::collectFiles`.

It performs a recursive traversal of the directory tree. I enabled `skip_permission_denied`, manually increment with an `error_code`, and disable recursion into symlinks to avoid broken links or problematic recursion.

## 4. Key Implementation Decisions

### Q21. Why did you use C++ for the core logic?
C++ gives low-level access to filesystem operations, good performance, and direct use of native and system-level APIs such as:

- `std::filesystem`
- Windows BCrypt
- Windows process creation APIs
- Zstandard native library

It is also a realistic choice for performance-sensitive or system-oriented functionality.

### Q22. Why did you use C# WinForms for the UI?
I used C# WinForms because:

- it is quick to build Windows desktop interfaces with
- it integrates well with .NET libraries
- it supports event-driven UI development cleanly
- it is simpler for this project than building a full native Windows GUI in C++

### Q23. Why WinForms instead of WPF?
WinForms was the simpler and faster choice for a first working desktop version. WPF would offer richer styling and MVVM patterns, but it would also add more complexity. For a college file manager, WinForms was enough to demonstrate architecture and interop.

### Q24. Why did you use P/Invoke instead of C++/CLI?
P/Invoke works well with a plain C ABI and keeps the projects more independent. C++/CLI could also bridge C++ and .NET, but it introduces a mixed-mode project and extra complexity. A C-style exported API is easier to explain and more reusable.

### Q25. Why did you use `std::filesystem`?
Because it provides modern C++ support for:

- iterating directories
- copying files and folders
- deleting
- renaming
- querying metadata

It is cleaner than old C-style Win32 filesystem code for most of this project.

### Q26. Why did you use Zstandard?
Zstandard support already existed in the project and is linked through `zstd` as shown in `vcpkg.json` and the VC++ project files. It is a modern compression format that offers a good speed-to-ratio balance.

### Q27. Why is ZIP implemented differently from Zstd?
Because `.zip` is not just "another compression level." ZIP is an archive format that can contain multiple files and folders. My Zstd path is currently a single-file codec workflow, while ZIP is archive-oriented.

So I implemented ZIP in `CompressionService.cpp` through Windows PowerShell archive commands:

- `Compress-Archive`
- `Expand-Archive`

### Q28. Why did you use PowerShell for ZIP support instead of another library?
It was a practical decision:

- Windows already provides archive commands
- it avoided introducing another third-party ZIP library
- it kept the code size smaller

The tradeoff is that it is Windows-specific and depends on PowerShell being available.

### Q29. Why is duplicate detection separate from search?
Because they solve different problems.

- Search answers: "What matches this query?"
- Duplicate detection answers: "Which files belong to duplicate groups?"

The algorithms, outputs, and performance considerations are different enough that separating them into `SearchIndex` and `DuplicateDetector` is cleaner.

### Q30. Why does the application use an internal clipboard instead of the Windows clipboard for copy/cut/paste?
I used an internal clipboard in `Form1.cs` because I wanted Explorer-like deferred copy/cut/paste behavior without the complexity of integrating with the global system clipboard.

The UI stores a `ClipboardIntent` record with:

- source path
- source name
- action type: copy or cut

Then paste executes later in the chosen destination folder.

## 5. Core Features: How They Work

### Q31. How does file listing work?
The call flow is:

1. `Form1.NavigateTo` calls `NativeMethods.GetDirectoryContents`
2. `NativeMethods` calls `Core_GetDirectoryContents`
3. `Core_GetDirectoryContents` validates the path and calls `getSortedItems`
4. `getSortedItems` calls `FileService::getDirectoryContent`
5. `FileService` uses `std::filesystem::directory_iterator` to gather metadata into `FileInfo`
6. `FileSorter::sort` sorts the vector
7. `copyResults` converts the `FileInfo` vector into `CoreFileInfo` structs
8. `NativeMethods` converts those into `FileItemView` objects
9. `Form1.BindItems` binds them to the `DataGridView`

### Q32. How do you obtain file metadata?
Inside `FileService::getDirectoryContent`:

- name comes from `entry.path().filename()`
- path comes from `entry.path()`
- directory status comes from `entry.is_directory()`
- file size comes from `entry.file_size()` for non-directories
- modified time comes from `std::filesystem::last_write_time`

The modified time is converted from `file_clock` to `system_clock` and then to `time_t`.

### Q33. How does navigation work?
Navigation is handled in `Form1.cs`.

- `NavigateTo` loads a new directory
- `RecordNavigation` appends it to history
- `GoBack` and `GoForward` move across history
- `GoUp` navigates to the parent directory

This is similar to a simplified browser history model.

### Q34. How does opening a file work?
In `Form1.OpenItem`:

- if the selected item is a directory, it navigates into it
- if it is a file, it calls `Process.Start` with `UseShellExecute = true`

That tells Windows to open the file with its default associated application.

### Q35. How does search work from the UI all the way to the core?
The flow is:

1. The user types a query and clicks Search
2. `Form1.searchButton_Click` calls `NativeMethods.SearchDirectoryContents`
3. `NativeMethods` calls `Core_SearchDirectoryContents`
4. `Core_SearchDirectoryContents` first loads the current directory items
5. It builds a `SearchIndex` from those items
6. It runs `SearchIndex::search(query)`
7. It sorts the results
8. The results are marshaled back into C#
9. The grid displays the matched rows

### Q36. Does search scan the whole drive?
No. The current implementation only searches within the currently loaded directory contents. It is not a full recursive drive indexer.

### Q37. How does rename work?
Rename is initiated from `Form1.RenameSelectedItem`.

- the UI gets the new name through `TextPrompt`
- it builds the destination path using the same parent directory
- it calls `NativeMethods.RenamePath`
- that calls `Core_RenamePath`
- which calls `FileService::renamePath`
- `renamePath` uses `std::filesystem::rename`

### Q38. How does new file creation work?
In `Form1.CreateNewFile`, the UI asks for a filename and calls `NativeMethods.CreateEmptyFile`.

The native call ends up in `FileService::createEmptyFile`, which opens an `std::ofstream` in binary mode and checks `output.good()`.

### Q39. How does new folder creation work?
The UI calls `NativeMethods.CreateDirectory`, which calls `Core_CreateDirectory`, which calls `FileService::createDirectory`.

That method uses `std::filesystem::create_directory`.

### Q40. How does delete work?
Delete starts in `Form1.deleteButton_Click`:

- the UI confirms with a message box
- it calls `NativeMethods.DeletePath`
- `Core_DeletePath` calls `FileService::deletePath`

`deletePath`:

- checks whether the target exists
- if it is a folder, it uses `remove_all`
- if it is a file, it uses `remove`

### Q41. How does copy/cut/paste work?
Copy and cut do not immediately modify the filesystem.

Instead:

- `CopySelectedItem` stores a `ClipboardIntent` with action `Copy`
- `CutSelectedItem` stores a `ClipboardIntent` with action `Cut`

Later, `PasteClipboardIntent`:

- gets the current directory
- builds the destination path
- prevents self-paste
- rejects name conflicts
- calls `NativeMethods.CopyPath` for copy
- calls `NativeMethods.MovePath` for cut

### Q42. How does folder copy work?
Folder copy is supported by `FileService::copyPath`. It checks whether the source is a directory, and if so it calls:

`std::filesystem::copy(source, destination, std::filesystem::copy_options::recursive)`

### Q43. How does move work under the hood?
`FileService::movePath` tries `std::filesystem::rename` first, because rename is usually fast within the same filesystem.

If that fails, it falls back to:

1. copy
2. delete original

This helps when moving across volumes or situations where rename is not possible.

### Q44. How does compression work?
Compression is routed through `CompressionService` in `CompressionService.cpp`.

The UI decides the format from the destination extension:

- `.zst` means Zstandard compression
- `.zip` means ZIP archive creation

Then `NativeMethods.CompressPath` calls `Core_CompressPath`, which calls `compressPath`.

### Q45. How does Zstd compression work?
Zstd compression uses `compressFile` in `ZstdCompressor.cpp`.

The current method:

1. opens the whole input file
2. loads it into a `std::vector<char>`
3. allocates a compression buffer using `ZSTD_compressBound`
4. calls `ZSTD_compress`
5. writes the compressed bytes to the output file

### Q46. How does ZIP compression work?
ZIP compression is implemented in `CompressionService.cpp`.

It:

1. validates the source exists
2. validates the destination ends in `.zip`
3. escapes the paths for PowerShell single-quoted strings
4. launches a hidden PowerShell process with `Compress-Archive`

### Q47. How does decompression work?
Decompression also goes through `CompressionService`.

- `.zst` uses `decompressFile` in `ZstdCompressor.cpp`
- `.zip` uses PowerShell `Expand-Archive`

### Q48. Why is ZIP extraction folder-based but Zstd decompression file-based?
Because ZIP is an archive format and can contain multiple files and folders, so it naturally extracts into a folder. Zstd in this project is used as a single-file compressed stream, so it naturally decompresses to one output file.

### Q49. How does duplicate name detection work?
`DuplicateDetector::findDuplicateNames`:

1. recursively collects files
2. groups them by exact filename using `unordered_map<string, vector<FileInfo>>`
3. returns only groups with at least two files

### Q50. How does duplicate content detection work?
`DuplicateDetector::findDuplicateContents`:

1. recursively collects files
2. groups them by file size
3. only hashes groups with at least two same-size files
4. computes SHA-256 for those files
5. groups by hash
6. returns only hash groups with at least two files

### Q51. How does the analysis panel work?
The analysis panel is defined by:

- `AnalysisForm.cs`
- `AnalysisForm.Designer.cs`
- `AnalysisService.cs`

The UI is a separate dialog window. Each button calls an analysis method in `AnalysisService`, which then either:

- uses .NET directory traversal for storage summary
- or reuses native functionality through `NativeMethods`

### Q52. How do the search and sort benchmarks work?
They are implemented in `AnalysisService`.

- Search benchmark repeatedly calls `NativeMethods.SearchDirectoryContents`
- Sort benchmark repeatedly calls `NativeMethods.GetDirectoryContents` with different sort keys

These benchmarks measure end-to-end operation time, not just the pure algorithm in isolation.

## 6. Error Handling & Edge Cases

### Q53. How does the application report errors from C++ back to C#?
The bridge stores the last native error message in a `thread_local std::wstring g_lastError` in `FileManagerApi.cpp`.

When a native operation fails:

- the API returns `0` or `-1`
- `NativeMethods` calls `Core_GetLastErrorMessage`
- C# throws an `InvalidOperationException` with that message

### Q54. What happens if the user enters an invalid path?
The path is validated in `getSortedItems` inside `FileManagerApi.cpp`.

It throws if:

- the path is null or empty
- the path does not exist
- the path is not a directory

The UI catches the exception and shows the message.

### Q55. What happens if the native DLL is missing?
`Form1.NavigateTo` catches `DllNotFoundException` and shows a user-friendly message telling the user to build the `CoreEngine` project first.

### Q56. How are missing or unreadable files handled during duplicate scanning?
`DuplicateDetector::collectFiles` uses:

- `skip_permission_denied`
- manual `error_code` increments
- a `try/catch` around per-entry processing

So it skips problematic entries instead of failing the entire scan.

### Q57. How does the application handle broken links or junctions?
In duplicate scanning, if `entry.is_symlink()` is true, recursion is disabled for that branch. This avoids following problematic links or broken targets.

### Q58. What happens if the destination already exists during paste or copy?
At the UI level, `PasteClipboardIntent` in `Form1.cs` rejects the operation early and shows:

"An item with the same name already exists in this folder."

At the native level, `FileService::copyPath` and `movePath` also reject if the destination already exists.

### Q59. How do you prevent copy-paste into the same exact path?
`PasteClipboardIntent` compares the source path and destination path case-insensitively and rejects if they are the same.

### Q60. What happens if the user tries to decompress an unsupported file type?
`Form1.decompressButton_Click` checks the extension and allows only:

- `.zip`
- `.zst`

Anything else is rejected with a clear message.

### Q61. What happens if the user tries to compress a folder as Zstd?
The UI prevents it and shows an error. Folders are currently allowed only for ZIP compression.

### Q62. What happens if a search query is empty?
The UI treats an empty query as a normal directory listing refresh instead of calling the search API.

### Q63. What edge cases are handled in navigation history?
`RecordNavigation` avoids re-adding the same current path, and if the user navigates after going back, it truncates the forward history before adding the new path. That matches typical history behavior.

### Q64. Are there places where error handling is intentionally best-effort rather than strict?
Yes.

Examples:

- `FileService::getDirectoryContent` catches exceptions and returns whatever was collected so far or an empty vector
- temporary benchmark cleanup in `AnalysisService` is best-effort
- duplicate scanning skips unreadable files instead of failing

This is a practical file-manager choice: continue working when possible.

## 7. Performance & Scalability

### Q65. How does the app perform on large directories?
For ordinary directories it should be fine, because listing and sorting use standard vector-based algorithms. But there are scalability limits:

- search rebuilds the index each time instead of caching it
- the UI runs operations synchronously on the UI thread
- large folders can make the interface feel blocked during loading

### Q66. What optimizations are present?
The main optimizations are:

- using `std::vector` for sequential data
- using `std::sort` for efficient sorting
- using an inverted index for search
- using `unordered_map` and `unordered_set` for fast average-case hash lookups
- using size bucketing before hashing in duplicate detection
- reading duplicate-hash input files in 64 KB chunks

### Q67. What are the major performance limitations?
The major ones are:

- no asynchronous or background execution
- Zstd compression reads the entire file into memory
- Zstd decompression also reads the entire compressed file into memory
- search indexing is rebuilt on each search
- analysis operations can block the UI
- duplicate detection can be expensive for large directory trees

### Q68. How scalable is duplicate detection?
The size-bucketing strategy helps a lot, but a recursive full-tree scan with hashing still grows with the number and total size of files. It is reasonable for a college project and medium-scale folders, but it is not yet optimized like a production deduplication engine.

### Q69. How scalable is the search implementation?
It is acceptable for current-directory search, but not for full-drive search at scale. Because the index is built from the current directory on demand and partial matching scans indexed tokens, it is more of a "local folder search" than a persistent high-scale search engine.

### Q70. Why didn't you make operations asynchronous?
The goal was to get a clear working architecture first. Moving long-running operations into background tasks would improve responsiveness, but it would also require progress reporting, thread-safe UI updates, and cancellation design.

## 8. Security Considerations

### Q71. Are there security risks in this kind of application?
Yes, but they are mostly local-desktop risks rather than web-style risks.

The application can modify files on the machine, so the main risks are:

- deleting or moving the wrong path
- handling untrusted file names incorrectly
- launching the wrong external process
- following bad paths or links

### Q72. How do you mitigate risky path handling?
I mitigate it by:

- validating whether paths exist and whether they are directories when needed
- using `Path.Combine` in the UI for destination construction
- rejecting identical source and destination paths
- rejecting destination conflicts before copy/move
- using `std::filesystem` path-aware operations instead of building shell commands for file operations

### Q73. Is there a path traversal vulnerability?
Not in the typical web sense, because this is a local desktop app, not a server exposing paths to remote users. The user is intentionally choosing and entering local paths. The operating system still enforces access permissions.

### Q74. Does the app bypass Windows permissions?
No. It runs with the user's permissions. If the OS denies access, the operation fails.

### Q75. Are there any security concerns with file opening?
Yes. `Process.Start` with `UseShellExecute = true` trusts Windows file associations. That is the normal desktop behavior, but it means opening a file triggers the external application associated with that file type.

### Q76. Are there any security concerns with ZIP support through PowerShell?
Yes, the main consideration is external process invocation.

I reduced the risk by:

- using a fixed PowerShell executable path
- hiding the PowerShell window
- escaping single quotes in paths before inserting them into the command
- using `LiteralPath`

That said, it is still more complex than calling a dedicated in-process ZIP library.

## 9. Testing

### Q77. How was the application tested?
Testing was mainly manual and feature-oriented.

I verified:

- directory loading
- navigation
- search behavior
- sorting modes
- file open
- copy/cut/paste
- rename/new file/new folder
- delete
- compression/decompression
- duplicate detection
- analysis panel output
- keyboard shortcuts

### Q78. Is there a unit test suite?
No, there is no real automated unit test suite in the current project.

There is a native console test harness in `C:\dev\Semester_Project\File-Optimizer\src\main.cpp`, but it is more of a manual test console than a formal unit test framework.

`Test.cpp` is just a placeholder and is not a meaningful test file.

### Q79. What kind of testing does `main.cpp` provide?
It provides an interactive console that can manually exercise:

- directory browsing
- sorting
- search
- copy
- delete
- compression
- decompression

It is useful during development, but it is not automated and not exhaustive.

### Q80. How were benchmarks tested?
The analysis window itself serves as a runtime test surface for:

- search timing
- sort timing
- compression ratios and timings
- storage summary
- duplicate scans

### Q81. What would proper testing look like if you had more time?
I would add:

- unit tests for `SearchIndex`, `FileSorter`, and path-validation helpers
- integration tests for DLL calls from C#
- test directories with known expected outputs
- regression tests for duplicate detection
- UI automation tests for main flows

## 10. Known Limitations & Future Improvements

### Q82. What are the biggest current limitations?
The biggest current limitations are:

- search is current-directory only
- no multi-select operations
- no overwrite/merge conflict dialog during paste
- some operations can freeze the UI because they run on the UI thread
- Zstd compression/decompression is memory-heavy for large files
- no formal automated test suite
- no byte-by-byte verification after matching SHA-256 hashes
- ZIP support depends on PowerShell
- path buffers in the native bridge are fixed-size and can truncate very long paths

### Q83. Why are fixed-size native buffers a limitation?
In `FileManagerApi.h`, the exported structs use fixed-size `wchar_t` arrays:

- `CORE_NAME_CAPACITY = 260`
- `CORE_PATH_CAPACITY = 1024`

That makes interop simple and safe for normal paths, but extremely long paths or names can be truncated.

### Q84. Why can the UI freeze during long operations?
Because most operations are executed directly on the UI thread in event handlers. That keeps the code simpler, but if a directory is huge or an operation is expensive, the form can become temporarily unresponsive.

### Q85. What would you improve first if you had more time?
My highest-priority improvements would be:

1. Move long operations to background tasks
2. Add progress indicators and cancellation
3. Add overwrite/conflict resolution dialogs
4. Add multi-select support
5. Replace PowerShell ZIP handling with a dedicated archive library
6. Add streaming Zstd compression/decompression
7. Add automated tests
8. Add recursive search or a persistent search index

### Q86. Would you keep the same architecture in a future version?
Yes. I would keep the three-layer structure because it is one of the strongest parts of the project:

- native core logic
- DLL bridge
- managed UI

That separation is clean, extensible, and easy to reason about.

## 11. Honest Technical Self-Assessment

### Q87. What are the strongest design choices in this project?
The strongest design choices are:

- separating UI from native logic
- using a C-style bridge for reliable interop
- keeping feature-specific logic in focused services like `FileService`, `SearchIndex`, and `DuplicateDetector`
- using appropriate hash-based groupings for search and duplicate detection
- making analysis features a separate panel instead of mixing them into the main file-browser workflow

### Q88. What are the weakest parts or most provisional parts?
The weakest or most provisional parts are:

- lack of automated tests
- synchronous UI operations
- PowerShell-based ZIP implementation
- whole-file Zstd memory loading
- some best-effort exception swallowing in native file listing

### Q89. If a professor asks, "What would you refactor first?" what would you say?
I would say:

"First I would make long-running operations asynchronous and add progress reporting, because that would improve real usability immediately. After that I would refactor compression to a streaming model and replace the PowerShell ZIP path with a proper archive library."

## 12. Short Rapid-Fire Answers

### Q90. One-minute architecture summary?
"The application has three layers: a native C++ core for filesystem and utility logic, a native DLL bridge that exposes a simple C API, and a C# WinForms UI that calls the DLL through P/Invoke. The UI handles interaction and display, while the DLL translates calls into native services like file listing, search, sorting, compression, and duplicate detection."

### Q91. Why not build everything in C#?
"I wanted a more realistic mixed-language architecture where low-level file operations live in C++ and the UI is easier to build in C#. It also demonstrates interop design."

### Q92. Why not call C++ classes directly?
"Because direct C++ class interop is harder due to ABI issues, name mangling, STL types, and memory ownership. A plain exported C API is simpler and safer."

### Q93. What algorithmic ideas should I mention confidently?
Mention:

- `std::sort` with custom comparator
- inverted index for search
- tokenization and deduplication with `unordered_set`
- recursive filesystem traversal
- size bucketing before SHA-256 hashing

### Q94. What limitation should I mention before they point it out?
I would proactively mention:

"Search currently works on the current directory, not the full drive, and long operations still run synchronously on the UI thread. Those are the main areas I would improve next."

### Q95. What is the best way to describe the project's maturity level?
I would describe it as:

"a working functional prototype with a solid architecture, several practical desktop features, and clear next steps toward production-level robustness."
