---
title: Core Services Overview
nav_order: 5
---

# Core Services Overview

This document provides an overview of the core services implemented in the WSyncPro project. Each service plays a distinct role in ensuring efficient and reliable file operations, synchronization, and application management.

---

## Services List

### 1. **AppCache**
- **Files**: `AppCache.cs`, `IAppCache.cs`
- **Description**: Provides caching mechanisms to store and retrieve frequently used data, reducing redundant computations and database lookups.

### 2. **AppLocalDb**
- **Files**: `AppLocalDb.cs`, `IAppLocalDb.cs`
- **Description**: Manages the local database for the application, handling CRUD operations and ensuring data persistence.

### 3. **CopyService**
- **Files**: `CopyService.cs`, `ICopyService.cs`
- **Description**: Handles file copying operations across directories and devices. Includes features for validation, conflict resolution, and performance optimization.

### 4. **FileVersioning**
- **Files**: `FileVersioning.cs`, `IFileVersioning.cs`
- **Description**: Provides functionality for managing file versions, ensuring integrity and tracking changes over time.

### 5. **ImportService**
- **Files**: `ImportService.cs`, `IImportService.cs`
- **Description**: Facilitates importing files and data into the system, ensuring compatibility and proper handling of imported content.

### 6. **SyncService**
- **Files**: `SyncService.cs`, `ISyncService.cs`
- **Description**: Coordinates file synchronization operations, ensuring consistency between source and target directories or devices. Supports conflict handling, scheduling, and logging.

---

## Core Functionality

### **Caching**
The `AppCache` service ensures that frequently accessed data is stored in memory, improving performance by avoiding repeated calculations or database queries.

### **Local Database Management**
The `AppLocalDb` service provides lightweight database operations, ensuring persistent storage and quick retrieval of essential data.

### **File Operations**
The `CopyService` and `FileVersioning` services enable robust file management, offering features like safe copying, backup, and version tracking.

### **Data Import**
The `ImportService` supports importing various types of data, ensuring compatibility and providing feedback on the import status.

### **Synchronization**
The `SyncService` handles bidirectional synchronization, ensuring that files remain up-to-date between different systems while managing conflicts effectively.

---

## Dependency Flow

1. **SyncService** relies on:
   - `AppCache` for caching sync-related data.
   - `AppLocalDb` for retrieving sync configurations.
   - `CopyService` for transferring files.

2. **ImportService** integrates with:
   - `AppLocalDb` to store imported data.
   - `FileVersioning` to manage imported file versions.

3. **FileVersioning** uses:
   - `AppCache` to cache version metadata.

---

## Next Steps

Refer to the detailed documentation of each service for in-depth understanding and implementation details:

- [AppCache Documentation](./AppCache.md)
- [AppLocalDb Documentation](./AppLocalDb.md)
- [CopyService Documentation](./CopyService.md)
- [FileVersioning Documentation](./FileVersioning.md)
- [ImportService Documentation](./ImportService.md)
- [SyncService Documentation](./SyncService.md)
