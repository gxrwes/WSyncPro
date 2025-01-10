---
title: AppCache Service
parent: Core Services Overview
nav_order: 1
---

# AppCache Service

The AppCache service provides an in-memory caching mechanism to store and retrieve frequently accessed data, reducing database lookups and enhancing performance.

---

## Files
- **Implementation**: `AppCache.cs`
- **Interface**: `IAppCache.cs`

## Description
The AppCache is responsible for:
- Storing and managing in-memory data.
- Syncing cache with the local database.
- Providing helper methods for managing jobs, directories, and file history snapshots.

## Key Methods
- `AddSyncJob(SyncJob job)`: Adds a new synchronization job.
- `RemoveSyncJob(string jobId)`: Removes a synchronization job by ID.
- `UpdateSyncJob(SyncJob job)`: Updates an existing synchronization job.
- `SyncWithDb()`: Synchronizes the in-memory cache with the local database.
- `GetAllSyncJobs()`: Retrieves all sync jobs from the cache.
- `AddFileHistorySnapshot(FileHistorySnapShot snapshot)`: Adds a snapshot of file history to the cache.

## Dependencies
- **IAppLocalDb**: Used for database interactions.
- **ILogger<AppCache>**: For logging operations.

## Usage Example
```csharp
var appCache = new AppCache(localDb, logger);
await appCache.AddSyncJob(new SyncJob { Id = Guid.NewGuid(), Name = "Sample Job" });
var jobs = await appCache.GetAllSyncJobs();
```

For further details, refer to the [AppCache Implementation](./AppCache.md).
