# File Optimizer: Desktop File Manager and Optimization System

Submitted by:

- Rosis Kharel (79010508)
- Aayam Pradhan (79010486)
- Bibash Adhikari (_______________)

Submitted to:

- Prime College
- BSc. CSIT
- Tribhuvan University
- 7th Semester
- CSC 422, Project Work

Supervisor:

- Sravan Ghimire, Lecturer

Submission Date: ______________________

<<<PAGEBREAK>>>

# PROJECT SUBMISSION PAGE

**Project Title:** File Optimizer: Desktop File Manager and Optimization System

This project report is submitted in partial fulfillment of the requirements of the degree of Bachelor of Science in Computer Science and Information Technology (BSc. CSIT), 7th Semester, under Tribhuvan University.

**Submitted By**

- Rosis Kharel (79010508)
- Aayam Pradhan (79010486)
- Bibash Adhikari (_______________)

**Submitted To**

- Prime College
- BSc. CSIT
- Tribhuvan University
- CSC 422, Project Work

**Supervisor**

- Sravan Ghimire, Lecturer

Submission Date: ______________________

<<<PAGEBREAK>>>

# SUPERVISOR'S RECOMMENDATION

It is my pleasure to recommend that the project report entitled **"File Optimizer: Desktop File Manager and Optimization System"** has been prepared under my supervision by **Rosis Kharel**, **Aayam Pradhan**, and **Bibash Adhikari** in partial fulfillment of the requirement for the degree of Bachelor of Science in Computer Science and Information Technology (BSc. CSIT), Tribhuvan University.

To the best of my knowledge, the work presented in this report is original and satisfactory in scope and quality for further academic evaluation.

Supervisor's Name: Sravan Ghimire

Designation: Lecturer

Signature: ______________________

Date: ______________________

<<<PAGEBREAK>>>

# CERTIFICATE OF APPROVAL

This is to certify that the project report entitled **"File Optimizer: Desktop File Manager and Optimization System"** submitted by **Rosis Kharel**, **Aayam Pradhan**, and **Bibash Adhikari** in partial fulfillment of the requirements for the degree of Bachelor of Science in Computer Science and Information Technology (BSc. CSIT), Tribhuvan University, is hereby recommended for acceptance.

| Position | Name | Signature | Date |
| --- | --- | --- | --- |
| Supervisor | Sravan Ghimire | ____________________ | ____________________ |
| Internal Examiner | ____________________ | ____________________ | ____________________ |
| External Examiner | ____________________ | ____________________ | ____________________ |
| Head of Department / Coordinator | ____________________ | ____________________ | ____________________ |

<<<PAGEBREAK>>>

# ACKNOWLEDGEMENT

We would like to express our sincere gratitude to our supervisor, **Sravan Ghimire**, for his valuable guidance, continuous encouragement, and constructive suggestions throughout the development of this project. His support played an important role in refining both the technical implementation and the structure of this report.

We are equally thankful to **Prime College**, the **BSc. CSIT** department, and all faculty members who provided us with the academic environment, resources, and motivation necessary to complete this project. We also acknowledge the support of our classmates, friends, and family members for their patience and encouragement during the project period.

This project gave us practical exposure to desktop application design, native-managed interoperability, Windows file-system programming, performance analysis, and software engineering practices. We are grateful for the opportunity to apply classroom concepts to a complete system that solves a practical problem.

<<<PAGEBREAK>>>

# ABSTRACT

Traditional desktop file managers provide common operations such as browsing, copying, deleting, and renaming files, but they often require users to rely on separate utilities for compression, duplicate detection, and storage analysis. This project, **File Optimizer**, was developed as a Windows desktop file management and optimization system intended to serve as an alternative to the default Windows file manager while also offering additional analysis and productivity features.

The system follows a hybrid architecture in which the core file-processing logic is implemented in **C++** and the graphical user interface is developed in **C# WinForms**. A native bridge layer named **CoreEngine** exposes C-style DLL functions that allow the managed C# user interface to safely call C++ functionality using **P/Invoke**. This design enables clear separation between presentation logic and performance-sensitive file operations.

The implemented system supports directory navigation, file opening, search, sorting, copy-cut-paste, rename, deletion, new file and folder creation, compression and decompression, duplicate detection, keyboard shortcuts, context-menu interaction, and a separate analysis panel for storage summary and performance-oriented observations. Compression support includes **Zstandard (.zst)** for file compression and **ZIP** for archive creation and extraction. Duplicate detection is performed using both exact file-name grouping and content-based grouping with **SHA-256 hashing**.

The project demonstrates how native and managed technologies can be combined to create a practical desktop application with modular architecture. It also highlights how a traditional file manager can be extended with optimization and monitoring capabilities in a single interface. The report documents the problem domain, design, implementation, testing plan, and future scope of the project.

**Keywords:** File Manager, Desktop Application, C++, C#, P/Invoke, Duplicate Detection, Compression, ZIP, Zstandard, WinForms

<<<PAGEBREAK>>>

# TABLE OF CONTENTS

- Supervisor's Recommendation
- Certificate of Approval
- Acknowledgement
- Abstract
- List of Abbreviations
- List of Figures
- List of Tables
- Chapter 1: Introduction
- Chapter 2: Background Study and Literature Review
- Chapter 3: System Analysis
- Chapter 4: System Design
- Chapter 5: Implementation and Testing
- Chapter 6: Conclusion and Future Recommendations
- References
- Appendices

Note: Update the table of contents page numbers in Microsoft Word after final formatting if required by your department.

<<<PAGEBREAK>>>

# LIST OF ABBREVIATIONS

