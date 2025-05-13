using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.ConstrainedExecution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminMnsV1.Models.Students
{

    //CREATION D'UN NOUVEL STAGIAIRE
    public class StudentCreateViewModel
    {
        [Required(ErrorMessage = "Le nom est requis.")] //Ceci est un attribut de validation. Il indique que la propriété LastName est obligatoire et affiche le message d'erreur spécifié si l'utilisateur ne fournit pas de valeur.

        [MaxLength(50, ErrorMessage = "Le nom ne peut pas dépasser 50 caractères.")] // Ceci est un autre attribut de validation. Il limite la longueur maximale de la chaîne LastName à 50 caractères et affiche le message d'erreur si cette limite est dépassée.
        [Display(Name = "Nom")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "Le prénom est requis.")]
        [MaxLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères.")]
        [Display(Name = "Prénom")]
        public string FirstName { get; set; }


        [MaxLength(50, ErrorMessage = "Le sexe ne peut pas dépasser 50 caractères.")]
        [Display(Name = "Sexe")]
        public string Sexe { get; set; }


        [EmailAddress(ErrorMessage = "Format d'e-mail invalide.")]
        [MaxLength(250, ErrorMessage = "L'e-mail ne peut pas dépasser 250 caractères.")]
        [Display(Name = "Email")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Le mot de passe est requis.")]
        [DataType(DataType.Password)] //Cet attribut indique que le champ Password doit être traité comme un champ de mot de passe en masquant la saisie.
        public string Password { get; set; }


        [MaxLength(150, ErrorMessage = "L'adresse ne peut pas dépasser 150 caractères.")]
        [Display(Name = "Adresse")]
        public string Address { get; set; }


        [MaxLength(50, ErrorMessage = "La ville ne peut pas dépasser 50 caractères.")]
        [Display(Name = "Ville")]
        public string City { get; set; }


        public string? Class { get; set; }
        public int ClassId { get; set; } // Pour stocker l'ID de la classe sélectionné
        public SelectList AvailableClasses { get; set; } // Pour la liste des classes à afficher dans le dropdown

        [Display(Name = "Date de naissance")]
        [DataType(DataType.Date)] //Cet attribut indique que le champ BirthDate doit être traité comme un champ de date.
        public DateTime BirthDate { get; set; }


        [MaxLength(50, ErrorMessage = "Le numéro de téléphone ne peut pas dépasser 50 caractères.")]
        [Display(Name = "Téléphone")]
        public string Phone { get; set; }


        [Display(Name = "Nationalité")]
        public string Nationality { get; set; }


        [Display(Name = "N° Sécurité Sociale")]
        public string SocialSecurityNumber { get; set; }


        [Display(Name = "N° France Travail")]
        public string FranceTravailNumber { get; set; }


        [Display(Name = "Status")]
        public string Status { get; set; } = "Candidat"; // Valeur par défaut

        [Display(Name = "Photo de profil")]
        [DataType(DataType.Upload)]
        public IFormFile? PhotoFile { get; set; } //IFormFile est une interface dans ASP.NET Core qui représente un fichier envoyé via un formulaire.Le ?                                   indique que la photo est facultative.
   
}
}



//C'est un modèle de données spécialement conçu pour recueillir les informations nécessaires à la création d'un nouvel étudiant via un formulaire. Il inclut des attributs de validation pour s'assurer que les données saisies par l'utilisateur sont valides avant d'être enregistrées.