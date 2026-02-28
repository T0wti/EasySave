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

## 3. Current Design Pattern
The architecture currently leverages several foundational design patterns to separate concerns and manage complexity:

* **Strategy Pattern:**  Used to encapsulate backup behaviors. The `IBackupStrategy` interface allows the application to seamlessly switch between `FullBackupStrategy` and `DifferentialBackupStrategy` at runtime depending on the job configuration.
* **Factory Pattern:** Currently implemented via the static `AppServiceFactory`, which handles the instantiation and wiring of the application services and dependencies.
* **Singleton Pattern:** Used for `EasyLogService` and `LogDispatcher` (via `Lazy<T>`) to ensure only one instance of the logging engine manages file locks and writes throughout the application lifecycle.
* **Observer Pattern:** Heavily utilized in two areas:
    * **UI Binding:** Through Avalonia's MVVM implementation (`ObservableObject`, `[ObservableProperty]`), allowing the GUI to automatically react to state changes in the ViewModels.
    * **Process Monitoring:** Through `ManagementEventWatcher` (WMI) in `BusinessSoftwareWatcher`, which listens for OS-level process start/stop events to pause or resume backups dynamically.
* **Facade Pattern:** `BackupAppService` and `ConfigAppService` act as facades, exposing a simplified API to the presentation layer and hiding the complexity of the underlying domain services (manager, executor, state service, etc..). ViewModels never interact directly with domain-level services.

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

---

# Improvement for the V4.0

Version 4 will introduce four major evolutions: multi-language support, optimisation of EasyLog, update `AppServiceFactory` and full cross-platform portability. None of these will require structural changes to the Domain or Application layers. This will be the clearest proof that the architecture was designed to absorb change, not merely tolerate it.

## 1. Internationalization (i18n) : Adding New Languages
* **Design decision:** Localization will be treated as a pure infrastructure concern, fully isolated from business logic.
* All user-facing strings will be externalized into resource files (ex : `.resx` per culture like `Strings.en.resx`, `Strings.fr.resx`, `Strings.de.resx`).
* A dedicated `ILocalizationService` abstraction, resolved at startup from the user's saved locale preference, will serve as the single source for every display string in the application.
* ViewModels will bind to keys, never to hardcoded text.

**Why this matters architecturally:**
* Adding a new language will require only creating a new `.resx` file and registering the culture in `AppServiceFactory`. No ViewModel, no service, and no Domain class will need to change, a direct application of OCP.
* The current locale will be stored in the user's configuration file alongside other settings, meaning it will persist across sessions and will remain decoupled from any OS-level locale, which will be critical for multi-user server deployments.

---

## 2. EasyLog Architecture Refinement : Factory & Composite Patterns
**Design Decision :** EasyLog infrastructure will be redesigned to eliminate hard-coded instantiation logic and global singleton state, replacing them with factory-driven creation and dependency injection.
* Log writer selection will be fully delegated to a dedicated ILogWriterFactory, responsible for instantiating the correct implementation (JSON, XML, or future formats for example CSV). This will remove conditional logic from EasyLogService and will fully respect the Open/Closed Principle.

**Architectural Impact :**
* The logging system will become fully extensible: adding a new log format requires only introducing a new writer class and registering it in the factory.
* No existing business or application code will be modified when extending logging capabilities.
* The logging infrastructure will become unit-test friendly, as factories and services will be mockable or substitutable.
* Hidden global state will be eliminated, reducing coupling and increasing predictability.

**Composite Logging Dispatching** :

The LogDispatcher will be strengthened as a pure Composite Pattern implementation, capable of routing log entries to multiple logging endpoints simultaneously (local, TCP, or any future channel). 

Rather than controlling routing logic through runtime conditionals, the active logging configuration will be entirely driven by dependency injection configuration, making the system:
- More modular
- Easier to extend
- Simpler to reason about
- Fully compliant with DIP

> This architectural refactoring will preserve full backward compatibility. Existing projects that already reference the EasyLog library will continue to operate without any modification, as the public logging interfaces and entry points will remain unchanged. The introduction of the factory pattern and the composite will affect only the internal instantiation workflow and will not alter the external usage.

---

## 3. Full Dependency Injection : Replacing the Static Service Factory
**Design Decision :** The original static AppServiceFactory will be replaced by a full-featured Dependency Injection container based on Microsoft.Extensions.DependencyInjection, making the application architecture fully compliant with modern .NET composition standards.

**Architectural Impact :**

- All service instantiations will be centralized in a single composition root.
- Service lifetimes (singleton, transient) will be explicitly controlled and documented.
- Hidden singletons will be eliminated in favor of controlled object lifecycles.
- The dependency graph will become explicit, readable, and maintainable.

**Why This Matters Architecturally :**

This shift will reinforce the Dependency Inversion Principle (DIP):

- High-level services will depend only on abstractions.
- Low-level infrastructure implementations will be injected, not instantiated.

The entire system will become highly testable, allowing every infrastructure dependency to be substituted by mocks or stubs.
This will also dramatically improve evolution capability: introducing new infrastructure services will become a configuration task, not a refactoring operation.

---

