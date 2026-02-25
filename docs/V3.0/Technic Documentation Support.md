# EasySave version 3.0 : Technical Support Document
**GUI Version : .NET SDK 10.0 / C#**

## Purpose

This document provides the information required by support teams to configure and maintain EasySave v3.0.

---

## Prerequisites

- Windows
- .NET SDK 10.0

---

## File Locations

| File | Default Path |
|---|---|
| Executable | User-defined (example : `Desktop\EasySave\EasySave.exe`) |
| CryptoSoft | Same folder as `EasySave.exe` |
| Configuration | `%AppData%\EasySave\config.json` |
| Encryption key | `%AppData%\EasySave\key.txt` |
| Backup jobs | `%AppData%\EasySave\jobs.json` |
| Real-time state | `%AppData%\EasySave\State\state.json` |
| Daily logs | `%AppData%\EasySave\Logs\YYYY-MM-DD.json` or `.xml` |

---

## Configuration File

`config.json` is generated automatically on first launch. It contains the following fields:

| Field | Description | Default |
|---|---|---|
| `Language` | `0` = French, `1` = English | `1` |
| `LogDirectoryPath` | Log output directory | `%AppData%\EasySave\Logs` |
| `StateFileDirectoryPath` | State file directory | `%AppData%\EasySave\State` |
| `LogFormat` | `0` = JSON, `1` = XML | `0` |
| `LogMode`| `0` = Local, `1` = Centralized, `2` = Both | `0` |
| `LogServerPort` | Listening port | `11000` |
| `BusinessSoftwareName` | Process name to monitor | `null` |
| `EncryptedFileExtensions` | List of extensions to encrypt | `[]` |
| `PriorityFileExtensions` | Extensions treated as priority | `[]` |
| `MaxLargeFileSizeKb`| Max size (KB) preventing parallel large transfers | `0` |
| `CryptoSoftPath` | CryptoSoft output directory | `Desktop\EasySave\CryptoSoft.exe` |
| `CryptoSoftKeyPath` | Absolute path to key.txt | `%AppData%\EasySave\key.txt` |

To reset the configuration, delete `config.json`, it will be recreated with default values on next launch.

---

## Constraints

**Backup job creation:**
- Unique job name required

**Backup execution:**

If BusinessSoftwareName is configured a background monitor checks running processes.

If detected:

- All transfers pause immediately
- No new transfer can begin
- Jobs automatically resume when the process stops.
		
Jobs are executed sequentially

---

## Backup Types

**Full backup:** copies all files from the source directory to the target directory.

**Differential backup:** copies only files that are new or modified. The comparison is based on:
- File existence in the target directory
- File size difference
- File hash difference (SHA-256)

---

## Parallel Backup Execution

Backup jobs are now executed in parallel.
Multiple jobs can run simultaneously, subject to the rules below:
	- Priority file rules
	- Large file transfer restriction
	- Business software monitoring
	- CryptoSoft Mono-Instance limitation

> **Note:**  it is forbidden to transfer two files larger than n KB at the same time (n KB is configurable).

## Encryption

After each file is copied, EasySave checks whether its extension matches `EncryptedFileExtensions`. If so, it calls `CryptoSoft.exe` as an external process with the file path and key path as arguments.

CryptoSoft returns its exit code as the encryption duration in milliseconds. A negative exit code indicates an error.

**Mono-Instance Constraint:** CryptoSoft cannot run simultaneously multiple times.

---

## Log File 

### Format

One entry per copied file. Fields: `Timestamp`, `BackupName`, `SourcePath`, `TargetPath`, `FileSize`, `TransferTimeMs`, `EncryptionTimeMs`.

`TransferTimeMs` values: `> 0` = duration in ms, `< 0` = error code.
`EncryptionTimeMs` values: `0` = not encrypted, `> 0` = duration in ms, `< 0` = error code.

### Storage

**Daily log files centralization:** a Docker-based real-time log centralization service is available.

1. Local Only : logs written to
2. Centrilized Only : logs sent via TCP (port 11 000) to LogServer
3. Both : logs written locally and sent to server

---

## Tests

Framework used: **xUnit**

Tests cover:
- Source and target path validation
- File selection logic (full and differential)
- Differential comparison (size + SHA-256 hash)
- JSON and XML log generation
- Encryption extension matching
