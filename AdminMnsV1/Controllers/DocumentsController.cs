using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminMnsV1.Data;
using AdminMnsV1.Models.Documents;
using AdminMnsV1.Application.Services.Interfaces; // Utilisation de l'interface du service
using Microsoft.AspNetCore.Identity;
using AdminMnsV1.Models; // Pour User
using Microsoft.AspNetCore.Mvc.Rendering;
using AdminMnsV1.Services.Interfaces; // Pour SelectList

namespace AdminMnsV1.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly IDocumentService _documentService;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context; // Toujours besoin pour les ViewDatas des SelectList

        public DocumentsController(IDocumentService documentService, UserManager<User> userManager, ApplicationDbContext context)
        {
            _documentService = documentService;
            _userManager = userManager;
            _context = context; // Garder le context pour les SelectLists
        }

        // GET: Documents
        public async Task<IActionResult> Index()
        {
            var documents = await _documentService.GetAllDocumentsAsync(); // Appelle la méthode du service
            return View(documents);
        }

        // GET: Documents/Create
        public async Task<IActionResult> Create()
        {
            // Charger les listes déroulantes nécessaires
            ViewData["DocumentTypeId"] = new SelectList(await _context.DocumentTypes.ToListAsync(), "DocumentTypeId", "DocumentTypeName");
            ViewData["DocumentStatusId"] = new SelectList(await _context.DocumentStatuses.ToListAsync(), "DocumentStatusId", "DocumentStatusName");
            ViewData["CandidatureId"] = new SelectList(await _context.Candidatures.ToListAsync(), "CandidatureId", "CandidatureName"); // Assure-toi que Candidature a une prop CandidatureName pour l'affichage

            return View();
        }

        // POST: Documents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DocumentName,DocumentTypeId,DocumentStatusId,CandidatureId")] Documents document, IFormFile documentFile)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                ModelState.AddModelError(string.Empty, "Utilisateur non authentifié.");
                // Recharger les ViewDatas en cas d'erreur
                ViewData["DocumentTypeId"] = new SelectList(await _context.DocumentTypes.ToListAsync(), "DocumentTypeId", "DocumentTypeName", document.DocumentTypeId);
                ViewData["DocumentStatusId"] = new SelectList(await _context.DocumentStatuses.ToListAsync(), "DocumentStatusId", "DocumentStatusName", document.DocumentStatusId);
                ViewData["CandidatureId"] = new SelectList(await _context.Candidatures.ToListAsync(), "CandidatureId", "CandidatureName", document.CandidatureId);
                return View(document);
            }

            // Appelle la nouvelle méthode CreateDocumentAsync du service
            var result = await _documentService.CreateDocumentAsync(document, documentFile, currentUser.Id);

            if (result.Success)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                // Recharger les ViewDatas en cas d'erreur
                ViewData["DocumentTypeId"] = new SelectList(await _context.DocumentTypes.ToListAsync(), "DocumentTypeId", "DocumentTypeName", document.DocumentTypeId);
                ViewData["DocumentStatusId"] = new SelectList(await _context.DocumentStatuses.ToListAsync(), "DocumentStatusId", "DocumentStatusName", document.DocumentStatusId);
                ViewData["CandidatureId"] = new SelectList(await _context.Candidatures.ToListAsync(), "CandidatureId", "CandidatureName", document.CandidatureId);
                return View(document);
            }
        }
        // TODO: Ajouter des actions pour Details, Edit, Delete
    }
}