# EasySave version 2.0 — Technical Support Document
**GUI Version — .NET SDK 10.0 / C#**

## Purpose

This document provides the information required by support teams to configure and maintain EasySave v2.0.

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
| `LogFormat` | `0` = JSON, `1` = XML | `0` |
| `LogDirectoryPath` | Log output directory | `%AppData%\EasySave\Logs` |
| `StateFileDirectoryPath` | State file directory | `%AppData%\EasySave\State` |
| `BusinessSoftwareName` | Process name to monitor | `null` |
| `CryptoSoftPath` | Absolute path to CryptoSoft.exe | Auto-resolved at launch |
| `CryptoSoftKeyPath` | Absolute path to key.txt | `%AppData%\EasySave\key.txt` |
| `EncryptedFileExtensions` | List of extensions to encrypt | `[]` |

To reset the configuration, delete `config.json`, it will be recreated with default values on next launch.

---

## Constraints

**Backup job creation:**
- Unique job name required

**Backup execution:**
- If the configured business software is running, the backup is interrupted immediately and its state is set to `Interrupted`
- Jobs are executed sequentially

---

## Backup Types

**Full backup:** copies all files from the source directory to the target directory.

**Differential backup:** copies only files that are new or modified. The comparison is based on:
- File existence in the target directory
- File size difference
- File hash difference (SHA-256)

---

## Encryption

After each file is copied, EasySave checks whether its extension matches `EncryptedFileExtensions`. If so, it calls `CryptoSoft.exe` as an external process with the file path and key path as arguments.

CryptoSoft returns its exit code as the encryption duration in milliseconds. A negative exit code indicates an error.

---

## Log File Format

One entry per copied file. Fields: `Timestamp`, `BackupName`, `SourcePath`, `TargetPath`, `FileSize`, `TransferTimeMs`, `EncryptionTimeMs`.

`TransferTimeMs` values: `> 0` = duration in ms, `< 0` = error code.
`EncryptionTimeMs` values: `0` = not encrypted, `> 0` = duration in ms, `< 0` = error code.

---

## Tests

Framework used: **xUnit**

Tests cover:
- Source and target path validation
- File selection logic (full and differential)
- Differential comparison (size + SHA-256 hash)
- JSON and XML log generation
- Encryption extension matching