| Abbreviation | Meaning |
| --- | --- |
| API | Application Programming Interface |
| CPU | Central Processing Unit |
| DLL | Dynamic-Link Library |
| GUI | Graphical User Interface |
| IDE | Integrated Development Environment |
| P/Invoke | Platform Invocation Services |
| SHA-256 | Secure Hash Algorithm 256-bit |
| UI | User Interface |
| UTF | Unicode Transformation Format |
| ZIP | Zip Archive Format |
| Zstd | Zstandard Compression Format |
| WinForms | Windows Forms |
| CNG | Cryptography API: Next Generation |

<<<PAGEBREAK>>>

# LIST OF FIGURES

| Figure No. | Title | Status |
| --- | --- | --- |
| Figure 1.1 | Incremental Development Model | Placeholder |
| Figure 3.1 | Use Case Diagram of File Optimizer | Placeholder |
| Figure 3.2 | Gantt Chart / Development Timeline | Placeholder |
| Figure 4.1 | System Architecture Diagram | Placeholder |
| Figure 4.2 | Component Diagram | Placeholder |
| Figure 4.3 | Sequence Diagram for File Browsing and Search | Placeholder |
| Figure 4.4 | Activity Diagram for Copy-Cut-Paste Workflow | Placeholder |
| Figure 5.1 | Main Window of File Optimizer | Placeholder |
| Figure 5.2 | Analysis Panel Window | Placeholder |

<<<PAGEBREAK>>>

# LIST OF TABLES

| Table No. | Title |
| --- | --- |
| Table 3.1 | Functional Requirements |
| Table 3.2 | Non-Functional Requirements |
| Table 3.3 | Feasibility Analysis |
| Table 3.4 | Use Case Scenario Summary |
| Table 5.1 | Tools Used in Implementation |
| Table 5.2 | Unit Test Cases |
| Table 5.3 | System Test Cases |
| Table 5.4 | Search Performance Observation Table |
| Table 5.5 | Sort Performance Observation Table |
| Table 5.6 | Compression and Storage Analysis Table |
| Table 5.7 | Duplicate Detection Analysis Table |

<<<PAGEBREAK>>>

# CHAPTER 1 INTRODUCTION

## 1.1 Introduction

File management is one of the most common activities performed on personal computers. Users continuously browse folders, open documents, sort files, move data between locations, compress content for sharing, and remove redundant files. The default file-management tools available in most operating systems are useful for basic interaction, but they often rely on separate utilities for advanced tasks such as compression analysis, duplicate detection, and storage monitoring.

This project, **File Optimizer**, was developed as a Windows desktop application that combines file management and file optimization tasks into one environment. The system is intended to serve as an alternative to the traditional Windows file manager while providing additional analytical and productivity features. The application allows users to browse and manage files in a familiar way, while also providing compression options, duplicate detection, storage summaries, and benchmarking-oriented analysis.

One of the key technical goals of the project is the use of two programming languages in a practical software architecture. The user interface is implemented in **C# using WinForms**, while the core file-processing and optimization logic is implemented in **C++**. A native DLL bridge exposes C-style functions so that the C# layer can call the C++ layer safely and efficiently. This approach reflects a real-world architecture in which presentation and core-processing responsibilities are separated.

The completed system includes file browsing, opening, searching, sorting, internal clipboard-based copy and cut operations, paste into target folders, rename, deletion, creation of new files and folders, ZIP and Zstandard compression support, duplicate detection by both filename and file content, right-click menus, keyboard shortcuts, and a separate analysis panel for storage and performance-oriented inspection.

## 1.2 Problem Statement

The default file manager provided with an operating system is primarily intended for general-purpose file access. While it supports many day-to-day operations, several useful optimization and analysis tasks are not integrated in a simple academic project-friendly interface. Users commonly need additional tools for:

- compressing and extracting archives in multiple formats,
- identifying duplicate files,
- monitoring storage usage inside a folder tree,
- comparing search or sort behavior,
- and accessing file-management utilities and performance observations in one place.

From a software-engineering perspective, there is also a learning problem. Many student projects are implemented in a single language, which can hide the architectural challenges of native code, managed code, and inter-language communication. This project addresses that gap by building a practical desktop application in which **C++** handles core operations and **C#** provides a user-friendly graphical environment.

Therefore, the main problem addressed by this project is the lack of a compact desktop application that combines standard file-management behavior with built-in optimization and analysis features while also demonstrating a modular native-plus-managed architecture suitable for real-world desktop systems.

## 1.3 Objectives

The main objectives of the project are:

- To develop a Windows desktop file manager that supports everyday file and folder operations.
- To implement the core file-processing logic in C++.
- To design a graphical desktop interface in C# WinForms.
- To connect the C# UI with the C++ core through a native DLL bridge using P/Invoke.
- To support directory browsing, file opening, searching, sorting, renaming, deletion, new file creation, and new folder creation.
- To implement copy, cut, and paste behavior using an internal clipboard similar to the behavior of standard operating-system file managers.
- To provide compression and decompression support using Zstandard and ZIP.
- To detect duplicates by filename and by file content using hashing techniques.
- To provide an analysis panel for storage usage, search observation, sorting observation, compression observation, and duplicate analysis.
- To build the project in a modular form that can be extended in the future.

## 1.4 Scope and Limitation

### 1.4.1 Scope

The scope of the project includes the following:

- Windows desktop application for file browsing and management.
- Native C++ core for file-system operations and optimization logic.
- C# WinForms interface for interaction and visualization.
- Search inside the current directory view using token-based and partial-token matching.
- Sorting by name, size, and modified date.
- File and folder copy, cut, paste, rename, delete, new file, and new folder operations.
- Double-click opening of files with their default associated applications.
- Compression and decompression using `.zst` and `.zip`, with folder compression supported through ZIP.
- Duplicate detection by exact file name and by SHA-256 content hash.
- Analysis features for storage summary and simple benchmark-style observations.
- Keyboard shortcuts and right-click context menus for improved usability.

