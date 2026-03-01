# Improvement for the V4.0

Version 4 will introduce four major evolutions: multi-language support, optimisation of EasyLog, update `AppServiceFactory` and full cross-platform portability. None of these will require structural changes to the Domain or Application layers. This will be the clearest proof that the architecture was designed to absorb change.

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

Rather than controllFing routing logic through runtime conditionals, the active logging configuration will be entirely driven by dependency injection configuration, making the system:
- More modular
- Easier to extend
- Simpler to reason about
- Fully compliant with DIP

> This architectural refactoring will preserve full backward compatibility. Existing projects that already reference the EasyLog library will continue to operate without any modification, as the public logging interfaces and entry points will remain unchanged. The introduction of the factory pattern and the composite will affect only the internal instantiation workflow and will not alter the external usage.

---

## 3. Full Dependency Injection : Replacing the Static Service Factory
**Design Decision :** The original static AppServiceFactory will be replaced by a full-featured Dependency Injection container based on `Microsoft.Extensions.DependencyInjection`, making the application architecture fully compliant with modern .NET composition standards.

**Architectural Impact :**

- All service instantiations will be centralized in a single composition root.
- Service lifetimes (singleton, etc...) will be explicitly controlled and documented.
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
* The Domain and Application layers will see none of this.

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
* A protected method `HandleAppException(AppException ex)` will be introduced in `ViewModelBase`, responsible for mapping any `AppErrorCode` to the correct localized error message and the affected field.
* An `IErrorMappingService` may be extracted if the mapping logic grows in complexity, making it injectable and testable independently.

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

