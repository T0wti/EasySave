# Justification of the General Architecture (EasySave)

The project's architecture is built upon the principle of **Clean Architecture**. 
The codebase has been divided into several distinct layers (`Domain`, `Application`, 
`GUI`, `EasyLog`) to ensure the maintainability, scalability, and testability of 
the application.

---

## 1. Layered Architecture

### A. The `Domain` Layer (Core Business Logic)

**Role:** Contains all pure business logic, data models (`BackupJob`, `BackupProgress`, 
`LogEntry`), enumerations (`BackupType`, `BackupJobState`), custom exceptions 
(`EasySaveException`, `BackupValidationException`), and all abstractions (interfaces 
such as `IBackupService`, `IFileService`, `IStateService`, `ICryptoSoftService`).

**Justification:** The Domain layer has **zero dependencies** on any other layer. 
It does not know about the GUI, the file format used for logs, or whether the 
application runs on Windows. This isolation is intentional: it guarantees 
that the fundamental business rules (example : "a differential backup only copies 
modified files", "a job cannot have an empty name") can never be accidentally broken 
by a UI change or a technology migration.

By centralizing all validation logic inside the Domain (example : `BackupManagerService` 
throws a `BackupValidationException` for invalid paths), we ensure that these rules 
are enforced **regardless of the entry point**, whether the user triggers a backup 
from the GUI, the CLI, or a future API.

---

### B. The `Application` Layer

**Role:** Contains the application's Use Cases (`BackupAppService`, `ConfigAppService`, 
`AppServiceFactory`). It acts as a bridge between the user interface and the domain 
using Data Transfer Objects (DTOs) and contain all the resources share between Console and GUI.

**Justification:** The Application layer serves two critical purposes:

**1. Protecting the UI from the Domain.**
The GUI never manipulates raw domain objects like `BackupJob` or `BackupProgress` 
directly. Instead, it receives simplified DTOs (`BackupJobDTO`, `BackupProgressDTO`) 
that expose only the data the UI actually needs. This prevents the ViewModels from 
becoming tightly coupled to internal domain structures that may change over time.

**2. Translating errors.**
Domain exceptions (`EasySaveException` and its subclasses) are caught at the 
Application boundary and converted into `AppException` objects via `DomainExceptionMapper`. 
This means the GUI only ever handles `AppErrorCode` values, it never needs to know 
whether an error originated from a file I/O failure, a validation rule, or a 
CryptoSoft process exit code. This separation makes error handling in ViewModels 
clean and straightforward.

**3. Utility helpers.**
The Application layer also hosts cross-cutting utilities like `BackupIdParser` 
(parses CLI arguments such as `"1-3"` or `"1;3"`) and `JobFilter` (search logic 
for the UI), keeping these concerns out of both the Domain and the GUI.

---

### C. The `GUI` Layer (The Presentation)

**Role:** Manages direct interaction with the user via a modern desktop interface. 
The project uses the **MVVM** (Model-View-ViewModel) pattern powered by the 
Avalonia UI framework.

**Justification:** The MVVM pattern enforces a strict contract: **Views are dumb**. 
They only bind to properties exposed by ViewModels and fire commands, they contain 
no business logic, no file system calls, and no backup rules whatsoever.

ViewModels act as the single source of truth for the UI state. They call the 
Application layer (`BackupAppService`, `ConfigAppService`) and expose the results 
as observable properties. This design has a concrete benefit: the entire GUI can 
be replaced (example : migrating from Avalonia to a web interface) without touching a 
single line of Domain or Application code.

The use of **commands** (rather than code-behind event handlers) also makes the UI 
logic independently testable, a ViewModel can be unit-tested without ever rendering 
a window.

---

### D. Modules / Infrastructure (`EasyLog`, `CryptoSoft`, `LogServer`)

**Role:** These modules handle concerns that cut across the entire application:
- `EasyLog.dll` : log persistence (JSON, XML) and routing (local, remote, both)
- `CryptoSoft.exe` : file encryption as a standalone, mono-instance external process
- `LogServer` (Docker) : centralized TCP log collection across multiple machines

**Justification:** Isolating these concerns into separate projects enforces the 
**Single Responsibility Principle** at the architectural level. Each module has one 
job and one reason to change:

