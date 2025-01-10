---
title: Getting Started
nav_order: 2
---

# Getting Started
{: .no_toc }

This section guides you through the initial configuration and setup of your WSyncPro project.
{: .fs-6 .fw-300 }

## Table of Contents
{: .no_toc .text-delta }

1. TOC
{:toc}

---
## The Easy Way (Windows)
Check out the latest release on the github and download the zip. After extracting to your desired location run the .exe.


## Getting Started

### Prerequisites

- .NET 8.0 or later
- HandBrakeCLI (for re-rendering functionality)


### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/your-repo/WSyncPro.git
   ```

2. Restore dependencies for all projects:

   ```bash
   dotnet restore
   ```

### Building the Application

To build the entire solution, navigate to the root of the repository and run:

```bash
dotnet build
```

### Running the Application

To run the UI application, use:

```bash
cd WSyncPro/WSyncPro.UI
dotnet run
```

### Running Tests

To execute all unit tests, navigate to the `WSyncPro.Tests` directory and run:

```bash
dotnet test
```

This will run all tests and provide a report on the results.

#### Building Via CMD
If the .exe that is created is not runnable, use this command to build instead:
```bash
dotnet publish -c Release -f net8.0-windows10.0.19041.0 -p:WindowsPackageType=None
```

---


