using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
namespace AdminMnsV1.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }


        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string Sexe { get; set; }

        [Required]
        [MaxLength(250)]

        [EmailAddress]
        public string Email { get; set; }

        [Required] //autre systeme obligatoire pour l'insertion + communication
        [MaxLength(250)]
        public string PasswordHash { get; set; }

        [MaxLength(150)]
        public string Address { get; set; }

        [MaxLength(50)]
        public string City { get; set; }

        public DateTime CreationDate { get; set; }
        [MaxLength(50)]

        public string Phone { get; set; }
        public DateTime BirthDate { get; set; }

        // Propriété de discrimination pour EF Core
        public string Discriminator { get; set; }
    }
}