### 1.4.2 Limitation

The current system has the following limitations:

- The application is currently Windows-specific.
- ZIP support depends on Windows PowerShell archive commands.
- Zstandard compression currently supports files only, not arbitrary folder-to-zst packaging.
- Search is performed on the current directory contents and is not a full recursive indexing engine.
- The internal clipboard currently handles one item at a time rather than multi-select clipboard operations.
- File conflict handling during paste is currently basic and does not provide advanced overwrite or rename dialogs.
- The analysis panel provides useful observations but is not yet a full automated reporting dashboard with charts or export formats.
- Some academic testing values and benchmark measurements still need to be executed and recorded.

## 1.5 Development Methodology

The project was developed using an **Incremental Development Model**. This methodology was suitable because the application could be built feature by feature, with each iteration producing a working extension of the previous one.

**Iteration 1: Core File Functionality in C++**

The initial phase focused on implementing the basic file-service layer in C++. This stage covered directory navigation support, file listing, sorting, searching, and the original Zstandard compression functionality. The goal of this iteration was to establish a reusable native core independent of the user interface.

**Iteration 2: Native-Managed Integration**

The second phase introduced a dedicated native bridge layer named **CoreEngine**. The bridge exported plain C-style functions from a C++ DLL and translated data between native C++ types and the managed C# layer. This phase established the interoperability model between the core logic and the future desktop interface.

**Iteration 3: User Interface Development**

In this iteration, the C# WinForms application was created. The UI began with file browsing, path navigation, searching, sorting, and operation buttons. The application was then extended with double-click file opening, status reporting, and right-click interactions.

**Iteration 4: Advanced Utilities**

The project was expanded with rename, new file, new folder, copy, cut, paste, and folder-aware operations. At the same stage, ZIP compression and extraction were added alongside the earlier Zstandard support.

**Iteration 5: Analysis and Duplicate Detection**

The final major iteration focused on enhancing the system beyond ordinary file browsing. This included analysis utilities such as storage summaries, search timing observations, sort timing observations, compression observations, and duplicate detection using filename grouping and content hashing.

This methodology allowed the project to remain functional at every stage while progressively adding more advanced features.

**Placeholder:** Insert Figure 1.1 here: Incremental Development Model

## 1.6 Report Organization

This report is organized into six chapters.

- **Chapter 1** introduces the project, problem statement, objectives, scope, limitations, methodology, and report organization.
- **Chapter 2** discusses the background study and reviews the technical foundations used in the system.
- **Chapter 3** presents system analysis, including requirements and feasibility.
- **Chapter 4** explains the system design, architecture, components, and algorithm details.
- **Chapter 5** describes implementation details, tools used, test cases, and result-oriented observation tables.
- **Chapter 6** concludes the report and provides recommendations for future enhancement.

<<<PAGEBREAK>>>

# CHAPTER 2 BACKGROUND STUDY AND LITERATURE REVIEW

## 2.1 Background Study

Desktop file managers are an essential part of modern operating systems because they provide direct access to folders, files, storage devices, and everyday file operations. The standard expectations from a file manager include navigation, file opening, file organization, sorting, copying, moving, renaming, and deletion. Over time, user expectations have increased, and many users now also expect compression support, duplicate cleanup assistance, and better visibility into storage usage.

From a software architecture perspective, desktop file managers are a useful case study because they involve both user-interface design and low-level system interaction. Operations such as folder traversal, metadata retrieval, path handling, and content hashing benefit from efficient system-level code. On the other hand, user interaction, windows, forms, menus, and status updates are often easier to implement in higher-level frameworks. This makes a mixed-language architecture a practical choice.

In this project, **C++** is used for core file-processing logic because it offers direct access to system libraries and efficient control over data structures and performance-sensitive operations. **C# WinForms** is used for the desktop interface because it provides a productive environment for building Windows applications with forms, controls, and event-driven interaction. The bridge between the two layers is implemented through **P/Invoke**, which allows managed code to call exported functions from a native DLL.

The system also relies on several important technical concepts:

- **File-system traversal:** required for listing directories, collecting file metadata, and recursive duplicate detection.
- **Sorting algorithms:** used to arrange files by name, size, and modification time.
- **Index-based search:** used to tokenize file names and match user queries.
- **Compression and archive handling:** used to reduce file size and package file/folder data for transfer or storage.
- **Hash-based duplicate detection:** used to compare file contents efficiently.
- **User-interaction design:** required to present operations in a way familiar to users of common desktop file managers.

## 2.2 Literature Review

Because this project is a systems-oriented desktop application rather than a machine-learning or scientific prediction system, the literature review focuses on technical foundations, framework documentation, and design approaches relevant to real desktop software.

### 2.2.1 Windows Forms as a Desktop UI Platform

Windows Forms is a mature desktop UI framework for Windows that supports controls, event-driven programming, graphics, user input, and designer-assisted development. It is suitable for projects that need rapid creation of forms, menus, toolbars, and dialogs. In this project, WinForms is used to implement the visible application interface, including the file grid, path bar, search controls, operation buttons, and analysis panel.

### 2.2.2 Native and Managed Interoperability through P/Invoke

P/Invoke provides a standard mechanism for calling unmanaged DLL functions from managed .NET applications. This is important in projects where the computational or system-specific portion is implemented in native code, while the presentation layer is implemented in .NET. In this project, the **CoreEngine.dll** bridge exports C-style functions and simple structs so that the C# application can call the native C++ core safely.

