# WSyncPro.UI

WSyncPro.UI is the user interface layer of the WSyncPro application. It provides an interactive interface for users to manage their file synchronization, archiving, and other tasks.

## Features

- **User Interface**: Intuitive UI for managing file operations.
- **Job Management**: Create, edit, and delete jobs for syncing, archiving, and more.
- **Real-Time Feedback**: Monitor the progress of file operations.

## Getting Started

### Prerequisites

- .NET 7.0 or later
- A compatible operating system (e.g., Windows 10/11)

### Installation

1. Clone the repository:

   ```
   git clone https://github.com/your-repo/WSyncPro.git
   ```

2. Navigate to the WSyncPro.UI directory:

   ```
   cd WSyncPro/WSyncPro.UI
   ```

3. Restore dependencies:

   ```
   dotnet restore
   ```

### Usage

To start the UI application, run:

```
dotnet run
```

This will launch the user interface, where you can interact with the application's features.

### Building

To create a release build of the UI, use:

```
dotnet build --configuration Release
```

### Testing

Ensure that all core functionalities are thoroughly tested before release. You can write UI tests if necessary, depending on the framework used.

## License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.
