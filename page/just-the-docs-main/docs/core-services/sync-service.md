---
title: Sync Service
parent: Core Services Overview
nav_order: 3
---

# Sync Service

The `SyncService` coordinates file synchronization operations, ensuring consistency between source and target directories or devices. It supports conflict handling, scheduling, and logging.

## Key Responsibilities

- Scan directories to identify differences between source and destination.
- Generate copy jobs for files that need to be synchronized.
- Execute synchronization tasks with error handling and logging.
- Maintain consistency between directories while respecting file filters.

## Implementation Details

- **Files**: `SyncService.cs`, `ISyncService.cs`
- **Namespace**: `WSyncPro.Core.Services`
- **Key Dependencies**:
  - `IAppCache`: Caches sync-related data and manages state.
  - `ICopyService`: Handles the file copying operations during synchronization.
  - `IFileVersioning`: Tracks and compares file versions to prevent unnecessary transfers.
  - `ILogger<SyncService>`: Logs synchronization progress and errors.

## Public Methods

### CreateCpJobsForSyncJob
Generates copy jobs for a single synchronization job.

```csharp
public Task<List<CopyJob>> CreateCpJobsForSyncJob(SyncJob job);
```

**Parameters**:
- `SyncJob`: Details of the synchronization task, including source and destination paths.

**Returns**:
- A list of `CopyJob` objects representing the files to be synchronized.

### CreateCpJobsForSyncJobs
Generates copy jobs for a list of synchronization jobs.

```csharp
public Task<List<CopyJob>> CreateCpJobsForSyncJobs(List<SyncJob> jobs);
```

**Parameters**:
- `List<SyncJob>`: A list of synchronization jobs.

**Returns**:
- A consolidated list of `CopyJob` objects for all jobs.

### ExecuteAndVerifyJobs
Executes a list of copy jobs and verifies their success.

```csharp
public Task<List<bool>> ExecuteAndVerifyJobs(List<CopyJob> jobs);
```

**Parameters**:
- `List<CopyJob>`: A list of copy jobs to execute.

**Returns**:
- A list of booleans indicating the success or failure of each job.

### ScanDirectoryAsync
Recursively scans a directory and returns its contents as a `WDirectory` object.

```csharp
public Task<WDirectory> ScanDirectoryAsync(string path);
```

**Parameters**:
- `string path`: The directory path to scan.

**Returns**:
- A `WDirectory` object representing the scanned directory structure.

## Workflow

1. **Directory Scanning**:
   - The `ScanDirectoryAsync` method scans the source and target directories to identify differences.

2. **Job Generation**:
   - The `CreateCpJobsForSyncJob` method generates copy jobs for missing or updated files.

3. **Execution and Verification**:
   - The `ExecuteAndVerifyJobs` method performs the copy operations and validates their success.

4. **Logging**:
   - Progress and errors are logged throughout the synchronization process.

## Example Usage

```csharp
var syncService = new SyncService(appCache, copyService, fileVersioning, logger);
var syncJob = new SyncJob
{
    SrcDirectory = "C:\Source",
    DstDirectory = "D:\Destination",
    FilterParams = new FilterParams
    {
        Include = new List<string> { "*.txt" },
        Exclude = new List<string> { "temp*" }
    }
};
var copyJobs = await syncService.CreateCpJobsForSyncJob(syncJob);
var results = await syncService.ExecuteAndVerifyJobs(copyJobs);
```