### 2.2.3 C++ File-System Support

The C++ standard library provides `<filesystem>` for path handling, directory traversal, metadata access, and file operations. This makes it practical to implement directory listing, copy, move, delete, and recursive scanning in a modern C++ codebase. The current project makes extensive use of this capability in the C++ service layer.

### 2.2.4 Compression Technologies

Compression is an important utility in file management because users often need to reduce storage usage or package files for transfer. This project supports two different approaches:

- **Zstandard (.zst)** for single-file compression and decompression.
- **ZIP** archive creation and extraction for common file and folder packaging workflows.

Zstandard was selected because it is a modern and efficient compression format. ZIP support was included because it is widely recognized and expected by users.

### 2.2.5 Duplicate Detection by Hashing

Duplicate detection can be performed at different levels. Name-based grouping can reveal suspiciously repeated files, but it is not sufficient for content equivalence. Therefore, content-based duplicate detection is also included. The current project first groups files by size to reduce unnecessary work and then computes **SHA-256** hashes for same-size files. This approach balances correctness and efficiency.

### 2.2.6 Relation to Existing File-Management Tools

Conventional file managers such as Windows File Explorer primarily focus on browsing and organization. Separate utilities are typically used for archive management, duplicate cleanup, and deeper storage inspection. The main contribution of this project is not to replace every advanced system-level behavior of the operating system, but to combine commonly needed file-management and optimization tasks in a single academic desktop application with a modular architecture.

## 2.3 Relevance to the Present Project

The reviewed technologies directly inform the implementation choices of File Optimizer:

- WinForms provides the Windows desktop interface.
- P/Invoke connects C# and C++.
- `<filesystem>` supports file and folder operations.
- Zstandard and ZIP provide compression support.
- SHA-256 hashing supports duplicate detection.

Thus, the literature and technical background collectively support the architectural direction of the project.

<<<PAGEBREAK>>>

# CHAPTER 3 SYSTEM ANALYSIS

## 3.1 System Analysis

System analysis was carried out to identify what the application should do, what constraints it should operate within, and whether the project was feasible in terms of time, technology, and usability. Since the project aimed to be both practical and demonstrative, analysis focused on user-facing functionality as well as software architecture concerns.

## 3.1.1 Requirement Analysis

### Functional Requirements

| Requirement ID | Functional Requirement | Description |
| --- | --- | --- |
| FR-1 | Browse directories | The user shall be able to navigate through folders and view directory contents. |
| FR-2 | Open files and folders | The user shall be able to open files with associated applications and enter folders by double-clicking. |
| FR-3 | Search files/folders | The user shall be able to search the current directory view by name using token and partial-token matching. |
| FR-4 | Sort directory contents | The user shall be able to sort items by name, size, and modified date in ascending or descending order. |
| FR-5 | Copy and paste | The user shall be able to copy an item, store the copy intent, and paste it into another folder. |
| FR-6 | Cut and paste | The user shall be able to cut an item and move it to another folder through paste. |
| FR-7 | Rename items | The user shall be able to rename files and folders. |
| FR-8 | Create items | The user shall be able to create a new empty file or a new folder. |
| FR-9 | Delete items | The user shall be able to delete files and folders. |
| FR-10 | Compress and decompress | The user shall be able to compress files/folders and decompress supported archive types. |
| FR-11 | Detect duplicates | The user shall be able to identify duplicate files by name and by file content. |
| FR-12 | View analysis panel | The user shall be able to inspect storage usage and benchmark-style analysis results. |
| FR-13 | Use context menu | The user shall be able to access major operations through right-click. |
| FR-14 | Use keyboard shortcuts | The user shall be able to use shortcuts such as Ctrl+C, Ctrl+X, Ctrl+V, Ctrl+L, F2, Delete, Alt+Left, and Alt+Right. |

### Non-Functional Requirements

| Requirement ID | Non-Functional Requirement | Description |
| --- | --- | --- |
| NFR-1 | Usability | The interface should feel familiar to users of standard Windows file managers. |
| NFR-2 | Modularity | The UI layer and core logic should remain separated for maintainability. |
| NFR-3 | Performance | Common operations such as listing and sorting should respond within practical desktop usage expectations. |
| NFR-4 | Reliability | Invalid or inaccessible paths should be handled gracefully without crashing the application. |
| NFR-5 | Compatibility | The application should run on Windows and load the native C++ DLL from the C# interface. |
| NFR-6 | Extensibility | New core operations should be addable through the native bridge without redesigning the entire UI. |
| NFR-7 | Maintainability | Source files should be organized into clear modules with focused responsibilities. |

## 3.1.2 Feasibility Study

| Feasibility Type | Observation | Conclusion |
| --- | --- | --- |
| Technical Feasibility | The required technologies, namely C++, C#, WinForms, P/Invoke, filesystem APIs, hashing, and compression libraries, are available on the Windows platform. | Feasible |
| Economic Feasibility | The project can be built using academic and freely available tools such as Visual Studio Community and open documentation. | Feasible |
| Operational Feasibility | Users are already familiar with file-manager style interfaces, reducing the learning curve. | Feasible |
| Schedule Feasibility | The project can be completed incrementally by developing the core first and the UI later. | Feasible |
| Legal / Ethical Feasibility | The application operates on user-selected local files and does not introduce high-risk data practices in its current form. | Feasible |

## 3.1.3 Use Case Scenario Summary

