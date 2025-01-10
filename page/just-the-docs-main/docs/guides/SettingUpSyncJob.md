---
title: Setting Up a SyncJob
parent: Guides
nav_order: 2
---

# Setting Up a SyncJob

This guide walks you through the steps to set up a **SyncJob** in WSyncPro. SyncJobs are at the heart of file synchronization, allowing you to define rules and filters for keeping directories in sync.

## Key Concepts

### 1. Source and Destination Directories
A SyncJob requires you to define a **Source Directory** (where files are located) and a **Destination Directory** (where files will be synchronized).

### 2. Keep Directories
- **Enabled**: Maintains the directory structure during synchronization.
- **Disabled**: All files are placed directly into the destination directory.

### 3. Wildcard Filtering
Filters are applied using wildcard patterns to include or exclude files during synchronization.

- **Include Filters**: Specifies patterns of files to include. Examples:
  - `*.txt` - Includes all text files.
  - `report_*.pdf` - Includes all PDF files starting with "report_".

- **Exclude Filters**: Specifies patterns of files to exclude. Examples:
  - `temp_*` - Excludes all files starting with "temp_".
  - `*.log` - Excludes all log files.

> **Tip**: Wildcards (`*`, `?`) can be used to define flexible patterns.  
> Example: `data_202?_*.csv` matches files like `data_2021_sales.csv` or `data_2023_inventory.csv`.

---

## Step-by-Step Instructions

### Step 1: Create a New SyncJob
1. Open the **SyncJob Management** section in the WSyncPro app.
2. Click the **New SyncJob** button.  
   *(Placeholder for screenshot of the button)*

### Step 2: Define Basic Settings
1. Provide a **Name** and **Description** for the job.
2. Select the **Source Directory** and **Destination Directory** using the directory picker.  
   *(Placeholder for screenshot of directory picker)*

### Step 3: Configure Filters
1. Add **Include** filters to specify files to include in synchronization.  
   *(Placeholder for screenshot of include filter input)*
2. Add **Exclude** filters to omit certain files.  
   *(Placeholder for screenshot of exclude filter input)*

### Step 4: Enable Advanced Options (Optional)
1. Toggle the **Keep Directories** option based on your preference.
2. Enable or disable overwrite for existing files in the destination directory.

### Step 5: Save and Run the Job
1. Click the **Save** button to store your SyncJob.  
   *(Placeholder for screenshot of save button)*
2. Select the job and click **Run** to execute it.  
   *(Placeholder for screenshot of run button)*

---

## SyncJob Data Model

### SyncJob
```csharp
public class SyncJob : Job
{
    public string SrcDirectory { get; set; }
    public string DstDirectory { get; set; }
    public bool KeepDirectories { get; set; }
    public FilterParams FilterParams { get; set; } = new FilterParams();
    public int RunCount { get; set; } = 0;
    public JobStatus Status { get; set; } = JobStatus.Unknown;
}
```

### FilterParams
- **Include**: List of wildcard patterns for including files.
- **Exclude**: List of wildcard patterns for excluding files.
- **File Types**: Specify extensions like `.txt` or `.jpg`.
- **File Size**: Define minimum and maximum file sizes for synchronization.

---

## Troubleshooting

- **No files are being synchronized**: Ensure your include filters are correctly configured and not being overridden by exclude filters.
- **Unexpected files in the destination**: Verify your exclude filters and directory structure settings.

---

Stay tuned for more advanced tutorials on setting up automated synchronization and leveraging the power of WSyncPro!