- `EasyLog` changes only if the log format or routing strategy changes.
- `CryptoSoft` changes only if the encryption algorithm or key management changes.
- `LogServer` changes only if the centralization strategy changes.

Crucially, `EasyLog` was designed as a **reusable DLL** from v1.0. Any future 
project at ProSoft can reference `EasySave.EasyLog` to gain structured daily logging 
without reimplementing it. All evolutions of the library (v1.1 added XML, v1.2 added 
`LogDispatcher` for TCP routing) were made **backward-compatible**: code written 
against v1.0 continues to compile and run unchanged.

---

### E. Current Design Pattern
Your architecture currently leverages several foundational design patterns to separate concerns and manage complexity:

* **Strategy Pattern:**  Used to encapsulate backup behaviors. The `IBackupStrategy` interface allows the application to seamlessly switch between `FullBackupStrategy` and `DifferentialBackupStrategy` at runtime depending on the job configuration.
* **Factory Pattern:** Currently implemented via the static `AppServiceFactory`, which handles the instantiation and wiring of your application's services and dependencies.
* **Singleton Pattern:** Used for `EasyLogService` and `LogDispatcher` (via `Lazy<T>`) to ensure only one instance of the logging engine manages file locks and writes throughout the application lifecycle.
* **Observer Pattern:** Heavily utilized in two areas:
    * **UI Binding:** Through Avalonia's MVVM implementation (`ObservableObject`, `[ObservableProperty]`), allowing the GUI to automatically react to state changes in the ViewModels.
    * **Process Monitoring:** Through `ManagementEventWatcher` (WMI) in `BusinessSoftwareWatcher`, which listens for OS-level process start/stop events to pause or resume backups dynamically.
  
---

### F. V4.0
Version 4 introduces three major evolutions: multi-language support, pluggable log formats, and full cross-platform portability. None of these required structural changes to the Domain or Application layers. This is the clearest proof that the architecture was designed to absorb change, not merely tolerate it.

##### A. Internationalization (i18n) : Adding New Languages
* **Design decision:** Localization is treated as a pure infrastructure concern, fully isolated from business logic.
* All user-facing strings are externalized into resource files (ex : `.resx` per culture like `Strings.en.resx`, `Strings.fr.resx`, `Strings.de.resx`).
* A dedicated `ILocalizationService` abstraction, resolved at startup from the user's saved locale preference, serves as the single source for every display string in the application.
* ViewModels bind to keys, never to hardcoded text.

**Why this matters architecturally:**
* The Domain layer throws typed exceptions with machine-readable codes (`BackupValidationException`, `AppErrorCode`), not human-readable sentences.
* The translation from error code to localized message happens exclusively at the ViewModel boundary.
* Adding a new language requires only creating a new `.resx` file and registering the culture in `AppServiceFactory`. No ViewModel, no service, and no Domain class needs to change, a direct application of OCP.
* The current locale is stored in the user's configuration file alongside other settings, meaning it persists across sessions and remains decoupled from any OS-level locale, which is critical for multi-user server deployments.

##### B. Pluggable Log Formats : CSV and Beyond
* **Design decision:** Log format selection is driven entirely by the `ILogWriter` abstraction, which already existed since v1.0. Version 4 extends the plug-in surface rather than replacing it.
* EasyLog v1.3 introduces `CsvLogWriter : ILogWriter`, joining the existing `JsonLogWriter` and `XmlLogWriter`.
* `EasyLogService.Initialize()` gains one additional case in its format-selector switch. No other file in the entire solution was modified.
* This means upgrading from v1.1 (JSON, XML) to v1.3 (JSON, XML, CSV) required only the addition of 1 class.
* The user can switch the active log format at runtime from the Settings panel. The preference is persisted via `ConfigAppService` and re-injected into `EasyLogService` on the next application start (or immediately via a hot-reload path).
* `BackupService` never knows which format is active: it simply calls `ILogService.WriteLog(entry)` and the dispatcher routes accordingly.
* **Resilience angle:** Because `LogDispatcher` can fan out to multiple writers simultaneously, an administrator can configure the application to write JSON locally (for real-time monitoring tools) and CSV remotely (for spreadsheet-based auditing) at the same time, without any code change.

