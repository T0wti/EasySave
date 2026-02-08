# EasySave version 1.0 — Technical Support Document
**Console Version — .NET SDK 10.0 / C#**
## Purpose

This document provides the information required by support teams to configure and maintain EasySave v1.0.

## Prerequisites

The following requirements must be met to run the application:

- Windows
- .NET SDK 10.0
- Visual Studio 2022+ or Rider
- Git

## Configuration

The application language can be changed from the main menu.

## Constraints 

During creation :
	- Unique backup job name required
	- Maximum number of backup jobs = 5

During deletion :
	- Backup job ID required

During modification :
	- Backup job ID required

## Backup Types

### Full Backup

A full backup performs a complete copy of files:

- Lists all files in the source directory
- Copies them all to the target directory

### Differential Backup

A differential backup copies only files that are new or modified since the last full backup.

The copy decision is based on three checks:

- File does not exist in the target directory
- File size is different
- File hash is different

Hash algorithm used: `SHA-256`
This algorithm was chosen for its strong collision resistance.

## JSON File Locations

**Daily Log File**
The daily log file records all backup operations in real time.

Generated JSON log files are stored at : C:\Users\Name\AppData\Roaming\EasySave\Logs
File name : YYYY-MM-DD.json

**Real-Time State File**
The real-time state file allows live tracking of backup execution.

Generated JSON state files are stored at : C:\Users\Name\AppData\Roaming\EasySave\State

## Tests

For the testing part, the open-source framework **xUnit** is used.  
Unit tests help ensure code reliability and reduce regressions in future versions.

Tests cover:

- Source and target path validation
- File selection logic
- Full backup logic
- Differential backup logic
- JSON file generation

