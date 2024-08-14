# WSyncPro.Tests

WSyncPro.Tests contains unit tests for the WSyncPro.Core project. It ensures that the business logic and services within the core library function correctly under various conditions.

## Structure

The tests are organized by service, with each service's tests residing in a separate file:

- `CopyServiceTests.cs`
- `MoveServiceTests.cs`
- `CleanServiceTests.cs`
- `ArchiveServiceTests.cs`
- `ReRenderServiceTests.cs`
- `ExternalDeviceDetectorServiceTests.cs`

## Getting Started

### Prerequisites

- .NET 7.0 or later

### Installation

1. Clone the repository:

   ```
   git clone https://github.com/your-repo/WSyncPro.git
   ```

2. Navigate to the WSyncPro.Tests directory:

   ```
   cd WSyncPro/WSyncPro.Tests
   ```

3. Restore dependencies:

   ```
   dotnet restore
   ```

### Running Tests

To run all tests, use the following command:

```
dotnet test
```

This will execute all the unit tests and provide a report on the results.

### Writing Tests

When adding new functionality to WSyncPro.Core, please ensure corresponding unit tests are added in this project. Use `Xunit` for test creation.

### Continuous Integration

The project is set up to run tests automatically on each push or pull request via a CI pipeline. Ensure that all tests pass before merging to the main branch.

## License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.
