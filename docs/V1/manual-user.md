# EasySave version 1.0 — User Manual
**Console Version — .NET SDK 10.0 / C#**

## Application Overview

EasySave is a command-line backup application that allows users to create and execute up to **5 backup jobs**.  
The software supports both **full** and **differential** backups.  
The application is multilingual and available in **French and English**.

## Prerequisites

The following requirements must be met to run the application:

- Windows
- .NET SDK 10.0
- Visual Studio 2022+ or Rider
- Git


## Program Launch

EasySave.exe to launch

## Main Menu

When the application starts, a console interface appears and provides several options depending on the desired action.

- **1. Create a file backup** → starts a backup job creation process (job name, source folder, destination folder, backup type)
- **2. Delete a file backup** → starts a backup job deletion process (job ID required)
- **3. Edit a file backup** → starts a backup job modification process (job ID required)
- **4. List file backups** → displays all configured backup jobs
- **5. Execute a file backup** → runs a backup job using its ID
- **9. Change console language** → opens the language selection menu (French or English)
- **0. Exit** → closes the application

## Backup Types

**Full backup:** copies all files from the source directory to the target directory.

**Differential backup:** copies only new or modified files since the last full backup.
## Daily Log File

The daily log file is a JSON file generated in real time and contains the following information:

- Timestamp
- Backup name
- Full source file path (UNC format)
- Full destination file path (UNC format)
- File size
- File transfer time (ms)


## Real-Time State File

The real-time state file is a JSON file that tracks backup job progress and the current action with the following information:

- Backup job name
- Timestamp of last action
- Status (Active / Inactive)

If the backup job is active, additional data is recorded:

- Total number of files to transfer
- Total file size
- Progress percentage
- Number of remaining files
- Full path of the current source file being processed
- Full destination file path

## Version 1.0 Limitations

Version 1.0 is the first console release of EasySave and includes the following limitations:

- Console-only application
- No graphical user interface (planned for version 2.0)
- Maximum of 5 backup jobs
- Sequential execution only
- No integration with external encryption software

## Language

The application is multilingual and available in **French and English**.

To change the console language, select option **9** in the main menu.

Language menu:

- **1. French** → switches the interface to French
- **2. English** → switches the interface to English
