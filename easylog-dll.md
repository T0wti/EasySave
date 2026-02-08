# EasySave version 1.0 — EasyLog Dynamic Link Library "EasyLog.dll"
**Console Version — .NET SDK 10.0 / C#**

## Library Overview

**EasyLog.dll** is a logging library used by EasySave to:

- Record all backup operations in **JSON** format
- Maintain a **daily log file**
- Provide information for support and real-time monitoring
- Ensure **compatibility** with EasySave v1.0

It is used by **BackupService** through the singleton service `EasyLogService`.

## Project Location

The DLL is built from the project:

`EasySave.EasyLog`

## Internal Architecture

### Interfaces

`ILogService`: defines the interface containing the `WriteJson` method.

### Writers

`JsonLogWriter`: responsible for creating and writing **JSON** log files.

### EasyLogService Singleton

`EasyLogService`: singleton pattern used to provide global access to logging features.  
It provides the methods `Initialize` and `WriteJson`.

- Must be initialized using the `Initialize` method before use
- Accessible from any EasySave service

## Log File Format

### File Name

YYYY-MM-DD.json

### Expected Information

The daily log file is a real-time generated JSON file containing the following information:

- Timestamp
- Backup name
- Full source file path (UNC format)
- Full destination file path (UNC format)
- File size
- File transfer time (ms)

Compatible with both backup types: `Full` and `Differential`.