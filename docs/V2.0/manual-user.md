# EasySave version 2.0 — User Manual
**GUI Version — Windows **

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

- **Left sidebar** — navigation menu to access each feature
- **Main area** — displays the active view depending on the selected option

The sidebar contains the following options:

- **Create** → opens the backup job creation form
- **Delete** → opens the backup job deletion view
- **Edit** → opens the backup job editing view
- **List** → displays all configured backup jobs
- **Execute** → opens the backup execution view
- **Settings** → opens the application settings
- **Exit** → closes the application

---

## Managing Backup Jobs

**Create** — fill in a name, source folder, target folder, and type (Full or Differential), then click Confirm.

**Edit** — select a job, update the desired fields (leave blank to keep current value), then click Confirm.

**Delete** — select a job and click Confirm.

**Full backup:** copies all files from source to target.  
**Differential backup:** copies only new or modified files.

---

## Executing a Backup Job

1. Click **Execute** in the sidebar
2. Select one or more jobs from the list
3. Click **Execute** to start

> **Note:** If the configured business software is currently running, the backup will not start and an error message will be displayed. Close the business software before retrying.

---

## File Encryption

EasySave can automatically encrypt files after copying them to the target directory using **CryptoSoft**.

Encryption is configured in **Settings**:

- **Encrypted file extensions** — list of file extensions to encrypt (e.g. `.txt`, `.docx`, `.pdf`)
- Only files whose extension matches this list will be encrypted after being copied

> **Note:** CryptoSoft.exe must be present in the same folder as EasySave.exe for encryption to work.

---

## Settings

Click **Settings** in the sidebar to access the following options:

**Language**
- **French** : switches the interface to French
- **English** : switches the interface to English

**Log format**
- **JSON** : daily log files are written in JSON format
- **XML** : daily log files are written in XML format

**Business software**
- Enter the process name of the business software to monitor

**Encrypted file extensions**
- Enter the list of file extensions that should be encrypted