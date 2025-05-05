using AdminMnsV1.Data;
using AdminMnsV1.Models.Experts; //  ce namespace pour ExpertEditViewModel
using AdminMnsV1.Models.Students;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace AdminMnsV1.Controllers
{

    //*************RECUPERE LA LISTE DES EXPERTS **********
    public class ExpertsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpertsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // Récupère tous les utilisateurs qui sont de type Expert et non supprimés
        public IActionResult Expert()
        {
            // Récupère tous les utilisateurs qui sont de type Expert
            var experts = _context.Users
                .OfType<Expert>()
                .Where(e => !e.IsDeleted)
                .ToList();

            // Crée une liste de ExpertEditViewModel à partir de la liste des Experts
            var expertViewModels = experts.Select(e => new ExpertEditViewModel // Utilisez votre ViewModel pour l'affichage des experts
            {
                UserId = e.UserId,
                LastName = e.LastName,
                FirstName = e.FirstName,
                Sexe = e.Sexe,
                BirthDate = e.BirthDate,
                Address = e.Address,
                City = e.City,
                Email = e.Email,
                Phone = e.Phone,
                CreationDate = e.CreationDate,
                Speciality = e.Speciality, // Propriétés spécifiques à Expert


            }).ToList();

            // Passe la liste des ExpertEditViewModel à la vue nommée "Expert"
            return View(expertViewModels);
        }

        //*************CREATION DUN NOUVEAU INTERVENANT**********

        [HttpPost]
        public IActionResult Create(ExpertCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newExpert = new Expert
                {
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    Sexe = model.Sexe,
                    BirthDate = model.BirthDate,
                    Address = model.Address,
                    City = model.City,
                    Email = model.Email,
                    Speciality = model.Speciality,
                    PasswordHash = model.Password,// Assignation directe du mot de passe (TEMPORAIRE)
                    Phone = model.Phone,
                    CreationDate = DateTime.Now,
                };

                _context.Users.Add(newExpert);
                _context.SaveChanges();

                TempData["SuccesMessage"] = "Le nouveau intervenant a été créé avec succès.";
                return RedirectToAction("Expert");
            }
            else
            {
                return View("~/Views/Experts/FormulaireExpert.cshtml", model);
            }
        }
    }



}
