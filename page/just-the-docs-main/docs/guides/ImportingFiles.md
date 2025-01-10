---
title: Importing Files
parent: Guides
nav_order: 3
---

# Importing Files

This guide explains how to import files into WSyncPro using the **Import Service**. The import process allows you to transfer files from a source directory to a destination with advanced filtering and path-building options.

## Key Concepts

### 1. Source and Destination
- **Source Directory**: The folder containing files to be imported.
- **Destination Directory**: The folder where the files will be copied.

### 2. Path Building
Path-building rules help organize imported files in the destination directory based on file attributes such as name, extension, or creation date.

### 3. Wildcard Filtering
Filters define which files to include or exclude during the import process:

- **Include Filters**: Specifies patterns to include files. Examples:
  - `*.jpg` - Includes all JPEG images.
  - `project_*.docx` - Includes Word documents starting with "project_".

- **Exclude Filters**: Specifies patterns to exclude files. Examples:
  - `temp_*` - Excludes temporary files.
  - `*.log` - Excludes log files.

> **Tip**: Combine include and exclude filters for precise control over the import process.

---

## Step-by-Step Instructions

### Step 1: Start a New Import
1. Navigate to the **Import Files** section in the WSyncPro app.
2. Click the **New Import** button.  
   *(Placeholder for screenshot of the button)*

### Step 2: Define the Source and Destination
1. Select the **Source Directory** using the directory picker.
2. Select the **Destination Directory** for the imported files.  
   *(Placeholder for screenshot of directory picker)*

### Step 3: Configure Filters
1. Add **Include** filters to specify files to import.  
   *(Placeholder for screenshot of include filter input)*
2. Add **Exclude** filters to prevent unwanted files from being imported.  
   *(Placeholder for screenshot of exclude filter input)*

### Step 4: Apply Path-Building Rules (Optional)
1. Use path-building options to structure files in the destination directory.
2. Examples:
   - Group files by creation date.
   - Organize files by type and name.

### Step 5: Execute the Import
1. Click **Save** to store the import configuration.  
   *(Placeholder for screenshot of save button)*
2. Click **Run Import** to start the import process.  
   *(Placeholder for screenshot of run button)*

---

## Import Data Model

### ImportPathType
Rules for building destination paths based on file attributes.

- **DateCreated**: Groups files by their creation date.
- **FileExtension**: Groups files by their type (e.g., `.jpg`, `.txt`).

### FilterParams
Defines the inclusion and exclusion rules for files.

```csharp
public class FilterParams
{
    public List<string> Include { get; set; } = new List<string>();
    public List<string> Exclude { get; set; } = new List<string>();
    public List<string> FileTypes { get; set; } = new List<string>();
    public int MaxFileSize { get; set; } = 0; // In bytes
    public int MinFileSize { get; set; } = 0; // In bytes
}
```

---

## Troubleshooting

- **Files are not being imported**: Check the include filters and ensure the source directory contains matching files.
- **Incorrect folder structure in destination**: Review path-building rules and adjust them as needed.

---

This guide covers the basics of importing files. For more advanced features, refer to the [Features Overview](/docs/features).