| Use Case | Actor | Precondition | Main Outcome |
| --- | --- | --- | --- |
| Browse Files | User | Application is opened | Directory contents are displayed |
| Search Items | User | Valid folder is loaded | Matching files/folders are shown |
| Copy/Cut and Paste | User | An item is selected | Item is copied or moved to target folder |
| Compress Item | User | Valid file/folder selected | Archive is created |
| Decompress Item | User | Supported archive selected | Archive contents are extracted |
| Detect Duplicates | User | Valid analysis path selected | Duplicate groups are reported |
| View Storage Summary | User | Valid path selected | Folder/file count and size summary are shown |

**Placeholder:** Insert Figure 3.1 here: Use Case Diagram of File Optimizer

## 3.1.4 System Users

The main user of the system is a general desktop user who needs to manage files and also perform optimization-oriented tasks such as compression, duplicate checking, and folder analysis. The application is also suitable for academic demonstration because it clearly shows interaction between frontend and backend layers.

## 3.1.5 Development Timeline

The development timeline followed an incremental sequence:

1. Core C++ file-service features.
2. Search and sorting support.
3. Native DLL bridge.
4. C# WinForms interface.
5. Compression and archive utilities.
6. Duplicate detection and analysis panel.
7. Usability improvements such as clipboard behavior, context menus, and keyboard shortcuts.

**Placeholder:** Insert Figure 3.2 here: Gantt Chart / Development Timeline

<<<PAGEBREAK>>>

# CHAPTER 4 SYSTEM DESIGN

## 4.1 Overall System Design

The design of File Optimizer follows a layered architecture. Each layer has a clear responsibility, which improves maintainability and makes the project easier to explain, extend, and test.

### 4.1.1 Architectural Layers

**1. Presentation Layer**

The presentation layer is the **C# WinForms** application. It handles:

- window creation,
- buttons and menus,
- path input and search input,
- file grid display,
- dialogs for browse, save, and folder selection,
- status messages,
- analysis panel visualization,
- keyboard shortcuts and context-menu behavior.

**2. Interop Layer**

The interop layer is implemented through the `NativeMethods` class in the C# project and the exported API in `CoreEngine.dll`. This layer is responsible for:

- importing native functions using `DllImport`,
- marshaling strings and arrays between managed and unmanaged code,
- converting plain native structs into UI-friendly view models,
- returning user-readable error messages.

**3. Native Bridge Layer**

The `CoreEngine` project provides a narrow and safe bridge around the C++ core. It exposes plain C-style functions such as:

- `Core_GetDirectoryContents`
- `Core_SearchDirectoryContents`
- `Core_CopyPath`
- `Core_MovePath`
- `Core_CreateDirectory`
- `Core_FindDuplicateNames`
- `Core_FindDuplicateContents`
- `Core_CompressPath`
- `Core_DecompressPath`

This layer also performs UTF-8 and wide-string conversion so that the UI can use Unicode paths correctly.

**4. Core Service Layer**

The C++ service layer contains the main logic of the system:

- `FileService` for file-system operations,
- `FileSorter` for sorting,
- `SearchIndex` for token-based searching,
- `CompressionService` for compression/decompression dispatch,
- `DuplicateDetector` for duplicate grouping,
- `ZstdCompressor` for Zstandard operations.

**Placeholder:** Insert Figure 4.1 here: System Architecture Diagram

## 4.1.2 Component Description

| Component | Language | Responsibility |
| --- | --- | --- |
| FileOptimizer.UI | C# | Main desktop interface and user interaction |
| NativeMethods | C# | P/Invoke declarations and managed wrappers |
| CoreEngine | C++ | Native DLL bridge between UI and core modules |
| FileService | C++ | Browse, copy, move, delete, rename, create file/folder |
| FileSorter | C++ | Sort by selected key and order |
| SearchIndex | C++ | Build token index and search file names |
| CompressionService | C++ | Route requests to Zstd or ZIP implementations |
| DuplicateDetector | C++ | Find duplicate names and duplicate contents |
| AnalysisService | C# | Perform storage and benchmark-style analysis using existing backend functionality |

**Placeholder:** Insert Figure 4.2 here: Component Diagram

## 4.1.3 Design Rationale

The separation between C++ and C# was chosen deliberately.

- C++ is well suited for performance-sensitive file and system operations.
- C# WinForms is productive for building desktop interfaces quickly.
- A C-style bridge is more reliable for interop than exposing raw C++ classes directly to C#.
- The design allows future UI changes without rewriting the core logic.

## 4.2 Refinement of Functional Design

### 4.2.1 Browsing and Navigation

The application starts at a default folder path and loads directory contents through the native bridge. Folder navigation updates the address bar and refreshes the data grid. Navigation history is maintained so that the user can move backward and forward using keyboard shortcuts.

### 4.2.2 Search

Search is performed on the current directory listing. File names are tokenized by converting them to lowercase and splitting them on separators such as underscores, hyphens, and periods. Exact token matches are returned first, and partial token matches are then added, while duplicate results are prevented through path-based deduplication.

### 4.2.3 Copy, Cut, and Paste Workflow

The system uses an **internal clipboard** at the UI level. When the user chooses copy or cut, the item path, name, and action type are stored in memory. The actual operation is performed only when the user pastes into a destination folder. This design replicates the intent-based workflow of common file managers and cleanly separates UI state from file execution.

### 4.2.4 Duplicate Detection Workflow

Two types of duplicate detection are supported:

- **Name duplicate detection:** groups files recursively by exact filename.
- **Content duplicate detection:** groups files by size first, then computes SHA-256 for files of equal size, and finally groups matching hashes.

### 4.2.5 Compression Workflow

Compression requests are routed according to the selected output format:

- `.zst` uses the Zstandard compressor for regular files.
- `.zip` uses Windows PowerShell archive commands for files and folders.

