---
title: Copy Service
parent: Core Services Overview
nav_order: 2
---

# Copy Service

The `CopyService` is responsible for handling file copying and moving operations across directories and devices. It includes features for validation, conflict resolution, and performance optimization.

## Key Responsibilities

- Safely copy files from a source to a destination directory.
- Handle file overwriting with proper versioning.
- Maintain logs of copy operations and potential errors.
- Perform file move operations with similar validation and logging mechanisms.

## Implementation Details

- **Files**: `CopyService.cs`, `ICopyService.cs`
- **Namespace**: `WSyncPro.Core.Services`
- **Key Dependencies**:
  - `IFileVersioning`: Compares and maintains version histories for files.
  - `IAppCache`: Caches copy job metadata and maintains application state.
  - `ILogger<CopyService>`: Logs the progress and errors during copy operations.

## Public Methods

### CopyFile
Copies a file from the source to the destination, ensuring proper validation.

```csharp
public Task CopyFile(CopyJob copyJob);
```

**Parameters**:
- `CopyJob`: Details of the file to be copied, including source and destination paths.

**Example**:
```csharp
var copyJob = new CopyJob
{
    SrcFilePathAbsolute = "C:\Source\file.txt",
    DstFilePathAbsolute = "D:\Destination\file.txt"
};
await copyService.CopyFile(copyJob);
```

### MoveFile
Moves a file from the source to the destination with validation and logging.

```csharp
public Task MoveFile(CopyJob moveJob);
```

**Parameters**:
- `CopyJob`: Details of the file to be moved.