#### C. Cross-Platform : Windows, Linux, macOS
* **Design decision:** The architecture was already platform-agnostic at the Domain and Application levels. Version 4 closes the remaining platform-specific gaps at the infrastructure and GUI levels.

**Infrastructure:**
* `FileService` now uses `Path.DirectorySeparatorChar` and `Environment.GetFolderPath` exclusively, eliminating all hardcoded Windows path assumptions.
* CryptoSoft is compiled as a self-contained native AOT binary for each target (win-x64, linux-x64, osx-arm64). `ICryptoSoftService` abstracts the process invocation; the concrete `CryptoSoftService` resolves the correct binary path at runtime based on `RuntimeInformation.IsOSPlatform(...)`.
* The Domain and Application layers see none of this. LogServer runs as a Docker container and is therefore platform-neutral by definition.

**GUI (Avalonia):**
* As detailed in the framework comparison below, Avalonia's Skia-based rendering engine delivers a pixel-perfect, identical interface on all three desktop operating systems without platform-specific code paths.
* The MVVM structure means that no ViewModel ever calls a platform API; platform differences are absorbed entirely within Avalonia's rendering pipeline and within the infrastructure services described above.

**Deployment & Resilience:**
* A single CI pipeline produces three artifacts from the same source tree: a Windows installer (`.msi`), a Linux `.deb` package, and a macOS `.dmg`.
* Configuration files, log files, and state files use the same JSON schema across all platforms; a job configured on Windows can be transferred to a Linux server and executed without conversion.
* **Resilience angle:** The clean separation between `IFileService` (abstraction in Domain) and `FileService` (implementation in Infrastructure) means that a future platform-specific optimization, for example, using Linux's `sendfile` syscall for zero-copy transfers, can be introduced by creating a `LinuxFileService : IFileService` and registering it conditionally in `AppServiceFactory`. The rest of the application is entirely unaffected.

---

## 2. Core Principles (SOLID) Respected

Our architecture strictly adheres to the SOLID principles at every layer:

---

### S : Single Responsibility Principle (SRP)

**Definition:** A class should have one, and only one, reason to change.

**In practice:**
Every class in EasySave has a single, well-defined job:
- `FileService` only handles file system operations (copy, list).
- `StateService` only manages backup progress state persistence.
- `DomainExceptionMapper` only translates domain exceptions to application exceptions.
- `BackupManagerService` only manages the lifecycle of backup job configurations 
  (create, edit, delete, load).

A concrete example: when we needed to add XML log support in v1.1, we created a 
new `XmlLogWriter` class rather than modifying `JsonLogWriter`. `EasyLogService` 
gained a format selector, but neither writer changed. This is SRP in action, each 
class changed for exactly one reason.

---

### O : Open/Closed Principle (OCP)

**Definition:** Software entities should be open for extension, but closed for modification.

**In practice:**
The backup strategy system is the clearest example. `IBackupStrategy` defines a 
single method: `GetFilesToCopy(string sourceDir, string targetDir)`. Adding a new 
backup type (example : Incremental) requires **only** creating a new class that implements 
this interface. `BackupService`, `BackupManagerService`, and the entire Application 
layer remain completely untouched.

The same applies to log writers: adding a CSV format tomorrow means creating 
`CsvLogWriter : ILogWriter` and adding one case to `EasyLogService.Initialize()`. 
No existing code is modified.

---

### L : Liskov Substitution Principle (LSP)

**Definition:** Objects of a derived class must be substitutable for objects of 
their base class without altering the correctness of the program.

**In practice:**
`LogDispatcher` and `EasyLogService` both implement `ILogService`. Any consumer 
(example : `BackupService`) that depends on `ILogService` works identically whether it 
receives a `LogDispatcher` instance (which may route to TCP) or an `EasyLogService` 
instance (which writes locally). The substitution is transparent and produces no 
unexpected behavior.

Similarly, `FullBackupStrategy` and `DifferentialBackupStrategy` both implement 
`IBackupStrategy`. `BackupService` selects between them at runtime based on 
`job.Type`, it never needs to know which concrete strategy it is using.

---

### I : Interface Segregation Principle (ISP)

**Definition:** Clients should not be forced to depend on interfaces they do not use.

