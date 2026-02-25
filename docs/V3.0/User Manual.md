# EasySave version 3.0 : User Manual
**GUI Version : Windows **

---

## Application Overview

EasySave is a graphical backup application that allows users to create and execute backup jobs.  
The software supports both **full** and **differential** backups.  
The application is multilingual and available in **French and English**.  
---

## Program Launch

Double-click `EasySave.exe` to launch the application. The graphical interface opens automatically.

On first launch, the application will ask you to select your preferred language before displaying the main window.

---

## Main Window

The main window is divided into two areas:

- **Left sidebar** : navigation menu to access each feature
- **Main area** : displays the active view depending on the selected option

The sidebar contains the following options:

- **Create** → opens the backup job creation form
- **Delete** → opens the backup job deletion view
- **Edit** → opens the backup job editing view
- **List** → displays all configured backup jobs
- **Execute** → opens the backup execution view
- **Settings** → opens the application settings
- **Exit** → closes the application

<img width="950" height="658" alt="Capture d’écran 2026-02-24 à 10 09 39" src="https://github.com/user-attachments/assets/cddae2b5-9241-441e-a9ae-18a571d2c408" />

---

## Managing Backup Jobs

**Create** : fill in a name, source folder, target folder, and type (Full or Differential), then click Confirm.

<img width="609" height="602" alt="Capture d’écran 2026-02-24 à 10 05 54" src="https://github.com/user-attachments/assets/5c6ee984-f58c-47f9-b582-99459b800e61" />

**Delete** : select a job and click Confirm.

<img width="609" height="320" alt="Capture d’écran 2026-02-24 à 10 07 03" src="https://github.com/user-attachments/assets/8cda2f73-76f1-42c4-96c4-a25ed3d47ca5" />

**Edit** : select a job, update the desired fields (leave blank to keep current value), then click Confirm.

<img width="609" height="320" alt="Capture d’écran 2026-02-24 à 10 06 39" src="https://github.com/user-attachments/assets/13bb1301-88d7-4bc4-bfcb-4efae892df6b" />

**Full backup:** copies all files from source to target.  
**Differential backup:** copies only new or modified files.

---

## Executing a Backup Job

1. Click **Execute** in the sidebar
2. Select one or more jobs from the list
3. Click **Execute** to start

<img width="609" height="474" alt="Capture d’écran 2026-02-24 à 10 07 56" src="https://github.com/user-attachments/assets/ce4dd939-1aec-43e5-a76d-9812a1868feb" />

**Real-Time Interaction**
- ▶ Play : Starts or resume a paused job
- ⏸ Pause : Pauses the job, she becomes effective after the current file transfer finishes
- ⏹ Stop : Immediately stops the job,the current file transfer is aborted


> **Note:** If the configured business software is currently running, the backup will not start and an error message will be displayed. Close the business software before retrying.
>
> If backup are in progress :
> - All running backup jobs are automatically paused
> - No new job can start
> - Jobs automatically resume once the business software is closed

---

## File Encryption

EasySave can automatically encrypt files after copying them to the target directory using **CryptoSoft**.

Encryption is configured in **Settings**:

- **Encrypted file extensions** : list of file extensions to encrypt (e.g. `.txt`, `.docx`, `.pdf`)
- Only files whose extension matches this list will be encrypted after being copied

Only one instance of CryptoSoft.exe can run at a time on the same computer. If multiple files require encryption:
- Encryption requests are queued
- Files are encrypted sequentially

> **Note:** CryptoSoft.exe must be present in the same folder as EasySave.exe for encryption to work.
---

## Settings

Click **Settings** in the sidebar to access the following options:

**Language**
- **French** : switches the interface to French
- **English** : switches the interface to English

<img width="619" height="133" alt="Capture d’écran 2026-02-24 à 09 57 31" src="https://github.com/user-attachments/assets/ffdd2fc0-924d-473d-8803-10888b388e83" />

**Log format**
- **JSON** : daily log files are written in JSON format
- **XML** : daily log files are written in XML format

<img width="619" height="133" alt="Capture d’écran 2026-02-24 à 10 00 06" src="https://github.com/user-attachments/assets/081ec65d-5786-4583-931f-a58a8b9be1cb" />

**Change Log Destination**

Logs storage options : 
- Logs stored only locally
- Logs stored only on the Docker centralized server
- Logs stored both locally and centrally

> **Note:** Only one unique daily log file is created on the central server

<img width="619" height="173" alt="Capture d’écran 2026-02-24 à 10 00 45" src="https://github.com/user-attachments/assets/e1a64913-399d-4737-abc8-1e6f30898efa" />

**Business software**
- Enter the process name of the business software to monitor

**Priority File Management**
- Enter the file extension name in the Settings menu.

**Encrypted file extensions**
- Enter the list of file extensions that should be encrypted

<img width="619" height="208" alt="Capture d’écran 2026-02-24 à 10 01 30" src="https://github.com/user-attachments/assets/400bf4fc-f5b8-4c84-a15a-8440dfa1f076" />

**File Parrallel Transfert Restriction**
- Maximum size n KB to restrict parallel large transfers

<img width="596" height="111" alt="Capture d’écran 2026-02-24 à 10 05 02" src="https://github.com/user-attachments/assets/a07d5d89-0d09-496e-a9b1-52155f4ffa28" />



