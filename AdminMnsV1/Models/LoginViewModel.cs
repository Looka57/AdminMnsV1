using System.ComponentModel.DataAnnotations; // Nécessaire pour les attributs [Required], [EmailAddress], etc.

namespace AdminMnsV1.Models // Assurez-vous que ce namespace correspond à celui de votre projet
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "L'email est requis.")]
        [EmailAddress(ErrorMessage = "Format d'email invalide.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est requis.")]
        [DataType(DataType.Password)] // Pour masquer le texte dans les formulaires et donner des indications aux navigateurs
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Se souvenir de moi ?")] // Texte affiché pour la case à cocher "Remember Me"
        public bool RememberMe { get; set; }
    }
}