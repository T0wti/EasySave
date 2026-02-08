# EasySave version 1.0 — Class Diagram Manual

## Architectural Layers

A. EasyLog
This layer is responsible for low-level data persistence regarding application activity.

    EasyLogService: Implements the Singleton pattern to provide a global access point for logging.
    JsonLogWriter: Decouples the logging logic from the file system, specifically handling JSON serialization.

B. Domain
The core of the system. It contains the business rules and entities without any dependency on the UI or database.

    Models: Entities like BackupJob (stores configuration) and BackupProgress (tracks real-time state).
    Services: Logic for file manipulation and backup orchestration.
    Interfaces: Define the "contracts" that the other layers interact with.

C. Application
This layer acts as an intermediary. It translates user actions from the UI into business logic calls.

    Controllers: Manage the flow of data.
    DTOs (Data Transfer Objects): Lightweight objects used to pass data to the UI without exposing the internal logic of the Domain entities.

D. Console
The user interface. It handles display logic and user input.

    Resources: Manages internationalization (i18n).
    ConsoleUI: Contains the visual structure of the menus.
    Commands: Orchestrates the interaction loops.

## Relationships
The strength of this diagram lies in the precise use of UML connectors to represent code structure.

A. Realization (Dashed line with hollow triangle ..|>)
This is used between Interfaces and Classes.

    Logic: It signifies that a class "fulfills the contract" of an interface.
    Example: BackupService ..|> IBackupService.
    Impact: The rest of the application (like the BackupController) only knows about the interface. This allows you to replace the BackupService with a new version without changing a single line of code in the Controller.

B. Generalization / Inheritance (Solid line with hollow triangle ——|>)

This represents an "is-a" relationship between two classes.
    Logic: A child class inherits all properties and methods from a parent class.
    Example: CreateBackupMenu ——|> GeneralContent.
    Impact: All menus share the same Header() and Footer() logic defined in GeneralContent, reducing code duplication.

C. Association (Solid line with open arrow ——>)
This represents a structural relationship where one class "has" or "knows" another as a field/property.

    Logic: Indicates a long-term relationship.
    Example: ConsoleRunner ——> BackupController.
    Impact: The ConsoleRunner holds a reference to the controller to delegate user commands. In the code, this is usually initialized via the constructor.

D. Dependency (Dashed line with open arrow - - >)
This represents a transient relationship.

    Logic: A class "uses" another temporarily (as a parameter in a method or a local variable).
    Example: BackupController - - > BackupJobDTO.
    Impact: The controller creates or returns a DTO, but it doesn't "own" it as a permanent member.