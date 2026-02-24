# Justification of the General Architecture (EasySave)

The project's architecture is built upon the principle of **Clean Architecture**. The codebase has been divided into several distinct layers (`Domain`, `Application`, `GUI`, `EasyLog`) to ensure the maintainability, scalability, and testability of the application.

## 1. Layered Architecture

### A. The `Domain` Layer (Core Business Logic)
* **Role:** Contains all pure business logic, data models (`BackupJob`, `LogEntry`), enumerations (`BackupType`, `BackupJobState`), and abstractions (Interfaces like `IBackupService`, `IFileService`, `ILogClient`).
* **Justification:** The domain does not depend on any other layer (neither the graphical interface nor the specific file system). This ensures that fundamental business rules remain intact and independent of external frameworks or technologies.

### B. The `Application` Layer (The Orchestrator)
* **Role:** Contains the application's Use Cases (`BackupAppService`, `ConfigAppService`). It acts as a bridge between the user interface and the domain using Data Transfer Objects (DTOs).
* **Justification:** It prevents exposing complex domain objects directly to the user interface. It orchestrates application logic (e.g., coordinating job creation, error checking, and mapping domain exceptions to application exceptions via `DomainExceptionMapper`).

### C. The `GUI` Layer (The Presentation)
* **Role:** Manages direct interaction with the user via a modern desktop interface. The project uses the **MVVM** (Model-View-ViewModel) pattern powered by the Avalonia UI framework.
* **Justification:** This strict decoupling allows for total separation of visual logic from business logic. The interface is considered "dumb": the Views simply display the data formatted by the ViewModels and capture events (clicks, inputs), without ever containing backup rules or file processing logic.

### D. Cross-Cutting Modules / Infrastructure (`EasyLog`)
* **Role:** Manages the persistence and export of activity logs (JSON, XML, TCP to LogServer).
* **Justification:** Isolating the logging logic into a separate project respects the Single Responsibility Principle (SRP). This module is decoupled and could even be reused as a standalone library (DLL) in an entirely different project.

---

## 2. Core Principles (SOLID) Respected

### 1. Separation of Concerns (SoC)
Each project has a single, well-defined responsibility. If a display or animation bug occurs, the team knows to look in the `GUI` layer. If a calculation rule for maximum parallel file size fails, the search goes directly to the `Domain` layer.

### 2. Dependency Inversion Principle (DIP)
Higher-level layers depend on abstractions (Interfaces) rather than concrete implementations. This is achieved through the **Dependency Injection** design pattern (via `AppServiceFactory`) which ties everything together. This makes it easy to swap out application behavior (e.g., changing how files are copied to the disk) without having to modify the core code.

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