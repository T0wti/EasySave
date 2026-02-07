# EasySave version 1.0 - Manuel d'utilisation
**Version Console - .NET SDK 10.0 / C#**


## Présentation de l'application 

EasySave est un logiciel de sauvegarde en ligne de commande permettant de créer et d’exécuter jusqu’à 5 travaux de sauvegarde.
Le logiciel supporte les sauvegardes complètes et différentielles. 
L’application est multi-langues, utilisable en français et en anglais. 

## Prérequis 

Les prérequis à remplir pour pouvoir exécuter l'application :
	- Windows
	- .NET SDK 10.0
	- Visual Studio 2022+ ou Rider
	- Git

## Lancement du programme

Depuis la racine du projet : dotnet run --project src/EasySave.Console

## Menu principal 

Au lancement de l'application, une interface apparaît. Elle nous offre plusieurs possibilitées en fonction de ce qu'on souhaite faire.

Interface :
	- 1. Créer une sauvegarde de fichiers => permet de démarrer une procédure de sauvegarde (nom du fichier, source du dossier, destination, type de sauvegarde)
	- 2. Supprimer une sauvegarde de fichiers => permet de démarrer une procédure de supppresion d'un travail de sauvegarde (ID du travail)
	- 3. Modifier une sauvegarde de fichiers => permet de démarrer une procédure de modification d'un travail de sauvegarde (ID du travail)
	- 4. Lister les sauvegardes de fichiers => permet d'afficher la liste des travaux de sauvegarde
	- 5. Exécuter une sauvegarde de fichiers => permet d'exécuter une sauvegarde à partir de son ID
	- 9. Changer la langue de la console => permet d'accéder à une interafce permettant de changer la langue de la console (français ou anglais)
	- 0. Quitter => fermeture de l'application

## Types de sauvegarde 

Complète : copie tous les fichiers du dossier source vers le dossier cible.
Différentielle : copie uniquement les fichiers nouveaux ou modifiés depuis la dernière sauvegarde complète.

## Fichier Log journalier 

Le fichier log journalier est un fichier JSON générée en temps réel et contient les informations suivantes :
	- Horodatage 
	- Nom de la sauvegarde (YYYY-MM-DD.json)
	- Adresse complète du fichier source (format UNC)
	- Adresse complète du fichier de destination (format UNC)
	- Taille du fichier 
	- Temps de transfert du fichier (ms)

## Fichier Etat temps réel 

Le fichier d'état temps réel est un fichier JSON regroupant l'état d'avancement des travaux de sauvegarde et l'action en cours avec les informations suivantes :
	- Nom du travail de sauvegarde
	- Horodatage de la dernière action
	- Statut (Actif / Inactif)
Si l'état du travail de sauvegarde est actif on rajoute :
	- Nombre total des fichiers à transférer
		- Taille des fichiers
		- Progression
		- Nombre de fichiers restants
		- Tailles des fichiers restants
		- Adresse complète du fichier source en cours de sauvegarde
		- Adresse complète du fichier de destination

## Langue 

L'application est multi-langue, elle est disponible en français et en anglais.
Si vous souhaitez changer la langue de la console, le menu de l'application donne accès à cette possibilité en sélectionnant 9.

Interface :
	- 1. Français => bascule l'interface dans sa version française
	- 2. Anglais => bascule l'interface dans sa version anglaise