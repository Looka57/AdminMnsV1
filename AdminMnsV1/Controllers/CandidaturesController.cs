using Microsoft.AspNetCore.Mvc;
using AdminMnsV1.Services.Interfaces; // Pour ICandidatureService, IDocumentTypeService, IClassService
using AdminMnsV1.Application.Services.Interfaces;
using AdminMnsV1.Models.Candidature; // Pour Candidature et CreateCandidatureViewModel
using Microsoft.AspNetCore.Authorization; // Pour l'autorisation (si tu l'utilises)
using System.Security.Claims; // Pour obtenir l'ID de l'utilisateur connecté

namespace AdminMnsV1.Web.Controllers
{
    // [Authorize] // Applique si tu veux que seuls les utilisateurs connectés puissent accéder
    public class CandidaturesController : Controller
    {
        private readonly ICandidatureService _candidatureService;
        private readonly IDocumentTypeService _documentTypeService; // Pour les types de documents nécessaires à la création
        private readonly IClassService _classService; // Pour les classes disponibles
        private readonly IDocumentService _documentService; // Ajout pour la gestion des documents depuis ce contrôleur





        public IActionResult Candidature()
        {
            return View();
        }
    }
}
