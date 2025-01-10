---
title: Home
layout: home
nav_order: 1
description: "WSyncPro is a versatile file synchronization, archiving, and management application designed to handle a wide variety of file operations across different storage devices and directories."
permalink: /
---

# WSyncPro
{: .fs-9 }

WSyncPro is a versatile file synchronization, archiving, and management application designed to handle a wide variety of file operations across different storage devices and directories.
{: .fs-6 .fw-300 }

[Get started now](/docs/configuration){: .btn .btn-primary .fs-5 .mb-4 .mb-md-0 .mr-2 }
[View Features](/docs/features){: .btn .fs-5 .mb-4 .mb-md-0 }
[View it on GitHub](https://github.com/gxrwes/WSyncPro){: .btn .fs-5 .mb-4 .mb-md-0 }

{: .warning }
> **Work in Progress**  
> This project is still under development. The main import and sync functionality are working, but the UI and reporting features are far from complete.


---

## Features

- **File Management**: Comprehensive services for copying, moving, and cleaning files based on user-defined criteria.
- **Synchronization**: Sync files between directories or external devices with ease.
- **Archiving**: Archive files into zip files with customizable grouping patterns.
- **State Management**: Persist and manage application state across sessions.
- **External Device Detection**: Detect and interact with external USB devices.
- **Data Import**: Efficiently import files with advanced filtering options.
- **User-Friendly Interface**: Intuitive UI with a dark mode theme and live job tracking.
- **Cross-Platform Support**: Fully compatible with Windows and Linux systems.

## Repository Structure

This repository is organized into several projects, each serving a distinct purpose within the WSyncPro ecosystem:

- **[WSyncPro.Core](./WSyncPro.Core/README.md)**: Contains the core business logic, services, and models used by the WSyncPro application.
- **[WSyncPro.Tests](./WSyncPro.Tests/README.md)**: Contains the unit tests for the WSyncPro.Core project, ensuring that the core services work correctly under various scenarios.
- **[WSyncPro.Models](./WSyncPro.Data/README.md)**: Contains shared models and classes mainly used for data structures.
- **[WSyncPro.App](./WSyncPro.UI/README.md)**: The Maui Blazor Hybrid App for Frontend.

---

Explore the detailed [Features](/docs/features) or get started with [Configuration](/docs/configuration).
