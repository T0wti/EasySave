# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run Commands

```bash
# Build entire solution
dotnet build EasySave.slnx

# Run Console app
dotnet run --project src/EasySave.Console

# Run GUI app
dotnet run --project src/EasySave.GUI

# Run all tests
dotnet test EasySave.slnx

# Run a specific test project
dotnet test tests/EasySave.Domain.Tests
dotnet test tests/EasySave.EasyLog.Tests
dotnet test tests/EasySave.Application.Tests

# Run a single test by name
dotnet test tests/EasySave.Domain.Tests --filter "FullyQualifiedName~TestMethodName"

# Publish Console (Windows)
dotnet publish src/EasySave.Console -c Release -r win-x64

# LogServer Docker (run from src/EasySave.LogServer/)
docker compose up -d
docker compose down
```

## Architecture

EasySave is a .NET 10 file backup application following **Domain-Driven Design** with clean layer separation.

### Layer Structure

| Project | Role |
|---------|------|
| `EasySave.Domain` | Pure business logic: models, services, interfaces, strategies |
| `EasySave.Application` | Orchestration, DTOs, `AppServiceFactory` wires all DI |
| `EasySave.Console` | CLI presentation (interactive menu + command-line mode) |
| `EasySave.GUI` | Avalonia 11 desktop UI (MVVM, `CommunityToolkit.MVVM`) |
| `EasySave.EasyLog` | Local file logging (JSON/XML daily files) |
| `EasySave.LogServer` | Centralized TCP log aggregation server (port 11000, Docker) |
| `EasySave.CryptoSoft` | Standalone encryption executable (called as subprocess) |

### Key Architectural Patterns

**Backup execution** (`BackupService`): Strategy pattern selects files (Full = all, Differential = changed by size+SHA256). Files execute in order: priority extensions first, then remaining. Two cross-job gates serialize access: `PriorityGate` blocks non-priority files while priority files copy; `LargeSizeGate` allows only one large file transfer at a time across all parallel jobs. Jobs run concurrently via `Task.WhenAll`.

**Job lifecycle control** (`BackupJobHandle`): Each running job gets a handle. Pause uses `ManualResetEventSlim` (thread waits at file boundaries). Stop uses `CancellationTokenSource`. `BackupHandleRegistry` tracks all active handles for the business software watcher to pause all jobs simultaneously.

**Logging** (`CompositeLogService`): Three modes — local only (0), TCP only (1), both (2). Local writes to `%AppData%\EasySave\logs\yyyy-MM-dd.{json|xml}`. Remote sends to `TcpLogClient` with `"JSON|"` or `"XML|"` prefix over TCP. Falls back to local if TCP fails.

**Encryption**: After file copy, `CryptoSoftService` launches `CryptoSoft.exe` as a subprocess if the file extension matches configured encrypted extensions. CryptoSoft is mono-instance; retry logic handles busy state (max 10 retries, 500ms delay, exit code -10 = busy).

**Business software watcher**: Uses WMI (`System.Management`) to monitor a configured process name. When the process starts, all active backup jobs are paused; when it exits, they resume.

### Configuration

All settings persisted to `%AppData%\EasySave\config.json`:

- `Language`: 0=French, 1=English
- `LogFormat`: 0=JSON, 1=XML
- `LogMode`: 0=Local, 1=Centralized, 2=Both
- `LogServerHost` / `LogServerPort` (default 11000)
- `BusinessSoftwareName`: process name to watch (without .exe)
- `EncryptedFileExtensions`: list of extensions to encrypt after copy
- `PriorityFileExtensions`: list copied before others across all parallel jobs
- `MaxLargeFileSizeKb`: threshold for the large-file gate
- `CryptoSoftPath` / `CryptoSoftKeyPath`

Backup jobs persisted to `%AppData%\EasySave\jobs.json`. Runtime progress in `%AppData%\EasySave\state.json`.

### Dependency Injection

`AppServiceFactory` (in `EasySave.Application`) is the composition root — it instantiates all domain services and wires them together. Both Console and GUI use it to get their application services. There is no DI container; all wiring is manual in this factory.

### Testing

Tests use **xUnit** + **Moq**. All domain services are tested by mocking their `IFileService`, `ILogService`, `IBackupStrategy`, etc. dependencies. The test projects mirror the source projects they cover.

### LogServer Docker

The LogServer is a standalone TCP server that aggregates logs from multiple clients into daily files under `/app/logs`. Run it from `src/EasySave.LogServer/` using `docker compose up -d`. It listens on port 11000 and supports simultaneous JSON and XML clients.
