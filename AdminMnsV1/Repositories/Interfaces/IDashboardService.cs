// Interfaces/IDashboardService.cs
using System.Security.Claims; // Pour ClaimsPrincipal pour l'utilisateur connecté
using AdminMnsV1.ViewModels; // Pour DashboardViewModel
using System.Threading.Tasks; // Pour les opérations asynchrones (qui ne bloquent pas le programme)


//Une interface en C# est comme un contrat ou un plan. Elle définit ce qu'une classe doit faire, sans dire comment elle doit le faire.

namespace AdminMnsV1.Interfaces
{
    public interface IDashboardService // Déclaration de l'interface publique
    {
        
        Task<DashboardViewModel> GetAdminDashboardDataAsync(ClaimsPrincipal userPrincipal);
        //Cette ligne définit une "promesse". Elle dit : "Toute classe qui veut être un IDashboardService doit avoir une méthode appelée GetAdminDashboardDataAsync. Cette méthode prendra en paramètre les informations de l'utilisateur (userPrincipal) et elle me retournera un DashboardViewModel de manière asynchrone."

        Task<DashboardViewModel> GetStudentDashboardDataAsync(ClaimsPrincipal userPrincipal);
        //C'est la même chose, mais pour le tableau de bord des stagiaires.
    }
}



//L'objectif ici est de créer une couche d'abstraction. Votre contrôleur saura qu'il a besoin d'un IDashboardService pour obtenir les données, mais il ne se souciera pas de savoir quelle implémentation spécifique il utilise, ni comment cette implémentation récupère les données. C'est le principe de l'injection de dépendances et de l'inversion de contrôle.