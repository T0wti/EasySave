# EasyLog.dll — Technical Reference v1.1

**.NET SDK 10.0 / C#  —  Namespace: `EasySave.EasyLog`**

---

## Overview

EasyLog is a lightweight .NET logging library that writes file transfer records to **daily JSON log files**. It has no external dependencies and is fully initialized via code.

---

## Architecture

| Component | Role |
|---|---|
| `ILogService` | Public interface. Depend on this, not on the concrete type. |
| `JsonLogWriter` | Internal. Handles JSON serialization and file writes. Never used directly. |
| `XmlLogWriter` | Internal. Handles XML serialization and file writes. Never used directly. |
| `EasyLogService` | Singleton entry point. Implements `ILogService`. |

---

## EasyLogService

```csharp
// Initialize once at startup — before any Write call
EasyLogService.Initialize(@"C:\Logs\MyApp");

// Write a log entry
EasyLogService.Instance.Write(entry);
```

> **Warning:** calling `Write` before `Initialize` throws a runtime exception.

| Method | Signature | Description |
|---|---|---|
| `Initialize` | `(string logDirectoryPath)` | Sets the output directory. Call once at startup. Creates the directory if absent. |
| `Write` | `(LogEntry entry)` | Appends one entry to the current day's log file. Thread-safe. |

---

## Log File Format

**Filename:** `YYYY-MM-DD.json` one file per day, auto-rotated at midnight.

```json
[
  {
    "Timestamp":      "2026-02-13T14:32:01.123Z",
    "BackupName":     "DailyBackup",
    "SourcePath":     "\\\\SRV-PROD\\data\\report.xlsx",
    "TargetPath":     "\\\\SRV-BCK\\backup\\report.xlsx",
    "FileSize":       204800,
    "TransferTimeMs": 47
  },
  {
    "Timestamp":      "2026-02-13T14:32:02.456Z",
    "BackupName":     "DailyBackup",
    "SourcePath":     "\\\\SRV-PROD\\data\\locked.docx",
    "TargetPath":     "",
    "FileSize":       98304,
    "TransferTimeMs": -1
  }
]
```

A `TransferTimeMs` of **`-1`** and an empty `TargetPath` indicate a failed transfer.

---

## Dependency Injection

```csharp
using EasySave.EasyLog;
using EasySave.EasyLog.Interfaces;
using Microsoft.Extensions.DependencyInjection;

EasyLogService.Initialize(@"C:\Logs\MyApp");

services.AddSingleton<ILogService>(EasyLogService.Instance);
```

Always inject `ILogService` never the concrete `EasyLogService` to keep consumers testable.

---

## Changelog

| Version | Changes |
|---|---|
| **1.1** | Method renamed from `WriteJson` to `Write`. And implementation of the XmlWriter|
| **1.0** | Initial release. |
