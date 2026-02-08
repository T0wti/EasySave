#  EasySave version 1.0 - EasyLog Dynamic Link Library "EasyLog.dll"
**Version Console - .NET SDK 10.0 / C#**

## Présentation de la librairie

**EasyLog.dll** est une librairie de journalisation (logging) utilisée par EasySave pour :
	- Enregistrer toutes les opérations de sauvegarde en **JSON**.
	- Maintenir un **fichier log par jour**.
	- Fournir des informations pour le support et le suivi temps réel.
	- Garantir la **compatibilité** avec EasySave v1.0.

Elle est utilisée par **BackupService** via le service singleton `EasyLogService`.

## Emplacement dans le projet 

La DLL est compilée à partir du projet `EasySave.EasyLog`.

## Architecture interne 

### Interfaces

`ILogService` : crée l'interface comprenant la méthode  `WriteJson`

### Writers

`JSONLogWriter` : responsable de la création et de l'écriture des fichiers **JSON**

### Singleton EasyLogService

`EasyLogService` : pattern singleton pour gérer l'accès global, il comprend les méthodes `Initialize` et `WriteJson`
	- Nécessite d'être initialisé via la méthode `Initialize`
	- Accessible depuis n'importe quel service EasySave

## Format du fichier log

### Nom du fichier

YYYY-MM-DD.json

### Informations attendues

Le fichier log journalier est un fichier JSON généré en temps réel qui contient les informations suivantes :
	- Horodatage 
	- Nom de la sauvegarde (YYYY-MM-DD.json)
	- Adresse complète du fichier source (format UNC)
	- Adresse complète du fichier de destination (format UNC)
	- Taille du fichier 
	- Temps de transfert du fichier (ms)

Compatible avec les deux type de sauvegarde `Full` et `Differential`.