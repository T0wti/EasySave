# EasyLog.dll : Technical Reference v1.1

**.NET SDK 10.0 / C#  :  Namespace: `EasySave.EasyLog`**

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
| `LogDispatcher` | Singleton router. Implements `ILogService`. Routes entries to local, remote, or both. |

---

`LogDispatcher` sits in front of `EasyLogService` and decides where each log entry goes:

- **Local only** : delegates to `EasyLogService` (same behavior as v1.0/v1.1)
- **Centralized only** : sends to the remote TCP server via `TcpLogClient`
- **Both** : writes locally AND sends remotely

This design keeps `EasyLogService` unchanged and backward-compatible, while adding
remote logging as a pure extension. All consumers depend on `ILogService` and are
unaware of whether they talk to `EasyLogService` or `LogDispatcher`.

---

## EasyLogService

```csharp
// Initialize once at startup before any Write call
EasyLogService.Instance.Initialize(@"C:\Logs\MyApp", Json);

// Write a log entry
EasyLogService.Instance.Write(entry);

//Reset the current instance
EasyLogService.Instance.Reset();
```

> **Warning:** calling `Write` before `Initialize` throws a runtime exception.

| Method | Signature | Description |
|---|---|---|
| `Initialize` | `(string logDirectoryPath, settings.LogFormat)` | Sets the output directory and the log format. Call once at startup. Creates the directory if absent. |
| `Write` | `(LogEntry entry)` | Appends one entry to the current day's log file. Thread-safe. |
| `Reset` | `()` | Clears the initialized state. Required before calling `Initialize` again with a different format. |

---

## LogDispatcher
```csharp
// Initialize after EasyLogService
LogDispatcher.Instance.Initialize(EasyLogService.Instance, remote, logMode);

// Write automatically routed based on logMode
LogDispatcher.Instance.Write(entry);

// Reset (required before re-initializing with a new mode)
LogDispatcher.Instance.Reset();
```

| Log Mode | Value | Behavior |
|---|---|---|
| Local only | `0` | Writes to local file via `EasyLogService` |
| Centralized only | `1` | Sends to remote TCP server only |
| Both | `2` | Writes locally AND sends remotely |

> **Note:** always inject `ILogService` pointing to `LogDispatcher.Instance`, not to
> `EasyLogService.Instance` directly, so that remote routing is always active.

| Method | Signature | Description |
|---|---|---|
| `Initialize` | `(EasyLogService local, Func<object, Task>? remote, int logMode)` | Configures routing. Call once after `EasyLogService` is initialized. Thread-safe. |
| `Write` | `(T entry)` | Routes the entry according to the configured log mode. |
| `Reset` | `()` | Clears the initialized state. Required before reconfiguring. |

---

## Recommended Initialization Order
```csharp
// 1. Initialize local writer
EasyLogService.Instance.Initialize(settings.LogDirectoryPath, LogFormat.Json);

// 2. Build optional remote client
Func? remote = null;
if (settings.LogMode is 1 or 2)
{
    var client = new TcpLogClient(host, port, isXml: false, fallbackDirectory);
    remote = entry => client.SendAsync(entry);
}

// 3. Initialize dispatcher
LogDispatcher.Instance.Initialize(EasyLogService.Instance, remote, settings.LogMode);

// 4. Inject ILogService → always use LogDispatcher
services.AddSingleton(LogDispatcher.Instance);
```

---

## Log File Format

**Filename:** `YYYY-MM-DD.json` one file per day, auto-rotated at midnight.

```json
[
  {
    "Timestamp":      "2026-02-13T14:32:01.123Z",
    "MachineName":    "PC-DE-Andre",
    "BackupName":     "DailyBackup",
    "SourcePath":     "\\\\SRV-PROD\\data\\report.xlsx",
    "TargetPath":     "\\\\SRV-BCK\\backup\\report.xlsx",
    "FileSize":       204800,
    "TransferTimeMs": 47,
    "EncryptionTimeMs": 12
  },
  {
    "Timestamp":      "2026-02-13T14:32:02.456Z",
    "MachineName":    "PC-DE-Albert",
    "BackupName":     "DailyBackup",
    "SourcePath":     "\\\\SRV-PROD\\data\\locked.docx",
    "TargetPath":     "",
    "FileSize":       98304,
    "TransferTimeMs": -1,
    "EncryptionTimeMs": -10
    
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
| **2.0** | Implementation of LogDispatcher|
| **1.1** | Method renamed from `WriteJson` to `Write`. And implementation of the XmlWriter|
| **1.0** | Initial release. |
