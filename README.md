## ADMIN MNS

## 📝 Description
**AdminMnsV1** est une application de gestion administrative développée en **C# / ASP.NET MVC**.  
Ce projet implémente un tableau de bord structuré pour centraliser la gestion de données et de dossiers,
créé dans le cadre de mon examen pour le diplôme **Concepteur Développeur d'Application 2024/2025**.

L’objectif principal est de fournir une interface intuitive pour l’administration et le suivi de processus métiers au sein du MNS.

````markdown
[![.NET](https://img.shields.io/badge/.NET-5.0%2B-blue.svg)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
---

## 🚀 Fonctionnalités

- Gestion des stagiaires (CRUD)
- Gestion et creation de dossiers
- Gestion des rôles et permissions
- Gestion des utilisateurs avec authentification
- Association des stagiaires à des classes
- Dashboard de pilotage pour visualiser l’activité
- Architecture modulaire facilitant l’ajout de fonctionnalités
- Persistance des données via Entity Framework et base SQL

---

## 🛠️ Technologies & Outils

- Langage : C#
- Framework : .NET 5+ / ASP.NET MVC
- Base de données : SQL Server (ou autre via chaîne de connexion)
- Interface : Razor Views, Bootstrap, CSS, JS
- IDE : Visual Studio 2022
- Versioning : Git / GitHub

---

## 📂 Structure du dépôt

```text
├── AdminMnsV1/             # Code source principal
│   ├── [Fichiers .cs]      # Logique métier
│   └── AdminMnsV1.csproj   # Fichier projet
├── AdminMnsV1.sln          # Solution Visual Studio
├── .gitignore              # Fichiers exclus de Git
└── README.md               # Documentation du projet
````

---

## ⚙️ Installation et exécution

### Prérequis

* [Visual Studio 2022](https://visualstudio.microsoft.com/fr/vs/) avec charge de travail .NET
* SDK .NET compatible

### Étapes

1. **Cloner le dépôt :**

```bash
git clone https://github.com/Looka57/AdminMnsV1.git
```

2. **Ouvrir la solution :** `AdminMnsV1.sln` dans Visual Studio
3. **Restaurer les packages NuGet**
4. **Configurer la base de données** dans `appsettings.json` ou `Web.config`
5. **Appliquer les migrations** (si Entity Framework) :

```bash
Update-Database
```

6. **Lancer l'application** avec IIS Express ou ton serveur préféré

---

## ▶️ Utilisation & Workflow

1. Administration : Connectez-vous avec un compte admin pour piloter la plateforme.
2. Inscription & Mail : Lors de la création d'un futur élève, le système déclenche automatiquement un envoi de mail de bienvenue/notification.
3. Gestion des Dossiers : 
   - Accédez à l'espace documentaire de l'élève.
   - Consultez les pièces justificatives envoyées.
   - Utilisez les boutons **Accepter** ou **Refuser** pour valider les documents.
4. Suivi : Naviguez dans les menus pour suivre l'état d'avancement des dossiers et gérer les classes.
5. L'élève peut aussi voir l'avancement de son dossier et les pieces refusées ou acceptées

---

## 🧪 Tests

Si tu as des tests unitaires ou d’intégration :

```bash
dotnet test
```

---

## 🚧 État du projet & Travaux en cours
Le cœur du système (Authentification, Rôles, CRUD Stagiaires) est opérationnel. Le module suivant est actuellement en phase de développement :

- [ ] Module Absences & Retards : 
    - Saisie des absences par classe.
    - Justification des retards.
    - Calcul automatique du volume horaire manquant.


## 👩‍💻 Contribution

Ce projet est développé par **Amandine** (@Looka57).
Les contributions sont les bienvenues via Issues ou Pull Requests.

---

## 📄 Licence

Ce projet est sous licence **MIT** – voir le fichier [LICENSE](LICENSE) pour plus d’informations.

---

✨ Merci d’utiliser **AdminMnsV1** ! 👏

```