Decompression also branches based on archive type:

- `.zst` returns a decompressed output file.
- `.zip` extracts contents into a destination folder.

### 4.2.6 Analysis Panel Workflow

The analysis panel is kept as a separate form to avoid cluttering the main browsing interface. It reuses backend operations and local traversal logic to provide:

- storage summary,
- search timing observations,
- sort timing observations,
- compression observations,
- duplicate name analysis,
- duplicate content analysis.

**Placeholder:** Insert Figure 4.3 here: Sequence Diagram for File Browsing and Search

**Placeholder:** Insert Figure 4.4 here: Activity Diagram for Copy-Cut-Paste Workflow

## 4.3 Algorithm Details

### 4.3.1 Directory Listing Algorithm

1. Receive a folder path from the UI.
2. Validate that the path exists and is a directory.
3. Traverse the directory using `std::filesystem::directory_iterator`.
4. Build a `FileInfo` object for each entry.
5. Attach name, path, size, type, and modified time.
6. Sort the result using the selected sort options.
7. Copy plain results into a native struct array for C#.

### 4.3.2 Search Algorithm

1. Load the current directory items.
2. Tokenize each file name into lowercase searchable units.
3. Build an index from token to matching file entries.
4. Tokenize the user query.
5. Add exact token matches first.
6. Add partial-token matches next.
7. Deduplicate using a path-based set.
8. Return the final ordered result list.

### 4.3.3 Sorting Algorithm

1. If directories-first mode is enabled, directory items are prioritized.
2. Compare items using the selected key:
   - name,
   - size,
   - modified time.
3. If primary comparison is equal, fall back to name.
4. Reverse the final comparison if descending order is selected.

### 4.3.4 Duplicate Detection Algorithm

**Name-based duplicates**

1. Recursively collect regular files.
2. Group them by exact filename using a hash table.
3. Return only groups with two or more files.

**Content-based duplicates**

1. Recursively collect regular files.
2. Group files by size.
3. Ignore size groups with only one file.
4. For remaining groups, compute SHA-256 for each file.
5. Group files by computed hash.
6. Return only hash groups with two or more files.

### 4.3.5 Clipboard-Based Copy/Cut Algorithm

1. Select item.
2. Store source path, item name, and action type in UI clipboard state.
3. Navigate to target folder.
4. Paste into the current directory.
5. If action is copy, call native copy.
6. If action is cut, call native move.
7. Refresh directory view and update status.

### 4.3.6 Compression Algorithm

1. Determine output format from user selection.
2. Validate supported path type and extension.
3. For `.zst`, compress file through native Zstandard logic.
4. For `.zip`, invoke PowerShell archive command through the native layer.
5. Return success or error message to the UI.

<<<PAGEBREAK>>>

# CHAPTER 5 IMPLEMENTATION AND TESTING

## 5.1 Implementation

The project was implemented as a multi-project Windows solution with separate responsibilities distributed across native and managed code.

### 5.1.1 Tools Used

| Tool / Technology | Purpose |
| --- | --- |
| Visual Studio | Project development, building, and debugging |
| C++ | Native core implementation |
| C# | Desktop user interface implementation |
| WinForms | Windows desktop GUI framework |
| .NET | Managed application runtime |
| P/Invoke | Calling native DLL functions from C# |
| C++ `<filesystem>` | Path and file-system handling |
| Zstandard library | `.zst` compression and decompression |
| Windows PowerShell Archive Cmdlets | ZIP compression and extraction |
| Windows CNG / BCrypt | SHA-256 hashing for duplicate detection |

### 5.1.2 Implementation Details of Modules

#### A. Main UI Module

The main UI is implemented in `Form1` and provides:

- address bar,
- browse and up navigation,
- search controls,
- sort controls,
- data grid for files and folders,
- toolbar actions,
- right-click context menu,
- keyboard shortcuts,
- clipboard state and navigation history.

#### B. Native Interop Module

`NativeMethods` contains the imported native functions and converts native structures into strongly typed C# view objects such as `FileItemView` and `DuplicateEntryView`. This module also centralizes error handling so that failures in the native layer can be shown to the user in a readable form.

#### C. Core File Service Module

`FileService` handles the following:

- listing directory contents,
- copy path,
- move path,
- rename path,
- delete path,
- create empty file,
- create directory.

The implementation relies on `std::filesystem`, making it suitable for both file and folder operations.

#### D. Search Module

`SearchIndex` builds a token-based index of filenames. Separators like `_`, `-`, and `.` are normalized into spaces. The search module supports exact token match as well as partial-token match, allowing queries such as `da` to match `dad_songs`.

#### E. Sorting Module

`FileSorter` sorts items according to selected sort criteria and respects:

- sort key,
- ascending/descending order,
- directories-first mode.

#### F. Compression Module

`CompressionService` serves as a format-aware dispatcher. It delegates `.zst` requests to the Zstandard compressor and `.zip` requests to PowerShell archive commands. This allows the user interface to present a unified compression workflow even though the underlying implementations are different.

#### G. Duplicate Detection Module

`DuplicateDetector` recursively scans files under a selected path while skipping inaccessible or problematic entries. It supports:

- exact filename grouping,
- size-prefiltered SHA-256 content grouping.

#### H. Analysis Module

`AnalysisService` is implemented in the C# layer and reuses the backend to produce:

- storage summary,
- search benchmark observations,
- sort benchmark observations,
- compression benchmark observations,
- duplicate name analysis,
- duplicate content analysis.

## 5.2 Testing

Testing for this project is divided into unit-level feature verification and system-level workflow validation. At the time of writing, the tables below are prepared for execution and recording. They can be used directly during final testing and viva preparation.

