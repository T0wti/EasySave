# EasySave : Developer Guide

## 1. Prerequisites
- .NET SDK 10.0
- Docker (optional, for LogServer)
- An IDE : Visual Studio 2022 / Rider / VS Code

## 2. Project Structure
EasySave/

├── EasySave.Domain/          # Business logic, models, interfaces, exceptions

├── EasySave.Application/     # Use cases, DTOs, exception mapping, CLI utils

├── EasySave.GUI/             # Avalonia MVVM interface

├── EasySave.EasyLog/         # Logging DLL (JSON/XML/TCP)

├── EasySave.CryptoSoft/      # Encryption external process

├── EasySave.LogServer/       # Docker TCP log server

└── EasySave.Tests/           # Unit tests

## 3. Getting Started

### Clone & Build
git clone https://...
cd EasySave
dotnet build

### Run the GUI

Run Cryptosoft before GUI and move cryptosoft in the same directory as the GUI :

dotnet run --project EasySave.CryptoSoft

dotnet run --project EasySave.GUI

### Run via CLI
dotnet run --project EasySave.GUI -- 1-3   # jobs 1 to 3

dotnet run --project EasySave.GUI -- 1;3   # jobs 1 and 3

## 4. Configuration
On first launch, config is auto-generated at:
%AppData%\EasySave\config.json

Key fields:
| Field                    | Default           | Description                        |
|--------------------------|-------------------|------------------------------------|
| `Language`               | `English`         | `English` or `French`              |
| `LogFormat`              | `0`               | `0` = JSON, `1` = XML              |
| `LogMode`                | `0`               | `0` = Local, `1` = Remote, `2` = Both |
| `LogServerHost`          | `127.0.0.1`       | TCP log server address             |
| `LogServerPort`          | `11000`           | TCP log server port                |
| `BusinessSoftwareName`   | `null`            | Process name to watch              |
| `EncryptedFileExtensions`| `[]`              | Extensions to encrypt e.g. [".txt"]|
| `PriorityFileExtensions` | `[]`              | Extensions processed first         |
| `MaxLargeFileSizeKb`     | `0`               | 0 = disabled                       |
| `CryptoSoftPath`         | auto              | Path to CryptoSoft.exe             |
| `CryptoSoftKeyPath`      | auto              | Path to key.txt                    |

## 5. Running the LogServer (Docker)
cd EasySave.LogServer
docker compose up --build

Logs are written to ./logs/ on the host machine.
Set LogMode to 1 or 2 in config.json to enable remote logging.

## 6. Adding a New Backup Strategy
1. Create a new class in EasySave.Domain/Services/
   public class IncrementalBackupStrategy : IBackupStrategy
   {
       public List<FileDescriptor> GetFilesToCopy(string sourceDir, string targetDir) { ... }
   }

2. Add the new type to BackupType enum
   public enum BackupType { Full, Differential, Incremental }

3. Wire it in AppServiceFactory
   IBackupStrategy incrementalStrategy = new IncrementalBackupStrategy(fileService);

4. Add the selection in BackupService.ExecuteBackupCore()
   IBackupStrategy strategy = job.Type switch
   {
       BackupType.Full         => _fullStrategy,
       BackupType.Differential => _differentialStrategy,
       BackupType.Incremental  => _incrementalStrategy,
       _                       => throw new ArgumentOutOfRangeException()
   };

No other files need to change.

## 7. Adding a New Log Format
1. Create a new writer in EasySave.EasyLog/Writers/
   public class CsvLogWriter : ILogWriter
   {
       public void Write<T>(T entry) { ... }
   }

2. Add the format to LogFormat enum
   public enum LogFormat { Json = 0, Xml = 1, Csv = 2 }

3. Add the case in EasyLogService.Initialize()
   2 => new CsvLogWriter(logDirectoryPath),

No other files need to change.

## 8. Error Handling Flow
Domain throws EasySaveException (with EasySaveErrorCode)
  -> Application catches and maps via DomainExceptionMapper
    -> AppException (with AppErrorCode) bubbles up to ViewModel
      -> ViewModel displays localized error message to user

To add a new error:
1. Add code to EasySaveErrorCode (Domain)
2. Add code to AppErrorCode (Application)
3. Add mapping in DomainExceptionMapper
4. Add localized message in the GUI resource files

## 9. Key Flows

### Backup Execution (parallel)
BackupAppService.ExecuteMultiple(ids)
  -> BackupService.ExecuteBackups(jobs, registry)
    -> Task.WhenAll : one task per job
      -> BackupService.ExecuteBackupCore(job, handle)
          -> PriorityGate  : priority files first
          -> LargeSizeGate  : max one large file at a time
          -> CryptoSoftService.Encrypt() or FileService.CopyFile()
          -> StateService.Update() : updates state.json
          -> LogDispatcher.Write() : writes log entry

### Business Software Detection
This is not multi-platform : only function with a Windows system
BusinessSoftwareWatcher.WatchAsync()
  -> WMI event: process started -> handle.Pause() on all jobs
  -> WMI event: process stopped -> handle.Resume() on all jobs

### Pause / Stop (from UI)
BackupAppService.PauseBackup(jobId)
  -> BackupHandleRegistry.Get(jobId).Pause()
    -> ManualResetEventSlim.Reset()
      -> BackupService loop blocks on handle.WaitIfPaused()

BackupAppService.StopBackup(jobId)
  -> BackupHandleRegistry.Get(jobId).Stop()
    -> CancellationTokenSource.Cancel()
      -> BackupService loop throws OperationCanceledException

## 10. Running Tests
dotnet test

Tests are located in EasySave.Tests/.
Domain and Application layers are fully testable without any UI or real file system, use mocks for IFileService, ILogService, IStateService, etc.
