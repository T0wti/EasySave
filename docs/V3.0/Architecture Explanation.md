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

### B. The `Application` Layer (The Orchestrator)

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