### 5.2.1 Test Cases for Unit Testing

| Test ID | Module | Test Scenario | Expected Result | Actual Result | Status |
| --- | --- | --- | --- | --- | --- |
| UT-1 | FileService | Load contents of a valid directory | Files and folders are returned with correct metadata | To be recorded | Pending |
| UT-2 | FileService | Create new empty file | New file appears in selected folder | To be recorded | Pending |
| UT-3 | FileService | Create new folder | New folder appears in selected folder | To be recorded | Pending |
| UT-4 | FileService | Rename selected item | Item name changes correctly | To be recorded | Pending |
| UT-5 | FileService | Copy file to destination | New file appears at destination | To be recorded | Pending |
| UT-6 | FileService | Move folder to destination | Folder is removed from source and appears at destination | To be recorded | Pending |
| UT-7 | SearchIndex | Search exact token | Matching item is returned | To be recorded | Pending |
| UT-8 | SearchIndex | Search partial token | Partial query returns matching item | To be recorded | Pending |
| UT-9 | FileSorter | Sort by size ascending | Items appear in ascending size order | To be recorded | Pending |
| UT-10 | CompressionService | Compress file to `.zst` | Output archive is created | To be recorded | Pending |
| UT-11 | CompressionService | Compress folder to `.zip` | ZIP archive is created | To be recorded | Pending |
| UT-12 | CompressionService | Decompress `.zip` archive | Contents are extracted to destination folder | To be recorded | Pending |
| UT-13 | DuplicateDetector | Find duplicate names | Duplicate filename groups are returned | To be recorded | Pending |
| UT-14 | DuplicateDetector | Find duplicate contents | Identical file-content groups are returned | To be recorded | Pending |

### 5.2.2 Test Cases for System Testing

| Test ID | Workflow | Input / Action | Expected Result | Actual Result | Status |
| --- | --- | --- | --- | --- | --- |
| ST-1 | Browse and open | Navigate to folder and double-click file | File opens in associated application | To be recorded | Pending |
| ST-2 | Search and sort | Search current directory and apply sort | Matching items are displayed in selected order | To be recorded | Pending |
| ST-3 | Copy and paste | Copy file, navigate, paste | File appears in target folder without source removal | To be recorded | Pending |
| ST-4 | Cut and paste | Cut folder, navigate, paste | Folder moves to target folder | To be recorded | Pending |
| ST-5 | Context menu operations | Right-click selected item and rename/delete | Operation completes successfully | To be recorded | Pending |
| ST-6 | Keyboard shortcuts | Use Ctrl+C, Ctrl+X, Ctrl+V, F2, Delete | Equivalent UI actions are triggered | To be recorded | Pending |
| ST-7 | Navigation shortcuts | Use Alt+Left, Alt+Right, Ctrl+L | Back/forward history and address focus work | To be recorded | Pending |
| ST-8 | Compression workflow | Compress selected file/folder | Archive is produced in requested format | To be recorded | Pending |
| ST-9 | Decompression workflow | Decompress selected `.zst` or `.zip` | Data is restored to chosen destination | To be recorded | Pending |
| ST-10 | Duplicate analysis | Run duplicate-name and content checks | Duplicate groups are shown in analysis panel | To be recorded | Pending |
| ST-11 | Storage analysis | Run storage summary on folder tree | Folder count, file count, size, and scan time are shown | To be recorded | Pending |

## 5.3 Result Analysis

At the present stage, the project has been implemented feature-complete for the targeted scope. The system successfully demonstrates:

- a working multi-language architecture,
- standard file-manager operations,
- archive and optimization utilities,
- duplicate detection,
- and a separate analysis panel.

The application also reflects a realistic desktop software design in which the user interface does not directly depend on internal C++ classes. Instead, the bridge layer provides a stable API boundary, making the system easier to explain and extend.

Because formal test execution and measured benchmarking are still pending, the following observation tables are prepared for direct use during final testing. These tables should be filled after running the application on selected folders and files.

### 5.3.1 Search Performance Observation Table

| Test Case | Folder Path | Query | Number of Items | Iterations | Average Time (ms) | Result Count | Remark |
| --- | --- | --- | --- | --- | --- | --- | --- |
| SP-1 | ____________________ | doc | ____________________ | 10 | To be recorded | To be recorded | Exact token search |
| SP-2 | ____________________ | da | ____________________ | 10 | To be recorded | To be recorded | Partial token search |
| SP-3 | ____________________ | project | ____________________ | 10 | To be recorded | To be recorded | Mixed directory contents |

### 5.3.2 Sort Performance Observation Table

| Test Case | Folder Path | Sort Key | Item Count | Iterations | Average Time (ms) | Remark |
| --- | --- | --- | --- | --- | --- | --- |
| SO-1 | ____________________ | Name | To be recorded | 10 | To be recorded | Ascending |
| SO-2 | ____________________ | Size | To be recorded | 10 | To be recorded | Ascending |
| SO-3 | ____________________ | Modified Time | To be recorded | 10 | To be recorded | Ascending |

### 5.3.3 Compression and Storage Analysis Table

| Test Case | Source Type | Source Path | Original Size | Output Format | Compressed Size | Compression Ratio | Time (ms) | Remark |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| CP-1 | File | ____________________ | To be recorded | Zstd | To be recorded | To be recorded | To be recorded | Single-file compression |
| CP-2 | File | ____________________ | To be recorded | ZIP | To be recorded | To be recorded | To be recorded | ZIP file compression |
| CP-3 | Folder | ____________________ | To be recorded | ZIP | To be recorded | To be recorded | To be recorded | Folder compression |