**In practice:**
Rather than creating a single large `IBackupManager` interface with every possible 
method, we decomposed responsibilities into focused interfaces:
- `IBackupService` : execution only (`ExecuteBackup`, `ExecuteBackups`)
- `IBackupManagerService` : CRUD only (`Create`, `Edit`, `Delete`, `GetAll`)
- `IStateService` : state tracking only (`Initialize`, `Update`, `Pause`, `Stop`, `Complete`)
- `ILargeSizeGate` : large file throttling only
- `IPriorityGate` : priority file ordering only

Each service only injects the interfaces it actually needs. `BackupService` for 
instance does not depend on `IBackupManagerService` at all, it only knows about 
`IFileService`, `IStateService`, `ILogService`, and the gate interfaces. This keeps 
the dependency graph shallow and makes unit testing straightforward.

---

### D : Dependency Inversion Principle (DIP)

**Definition:** High-level modules should not depend on low-level modules. 
Both should depend on abstractions.

**In practice:**
`BackupService` (high-level: orchestrates the backup workflow) never instantiates 
`FileService`, `JsonLogWriter`, or `CryptoSoftService` directly. It depends on 
`IFileService`, `ILogService`, and `ICryptoSoftService`. The concrete implementations 
are assembled once, in `AppServiceFactory`, and injected via constructors.

This has a direct practical benefit: in tests, `FileService` can be replaced with 
a mock that never touches the disk, `EasyLogService` with a no-op logger, and 
`CryptoSoftService` with a stub that returns a fixed encryption time. The entire 
backup workflow can be tested in milliseconds without any real files or processes.

`AppServiceFactory` acts as the **composition root**, the single place in the 
application where all concrete dependencies are wired together. Everything above it 
depends only on abstractions.

---

# Why Avalonia ? Technology Comparison: Avalonia UI vs. .NET MAUI

For the development of **EasySave**, choosing the right cross-platform framework was critical. While both **Avalonia UI** and **.NET MAUI** allow developers to share code across different systems, they rely on fundamentally different architectures.

## 1. Architectural Differences

### .NET MAUI (Native Wrapper)
MAUI uses a "Native Control" approach. If you define a `Button` in XAML, MAUI renders a Windows button on Windows, an Android button on Android, and a Mac button on macOS.
* **Pros:** Native look and feel, integration with platform-specific features.
* **Cons:** Inconsistent UI behavior across platforms. Limited styling flexibility because you are constrained by what the native OS control can do.

### Avalonia UI (Skia Rendering)
Avalonia uses a "Pixel-Perfect" approach. It draws every single pixel of the UI itself using the **Skia Sharp** graphics engine.
* **Pros:** 100% consistent UI on Windows, Linux, and macOS. Total control over styling.
* **Cons:** Does not automatically follow the "Native" OS look, though it can be themed to mimic it.

---

## 2. Feature Comparison Table

| Feature | Avalonia UI | .NET MAUI |
| :--- | :--- | :--- |
| **Primary Target** | Desktop (Power users/Admin tools) | Mobile (Consumer apps) |
| **Linux Support** | **Full Native Support** | No official support |
| **Windows Support** | Excellent (Win32/WinUI 3) | Excellent (WinUI 3) |
| **Styling System** | Advanced (CSS-like Selectors) | Standard XAML Styles |
| **Maturity** | Very High for Desktop | Still evolving for Desktop |

---

## 3. Why Avalonia was chosen for EasySave

### A. Professional Linux Support
EasySave is a backup utility designed for professional environments. Many servers and administrative workstations run on **Linux**. **.NET MAUI does not officially support Linux**, making it an immediate deal-breaker. Avalonia provides a first-class experience on Linux distributions (Ubuntu, Debian, etc.).

### B. UI Consistency and Precision
Since EasySave needs to display complex data (backup paths, logs, progress bars), we required an interface that behaves exactly the same on a Windows laptop and a Linux server. Avalonia’s rendering engine ensures that our "ProSoft" design language is preserved everywhere.

## Conclusion
While **.NET MAUI** is an excellent choice for mobile-centric applications, **Avalonia UI** is the superior framework for **robust, cross-platform Desktop applications** like EasySave. It offers the best performance, the most powerful styling system, and, most importantly, native Linux support.
