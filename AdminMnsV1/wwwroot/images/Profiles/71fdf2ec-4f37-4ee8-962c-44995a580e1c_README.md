


```md
# 📊 Projet : Dashboard de gestion des stagiaires

Un tableau de bord interactif affichant diverses statistiques sous forme de graphiques et de tableaux dynamiques.

---

## 🔧 Modifications et améliorations apportées

### 📈 Graphique en anneaux (Doughnut Chart) - Stagiaires par genre

✅ **Ajout d'un graphique en anneaux à deux niveaux** :
   - **Anneau extérieur (bleu) ➝** Stagiaires masculins 👨‍🎓  
   - **Anneau intérieur (orange) ➝** Stagiaires féminins 👩‍🎓  

✅ **Autres améliorations** :
   - Ajout d'une légende personnalisée avec couleurs correctes 🎨  
   - Correction du centrage du graphique ⚖️  
   - Ajout d'un icône au centre du doughnut 🎯  

---

### 🎯 Graphique polaire (Polar Chart) - Dossiers par classe

✅ **Intégration d'un graphique de type polaire** 📊  
✅ **Représentation des dossiers traités selon les différentes classes** 🏫  
✅ **Personnalisation des couleurs et des animations** 🎨  

---

## 📊 Tableau des stagiaires et état des dossiers

✅ **Ajout d'un tableau interactif avec Bootstrap** 🖥️  
✅ **Icônes ajoutées aux lignes pour indiquer les statuts** 📌  

### 🎨 **Personnalisation des boutons d’action** :
- 🟠 **En cours** ➝ Bouton bleu `btn-outline-primary`  
- 🟢 **Validé** ➝ Bouton vert `btn-outline-success`  
- 🔴 **Refusé** ➝ Bouton rouge `btn-outline-danger`  

✅ **Autres améliorations** :
   - Centrage vertical des cellules pour un meilleur alignement 🔄  
   - Suppression de l’espace à droite de la colonne **"Action"** 📏  

---

# Modifications et Améliorations du Tableau des Intervenants

## 1. Ajout de la Classe 2-B
- **10 nouveaux intervenants** ont été ajoutés, appartenant à la classe **2-B**.
- Les intervenants sont affichés **par ordre alphabétique**.

## 2. Amélioration de l'Affichage du Tableau
- Fusion des **en-têtes du tableau (`<th>`)** pour une meilleure lisibilité.
- Modification de l'en-tête de la colonne **"Action"** afin qu'il occupe **deux colonnes** avec `colspan="2"`.
- **Nouvelle colonne ajoutée** pour afficher les **professeurs**.

## 3. Ajout d'un Deuxième Bouton d'Action
- Chaque ligne du tableau possède désormais **deux boutons d'action** dans la colonne Action :
  - **Bouton 1** : Icône "visible" pour afficher les détails de l'intervenant.
  - **Bouton 2** : Icône "poubelle" pour supprimer l'intervenant.

## 4. Alignement d'un Bouton sur la Droite
- Un bouton **"Primary"** a été ajouté avec la classe `.btn-outline-primary`.
- Le bouton est aligné à droite grâce aux propriétés suivantes :
  - Le conteneur `.btnNewDossier` utilise `text-align: right;`.
  - La largeur du conteneur est définie à `width: 100%` pour garantir un bon positionnement.

## 5. Structure Mise à Jour du Tableau
- Le tableau est toujours structuré avec des **classes Bootstrap** telles que `table-striped`, `table-hover`, etc.
- Les nouvelles lignes ajoutées respectent le format et le design existants, garantissant ainsi une cohérence visuelle.

---

Ces améliorations rendent le tableau **plus lisible, ergonomique et fonctionnel**, avec une meilleure gestion des classes et des actions possibles. 🚀

# Résumé du Projet et Défis Rencontrés

Ce projet a consisté à développer une interface web avec des cartes de présentation d'étudiants et une section d'administration, en utilisant HTML, CSS et Bootstrap. Durant le développement, plusieurs défis ont été rencontrés et résolus :

## Défis Rencontrés et Solutions Apportées

1.  **Problèmes de Transition et de Couleur au Survol des Cartes :**
    * Au départ, les transitions et les couleurs de fond des cartes ne fonctionnaient pas comme prévu lors du survol.
    * Le problème a été résolu en ajustant les transitions CSS, en s'assurant que les couleurs de fond au survol alternaient correctement, et en modifiant le z-index des pseudo-éléments pour qu'ils ne masquent pas le texte.

2.  **Conflits avec la Balise `<a>` :**
    * La balise `<a>` a causé des conflits avec les styles des cartes, empêchant l'effet de couleur alternée.
    * Les solutions incluent l'utilisation de `color: inherit;` et `text-decoration: none;` pour éviter l'héritage des styles de lien, l'application des styles de couleur alternée aux éléments parents, et la réorganisation du HTML si necessaire.

3.  **Problèmes avec les Colonnes Bootstrap :**
    * Les colonnes Bootstrap (`col-lg-3` et `col-lg-9`) ne s'affichaient pas correctement.
    * Les causes possibles incluent des conteneurs manquants ou incorrects, des conflits CSS, des erreurs dans l'inclusion de Bootstrap, et des problèmes de cache du navigateur.
    * Les solutions ont été de vérifier l'intégration de bootstrap, de vérifier l'imbrication des div, et de vérifier qu'il n'y avait pas de conflits css.

4.  **Améliorations de la Structure HTML et CSS :**
    * Des suggestions ont été faites pour améliorer la sémantique HTML, utiliser des classes CSS personnalisées, organiser les informations de manière plus cohérente, et améliorer l'accessibilité.
    * Des exemples de code HTML et CSS améliorés ont été fournis pour illustrer ces suggestions.

## Leçons Apprises et Recommandations

* L'importance de l'inspecteur du navigateur pour le débogage CSS.
* La gestion des conflits CSS, en particulier avec les styles par défaut des éléments HTML.
* L'importance de la sémantique HTML et de l'accessibilité.
* L'importance de bien intégrer et de comprendre le fonctionnement de Bootstrap.
* L'importance de bien verifier l'imbrication des div.
* L'importance de tester sur plusieur navigateur.

## Ajouts Importants

* Il est crucial de vérifier régulièrement les conflits CSS potentiels, en particulier lors de l'utilisation de bibliothèques CSS comme Bootstrap.
* L'utilisation de l'inspecteur du navigateur est essentielle pour examiner les styles appliqués aux éléments et identifier les problèmes.
* Adopter une approche de développement itérative, en testant et en déboguant régulièrement, est crucial pour un développement efficace.




# Rapport de travail - [10.03]

# Modifications et Améliorations du Tableau des Intervenants

## Ajout de la Classe 2-B
- Intégration de 10 nouveaux intervenants appartenant à la classe 2-B.
- Affichage des intervenants par ordre alphabétique.

## Amélioration de l'Affichage du Tableau
- Fusion des en-têtes du tableau (`<th>`) pour une meilleure lisibilité.
- Modification de l'en-tête de la colonne "Action" pour occuper deux colonnes (`colspan="2"`).
- Ajout d'une colonne pour afficher les professeurs.

## Ajout d'un Deuxième Bouton d'Action
- Chaque ligne du tableau possède désormais deux boutons d'action :
  - Icône "visible" pour afficher les détails de l'intervenant.
  - Icône "poubelle" pour supprimer l'intervenant.

## Alignement d'un Bouton sur la Droite
- Ajout d'un bouton "Primary" avec la classe `.btn-outline-primary`.
- Alignement à droite du bouton via `text-align: right;` sur le conteneur `.btnNewDossier`.
- Largeur du conteneur fixée à `width: 100%` pour garantir un bon positionnement.

## Structure Mise à Jour du Tableau
- Utilisation des classes Bootstrap (`table-striped`, `table-hover`, etc.).
- Maintien de la cohérence visuelle avec les nouvelles lignes ajoutées.

## Résumé du Projet et Défis Rencontrés

### Problèmes et Solutions

1. **Problèmes de Transition et de Couleur au Survol des Cartes**  
   - Ajustement des transitions CSS et du z-index pour une meilleure superposition.

2. **Conflits avec la Balise `<a>`**  
   - Utilisation de `color: inherit;` et `text-decoration: none;` pour résoudre les conflits de style.

3. **Problèmes avec les Colonnes Bootstrap**  
   - Correction de l'imbrication des divs et vérification de l'inclusion correcte de Bootstrap.

4. **Améliorations de la Structure HTML et CSS**  
   - Optimisation de la sémantique HTML et accessibilité.

### Leçons Apprises et Recommandations
- Utilisation de l'inspecteur du navigateur pour le débogage CSS.
- Prise en compte des conflits CSS et importance de bien vérifier l'imbrication des divs.
- Tests sur plusieurs navigateurs pour assurer la compatibilité.

## Technologies Utilisées
- **HTML / CSS / Bootstrap 5**
- **JavaScript (Chart.js)**
- **Icons8** (pour les icônes du tableau)


# Rapport de travail - [18.03]

## 📌 Objectifs de la journée
- Correction de l'affichage des cases à cocher dans la section `typeLate`.
- Mise en place d'un **date picker** avec **Flatpickr** pour la sélection d'une période d'absence.
- Amélioration de l'ergonomie avec Bootstrap.
- Ajout de validations et restrictions sur les dates (exclusion des dates passées, dépendance entre la date de début et de fin).
- Test et validation de la fonctionnalité de sélection des dates.

---

## 🔧 Modifications et améliorations
### ✅ **Correction de l'affichage des cases à cocher**
- Utilisation de `display: flex; flex-wrap: wrap;` pour aligner les options côte à côte.
- Amélioration du style des labels pour une meilleure lisibilité et interaction.

### ✅ **Ajout d'un date picker avec Flatpickr**
- Intégration de **Flatpickr** pour permettre la sélection d'une plage de dates.
- Configuration pour empêcher la sélection d'une date de fin avant la date de début.
- Désactivation des dates passées.

### ✅ **Amélioration UX/UI avec Bootstrap**
- Ajout d'icônes Bootstrap pour les champs de date.
- Utilisation de `input-group` pour un rendu plus élégant.
- Ajout d'un bouton de validation pour confirmer la sélection de la période d'absence.

---

# Modifications et améliorations
# Rapport de travail - [22.03]

## 1. Page Profil - Bouton Trois Points
- Ajout d’un **menu déroulant Bootstrap** avec les options :
  - **Modifier le profil**
  - **Autre option**
- Positionnement du **bouton trois points** à droite du nom.

## 2. Bouton "Nouveau Dossier"
- Utilisation d’un bouton **Bootstrap `btn-outline-warning`**.
- Changement de couleur du **texte et de l’icône au hover** :
  - Texte en **noir au hover**
  - Icône en **blanc par défaut, noir au hover**

## 3. Border-Radius sur le Haut Uniquement
- Application de `border-radius` uniquement en haut d’un élément :
  ```css
  .ma-box {
      border-radius: 10px 10px 0 0;
  }




## 🛠️ Technos et librairies utilisées

- **HTML / CSS / Bootstrap 5** 🎨  
- **JavaScript (Chart.js)** 📊  
- **Icons8** (pour les icônes dans le tableau) 🎭  
```
