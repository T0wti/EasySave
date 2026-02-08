# EasySave version 1.0 - Support technique
**Version Console - .NET SDK 10.0 / C#**

## Objectif

Ce document fournit les informations nécessaires aux équipes de support pour configurer et maintenir EasySave v1.0.

## Prérequis 

Les prérequis à remplir pour pouvoir exécuter l'application :
	- Windows
	- .NET SDK 10.0
	- Visual Studio 2022+ ou Rider
	- Git

## Configuration

La langue de l'application peut être changée depuis le menu principal. 

## Contraintes 

Lors de la création :
	- Nom unique
	- Limite maximale de travaux de sauvegarde = 5

Lors de la suppression :
	- ID de la sauvegarde

Lors de la modification :
	- ID de la sauvegarde

## Types de sauvegarde 

### Sauvegarde complète

La sauvegarde complète effectue une copie complète du fichier :
	- Liste les fichiers du dossier source
	- Les copies tous

### Sauvegarde différentielle

La sauvegarde différentielle copie uniquement les fichiers nouveaux ou modifiés depuis la dernière sauvegarde complète.
La copie s'exécute en fonction de 3 paramètres :
	- Si le fichier est absent des fichiers cibles
	- Si la taille est différente
	- Si le hash est différent 

L'algorithme de hachage utilisé : SHA-256
Le choix s'est porté sur cet algorithme pour sa résistance aux collisions.

## Emplacement des fichiers JSON 

**Log journalier**
Le fichier log journalier trace toutes les opérations de sauvegarde en temps réel.

Les fichiers JSON générés par les logs journaliers s'enregistrent dans l'emplacement : C:\Users\Name\AppData\Roaming\EasySave\Logs
Nom de fichier : YYYY-MM-DD.json

**État temps réel**
Le fichier état temps réel permet le suivi en direct de l’exécution des sauvegardes.

Les fichiers JSON état temps réel générés s'enregistrent dans l'emplacement : C:\Users\Name\AppData\Roaming\EasySave\State

## Tests

Pour la partie tests, l’outil open source utilisé est **xUnit**.  
Les tests unitaires permettent de garantir la fiabilité du code et de limiter les régressions lors des évolutions futures.

Les tests couvrent :
	- Validation des chemins source et cible
	- Sélection des fichiers à copier 
	- Logique de la sauvegarde complète 
	- Logique de la sauvegarde différentielle
	- Génération des fichiers JSON 