| Storage Case | Analysis Path | Folder Count | File Count | Total Size | Scan Time (ms) | Remark |
| --- | --- | --- | --- | --- | --- | --- |
| SA-1 | ____________________ | To be recorded | To be recorded | To be recorded | To be recorded | General folder summary |
| SA-2 | ____________________ | To be recorded | To be recorded | To be recorded | To be recorded | Large dataset summary |

### 5.3.4 Duplicate Detection Analysis Table

| Test Case | Analysis Path | Duplicate Type | Total Groups | Files in Groups | Scan Time (ms) | Remark |
| --- | --- | --- | --- | --- | --- | --- |
| DD-1 | ____________________ | Name-based | To be recorded | To be recorded | To be recorded | Exact filename grouping |
| DD-2 | ____________________ | Content-based | To be recorded | To be recorded | To be recorded | Size + SHA-256 hash |

## 5.4 Screens and Demonstration Notes

The final report should include screenshots of the following screens after the UI is finalized for submission:

- Main file manager window
- Search and sort in action
- Context menu and rename dialog
- Compression/decompression workflow
- Analysis panel
- Duplicate detection result output

**Placeholder:** Insert Figure 5.1 here: Main Window of File Optimizer

**Placeholder:** Insert Figure 5.2 here: Analysis Panel Window

<<<PAGEBREAK>>>

# CHAPTER 6 CONCLUSION AND FUTURE RECOMMENDATIONS

## 6.1 Conclusion

This project successfully demonstrates the design and implementation of a **desktop file manager and optimization system** using a mixed-language architecture. The system combines a **C++ native core** with a **C# WinForms interface**, connected through a dedicated DLL bridge and P/Invoke. This architecture separates presentation concerns from core file-processing logic and reflects a practical software-engineering approach.

The application goes beyond basic file browsing by including:

- search and sorting,
- file and folder creation,
- rename and deletion,
- clipboard-based copy, cut, and paste,
- ZIP and Zstandard compression support,
- duplicate detection by name and content,
- storage and performance analysis,
- right-click menus and keyboard shortcuts.

As a result, the system can be presented not only as a file manager, but as an **alternative desktop file-management and optimization tool** with additional monitoring and analysis features. The project also provided valuable academic learning in desktop UI design, native-managed interoperability, filesystem programming, hashing, archive handling, and modular project organization.

## 6.2 Future Recommendations

Although the current implementation satisfies the major project goals, several improvements can be made in future versions:

- Add multi-selection for copy, cut, delete, and compression.
- Add drag-and-drop support.
- Add conflict-resolution dialogs for paste and overwrite cases.
- Add recursive indexed search across larger folder trees.
- Add more archive formats beyond ZIP and Zstandard.
- Improve the analysis panel with charts, export to CSV/PDF, and saved reports.
- Add a detailed properties panel for selected items.
- Support streaming compression for very large files.
- Add automated test execution and benchmark logging.
- Improve duplicate handling with actions such as open, reveal in folder, and delete selected duplicate.

With these enhancements, the project can evolve from a strong academic submission into an even more capable desktop utility.

<<<PAGEBREAK>>>

# REFERENCES

1. Microsoft Learn. *Windows Forms Overview*. Available at: https://learn.microsoft.com/en-us/dotnet/desktop/winforms/overview/
2. Microsoft Learn. *DllImportAttribute Class (System.Runtime.InteropServices)*. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.dllimportattribute?view=net-9.0
3. Microsoft Learn. *<filesystem>*. Available at: https://learn.microsoft.com/en-us/cpp/standard-library/filesystem?view=msvc-170
4. Zstandard Official Documentation. *zstd 1.5.7 Manual*. Available at: https://facebook.github.io/zstd/doc/api_manual_v1.5.7.html
5. Microsoft Learn. *Compress-Archive (Microsoft.PowerShell.Archive)*. Available at: https://learn.microsoft.com/id-id/powershell/module/microsoft.powershell.archive/compress-archive?view=powershell-7.5
6. Microsoft Learn. *Expand-Archive (Microsoft.PowerShell.Archive)*. Available at: https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.archive/expand-archive?view=powershell-5.1
7. Microsoft Learn. *BCryptOpenAlgorithmProvider function (bcrypt.h)*. Available at: https://learn.microsoft.com/en-us/windows/win32/api/bcrypt/nf-bcrypt-bcryptopenalgorithmprovider

<<<PAGEBREAK>>>

# APPENDICES

## Appendix A: Project Structure

- `File-Optimizer/`
  Native C++ core project containing file services, sorting, search, compression, and duplicate detection.
- `CoreEngine/`
  Native DLL bridge exposing C-style functions for the C# UI.
- `FileOptimizer.UI/`
  WinForms desktop application with the main file manager and analysis panel.

## Appendix B: Major Implemented Features

- Browse directory contents
- Open files with associated applications
- Search by token and partial token
- Sort by name, size, and modified date
- Copy, cut, and paste with internal clipboard
- Rename items
- Create new files and folders
- Delete files and folders
- Right-click context menu
- Keyboard shortcuts and navigation history
- Compress to Zstandard and ZIP
- Decompress Zstandard and ZIP
- Detect duplicates by name and content
- View storage and performance-oriented analysis

## Appendix C: Suggested Screenshot Insertions

| Appendix Figure | Description | Status |
| --- | --- | --- |
| A-1 | Cover screenshot of main dashboard | To be inserted |
| A-2 | Search and sort example | To be inserted |
| A-3 | Compression example | To be inserted |
| A-4 | Duplicate detection output | To be inserted |
| A-5 | Analysis panel | To be inserted |