## 4. Cross-Platform : Windows, Linux, macOS
* **Design decision :** The architecture will already be close to being multiplatform at the Domain and Application levels. Version 4 will close the remaining platform-specific gaps at the infrastructure and GUI levels.

**Infrastructure:**
* `FileService` will use `Path.DirectorySeparatorChar` and `Environment.GetFolderPath` exclusively, eliminating all hardcoded Windows path assumptions.
* CryptoSoft will be compiled as a self-contained native AOT binary for each target (win-x64, linux-x64, osx-arm64). `ICryptoSoftService` will abstract the process invocation, the concrete `CryptoSoftService` will resolve the correct binary path at runtime based on `RuntimeInformation.IsOSPlatform(...)`.
* In `BusinessSoftwareWatcher`, the Windows-specific WMI-based software detection will need to be replaced with a cross-platform detection layer. The `IBusinessSoftwareWatcherService` interface will abstract OS-specific discovery mechanisms, while the concrete implementations will dynamically select the appropriate detection strategy at runtime using `RuntimeInformation.IsOSPlatform(...)`. This approach will enable native support for Windows, Linux, and macOS while maintaining a clean and extensible architecture.
* The Domain and Application layers will see none of this. LogServer will run as a Docker container and will therefore be platform-neutral by definition.

**GUI (Avalonia):**
* As detailed in the framework comparison, Avalonia's Skia-based rendering engine will deliver a pixel-perfect, identical interface on all three desktop operating systems without platform-specific code paths.
* The MVVM structure will mean that no ViewModel will ever call a platform API; platform differences will be absorbed entirely within Avalonia's rendering pipeline and within the infrastructure services described above.

**Deployment & Resilience:**
* A single CI pipeline will produce three artifacts from the same source tree: a Windows installer (`.msi`), a Linux `.deb` package, and a macOS `.dmg`.
* Configuration files, log files, and state files will use the same JSON schema across all platforms, and a job configured on Windows will be transferable to a Linux server and executed without conversion.
* **Resilience angle:** The clean separation between `IFileService` (abstraction in Domain) and `FileService` (implementation in Infrastructure) will mean that a future platform-specific optimization, for example, using Linux's `sendfile` syscall for zero-copy transfers, will be introduced by creating a `LinuxFileService : IFileService` and registering it conditionally in `AppServiceFactory`. The rest of the application will remain entirely unaffected.

---

## 5. Centralized Error Handling in ViewModels
**Design Decision:** The duplicated switch blocks on `AppErrorCode` currently present in `CreateBackupMenuViewModel`, `EditBackupDetailMenuViewModel`, and `ExecuteBackupMenuViewModel` will be centralized into a shared error handling mechanism in the `ViewModelBase` class.

**Planned Implementation:**
* A protected method HandleAppException(AppException ex) will be introduced in ViewModelBase, responsible for mapping any AppErrorCode to the correct localized error message and the affected field.
* An IErrorMappingService may be extracted if the mapping logic grows in complexity, making it injectable and testable independently.

**Why This Matters Architecturally:**

* Eliminates approximately 30 lines of duplicated code per ViewModel that handles validation errors.
* Adding a new error code will require a single change in one location instead of modifying every ViewModel.
* Fully aligned with the DRY principle and improves maintainability as the number of ViewModels grows.

---

## 6. User Experience Enhancements

Version 4 will introduce a series of user-focused features designed to improve the usability, flexibility, and control of EasySave. These additions will ensure that users can manage backups more efficiently, monitor progress, and customize their experience according to their needs. These features will be implemented as isolated, pluggable services behind dedicated interfaces, ensuring that each addition follows the same DI and abstraction principles established throughout the architecture.

**Key Improvements :**

- Integrated Internal CryptoSoft

  Users will be able to take advantage of the integrated encryption engine with advanced options for customizing both encryption and decryption processes, providing enhanced security tailored to individual requirements.

- Estimated Backup Duration

   For larger backup jobs, the system will provide an estimated time to completion, allowing users to plan and monitor long-running operations more effectively.

- Scheduled Backups

   Users will be able to program backups to run automatically at specific times, reducing the need for manual intervention and ensuring regular data protection.

- Completion Notifications

   The application will notify users when a backup finishes, improving awareness and allowing immediate action if needed.

- Cloud Backup Support

   Backups will be able to be saved to or restored from cloud storage, providing more flexibility in storage options and disaster recovery scenarios.

- User Account Association

   Each backup job will be able to be associated with a specific user account, ensuring personalized job management and separation of responsibilities in multi-user environments.
- Backup History Tracking

   The application will maintain a complete history of executed backups, allowing users to review past operations, verify success, and audit their backup activities.

---

### 6. Architectural Benefits of V4 Enhancements

These refinements will collectively reinforce the architectural goals of EasySave:

| Axis | Architectural Gain |
| :--- | :--- |
| **Maintainability** | Reduced coupling, clearer dependencies, simpler evolution |
| **Extensibility** | Factory-driven creation, DI-based configuration, plug-in design |
| **Scalability** | Infrastructure growth without domain impact |
| **Portability** | Complete OS neutrality: native support for Windows, Linux, and macOS. |
| **Internationalization** | New languages can be added simply by adding resource files |


