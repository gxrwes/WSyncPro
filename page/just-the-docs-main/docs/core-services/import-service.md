
---
title: Import Service
parent: Core Services Overview
nav_order: 4
---

# Import Service

The `ImportService` facilitates importing files and data into the system. It ensures compatibility and handles files based on user-defined filters and path-building rules.

## Key Responsibilities

- Scan directories to locate files matching specified criteria.
- Generate destination paths dynamically for imported files.
- Create copy jobs for transferring files from source to destination.
- Execute import tasks with validation and logging.

## Implementation Details

- **Files**: `ImportService.cs`, `IImportService.cs`
- **Namespace**: `WSyncPro.Core.Services`
- **Key Dependencies**:
  - `ICopyService`: Executes copy jobs generated during the import process.
  - `ILogger<ImportService>`: Logs the progress and errors during import operations.

## Public Methods

### GetFoundFiles
Scans a directory and retrieves files matching the specified filter parameters.

```csharp
public Task<WDirectory> GetFoundFiles(string path, FilterParams filterParams);
```

**Parameters**:
- `string path`: The directory to scan.
- `FilterParams filterParams`: Filtering criteria to include or exclude files.

**Returns**:
- A `WDirectory` object containing the filtered files.

### GenerateDstPathForfile
Generates the destination path for a file based on path-building rules.

```csharp
public Task<string> GenerateDstPathForfile(WFile file, List<ImportPathType> pathBuilder);
```

**Parameters**:
- `WFile file`: The file for which the destination path is generated.
- `List<ImportPathType> pathBuilder`: Rules for building the path.

**Returns**:
- A string representing the destination path.

### CreateCopyJobsFromDirectory
Creates copy jobs for files in a source directory to be imported into a destination directory.

```csharp
public Task<List<CopyJob>> CreateCopyJobsFromDirectory(WDirectory src, string importDirectory, List<ImportPathType> pathBuilder);
```

**Parameters**:
- `WDirectory src`: The source directory containing files to be imported.
- `string importDirectory`: The target directory for imported files.
- `List<ImportPathType> pathBuilder`: Rules for building the paths.

**Returns**:
- A list of `CopyJob` objects representing the import operations.

### RunImport
Executes the import process for a given source and destination directory.

```csharp
public Task<bool> RunImport(string srcPath, string dstPath, FilterParams filterParams, List<ImportPathType> pathBuilder);
```

**Parameters**:
- `string srcPath`: The source directory.
- `string dstPath`: The destination directory.
- `FilterParams filterParams`: Filtering criteria.
- `List<ImportPathType> pathBuilder`: Rules for building destination paths.

**Returns**:
- A boolean indicating the success or failure of the operation.

## Workflow

1. **Directory Scanning**:
   - The `GetFoundFiles` method scans the source directory and filters files based on user-defined criteria.

2. **Path Generation**:
   - The `GenerateDstPathForfile` method determines where each file will be imported in the destination directory.

3. **Copy Job Creation**:
   - The `CreateCopyJobsFromDirectory` method prepares the necessary copy jobs for the import operation.

4. **Execution**:
   - The `RunImport` method executes the copy jobs and logs the process.

## Example Usage

```csharp
var importService = new ImportService(logger, copyService);
var filterParams = new FilterParams
{
    Include = new List<string> { "*.jpg", "*.png" },
    Exclude = new List<string> { "temp*" }
};
var pathBuilder = new List<ImportPathType>
{
    ImportPathType.DateCreated,
    ImportPathType.FileExtension
};
bool success = await importService.RunImport("C:\Source", "D:\Destination", filterParams, pathBuilder);
```

