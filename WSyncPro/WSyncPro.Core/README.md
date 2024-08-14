# WSyncPro.Core

WSyncPro.Core is the core library of the WSyncPro project, containing essential business logic and services that drive the application's functionality. It includes services for file management, archiving, synchronization, and more.

## Features

- **File Management Services**: Copy, move, and clean files based on various criteria.
- **Synchronization Services**: Sync files between directories or external devices.
- **Archive Services**: Archive files with customizable grouping patterns.
- **State Management**: Persist and manage application state across sessions.
- **External Device Detection**: Detect and interact with external USB devices.
- **ReRender Services**: Re-render video files using HandBrakeCLI.

## Services

- **CopyService**: Handles copying files with options for overwriting and directory structure retention.
- **MoveService**: Manages moving files with similar options as the CopyService.
- **CleanService**: Moves files to a trash directory based on job criteria.
- **ArchiveService**: Archives files into zip files based on job specifications.
- **ReRenderService**: Uses HandBrakeCLI to re-render video files.
- **ExternalDeviceDetectorService**: Detects and lists external USB devices.

## Getting Started

### Prerequisites

- .NET 7.0 or later
- HandBrakeCLI for ReRenderService functionality

### Installation

1. Clone the repository:

   ```
   git clone https://github.com/your-repo/WSyncPro.git
   ```

2. Navigate to the WSyncPro.Core directory:

   ```
   cd WSyncPro/WSyncPro.Core
   ```

3. Restore dependencies:

   ```
   dotnet restore
   ```

### Usage

To use the services provided by WSyncPro.Core in your application, you can reference the library and instantiate the necessary services. For example, to use the `CopyService`:

```
var copyService = new CopyService();
copyService.CopyFiles(sourceDirectory, job, FileOverwriteOptions.ALWAYS, true);
```

### Testing

Unit tests for WSyncPro.Core are located in the `WSyncPro.Tests` project.

```
dotnet test
```

## License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.
