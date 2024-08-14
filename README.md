# WSyncPro

WSyncPro is a versatile file synchronization, archiving, and management application designed to handle a wide variety of file operations across different storage devices and directories. This repository contains all the necessary components to build, test, and run the WSyncPro application.

## Repository Structure

This repository is organized into several projects, each serving a distinct purpose within the WSyncPro ecosystem:

- **[WSyncPro.Core](./WSyncPro.Core/README.md)**: Contains the core business logic, services, and models used by the WSyncPro application. It includes services for file management, synchronization, archiving, re-rendering, and more.
- **[WSyncPro.Tests](./WSyncPro.Tests/README.md)**: Contains the unit tests for the WSyncPro.Core project, ensuring that the core services work correctly under various scenarios.
- **[WSyncPro.Data](./WSyncPro.Data/README.md)**: Handles data access and storage, including database contexts and repository patterns for managing application data.
- **[WSyncPro.UI](./WSyncPro.UI/README.md)**: The user interface layer of the WSyncPro application, providing an interactive interface for users to manage their file operations.

## Features

- **File Management**: Comprehensive services for copying, moving, and cleaning files based on user-defined criteria.
- **Synchronization**: Sync files between directories or external devices with ease.
- **Archiving**: Archive files into zip files with customizable grouping patterns.
- **State Management**: Persist and manage application state across sessions.
- **External Device Detection**: Detect and interact with external USB devices.
- **ReRender Services**: Re-render video files using HandBrakeCLI.
- **Data Management**: Interface with the application's data storage systems.
- **User Interface**: A user-friendly UI for managing all file operations.

## Getting Started

### Prerequisites

- .NET 7.0 or later
- HandBrakeCLI (for re-rendering functionality)

### Installation

1. Clone the repository:

   ```
   git clone https://github.com/your-repo/WSyncPro.git
   ```

2. Restore dependencies for all projects:

   ```
   dotnet restore
   ```

### Building the Application

To build the entire solution, navigate to the root of the repository and run:

```
dotnet build
```

### Running the Application

To run the UI application, use:

```
cd WSyncPro/WSyncPro.UI
dotnet run
```

### Running Tests

To execute all unit tests, navigate to the `WSyncPro.Tests` directory and run:

```
dotnet test
```

This will run all tests and provide a report on the results.

## Contributing

Contributions are welcome! Please fork the repository, create a new branch, and submit a pull request with your changes. Ensure that all new features and bug fixes are covered by appropriate unit tests.

## License

This project is licensed under the MIT License - see the [LICENSE](./LICENSE) file for details.

## Additional Documentation

For more detailed information about each component of the WSyncPro application, refer to the individual README files in the respective project directories:

- [WSyncPro.Core](./WSyncPro.Core/README.md)
- [WSyncPro.Tests](./WSyncPro.Tests/README.md)
- [WSyncPro.Data](./WSyncPro.Data/README.md)
- [WSyncPro.UI](./WSyncPro.UI/README.md)
