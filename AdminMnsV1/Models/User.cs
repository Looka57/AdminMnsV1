using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
namespace AdminMnsV1.Models
{
    public class User : IdentityUser
    {
        // Propriétés de base pour tous les utilisateurs
        // Ces propriétés sont communes à tous les types d'utilisateurs (Expert, Student, etc.)
        // Elles sont définies ici pour éviter la duplication de code dans les classes dérivées.

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Sexe { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public DateTime CreationDate { get; set; }
        public string? Phone { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Status { get; set; } // "Candidat" ou "Stagiaire"
        public bool IsDeleted { get; set; } = false; // Propriété pour gérer la suppression logique



        //STUDENT
        public string? Nationality { get; set; }
        public string? SocialSecurityNumber { get; set; }
        public string? FranceTravailNumber { get; set; }
        public string? Photo { get; set; }

        //Le ? après string signifie que cette propriété peut accepter la valeur null

        //EXPERTS 
        public string? Speciality { get; set; }

        //ADMIN 
        public string? Service { get; set; }


        // --- Propriété AJOUTÉE pour la relation plusieurs-à-plusieurs via Attend ---
        // Collection des inscriptions (Attend) pour cet utilisateur
        public virtual ICollection<Attend> Attends { get; set; } = new List<Attend>();
        // --- FIN de la propriété AJOUTÉE ---
    }


}

