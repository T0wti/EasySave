# EasySave

**.NET** application enabling **automatic file backup**.

---

## Overview

EasySave is a professional backup management application developed by ProSoft.
It allows users to create, configure, and execute backup jobs with real-time
progress tracking, file encryption, and centralized log management.

<img width="1004" height="695" alt="image" src="https://github.com/user-attachments/assets/f0d35cd7-9ad5-4f87-946c-8edf3e1d1874" />

---

## Features

- Full and differential backup jobs
- Parallel execution with priority file handling
- File encryption via CryptoSoft
- Real-time state tracking (`state.json`)
- Daily logs in JSON or XML format
- Centralized log server via Docker (optional)
- Business software detection (auto-pause)
- English and French support

---

## Requirements

- Windows 10/11 or Linux
- .NET SDK 10.0
- Docker (optional, for centralized logging)
- Visual Studio 2022, JetBrains Rider, or VS Code

---

## Getting Started
```bash
git clone https://...
cd EasySave
dotnet build
dotnet run --project EasySave.GUI
```

---

## Project Structure
```
EasySave/
├── EasySave.Domain/       # Business logic and interfaces
├── EasySave.Application/  # Use cases and DTOs
├── EasySave.GUI/          # Avalonia UI (MVVM)
├── EasySave.EasyLog/      # Logging DLL
├── EasySave.CryptoSoft/   # Encryption process
└── EasySave.LogServer/    # Docker TCP log server
```

---

## Documentation

| Document | Description |
|---|---|
| `docs/architecture.md` | Architecture and SOLID justification |
| `docs/developer-guide.md` | Setup, flows, and contribution guide |
| `docs/easylog.md` | EasyLog.dll technical reference |

---

## Authors

ProSoft — CESI 2025/2026
