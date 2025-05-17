using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AdminMnsV1.Data;
using AdminMnsV1.Models;

namespace AdminMnsV1.Controllers
{
    public class ClassesController : Controller
    {

        private readonly ApplicationDbContext _context;

        public ClassesController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Class()
        {
            //recuper le nombre d'eleve par classes 
            var classStudentCounts = _context.Attends
           .Include(a => a.Class)
           .Where(a => a.Class != null && a.Student.Status =="Stagiaire" && !a.Student.IsDeleted )
           .GroupBy(a => a.Class)
           .Select(g => new
           {
               Class = g.Key, // L'entité Class
               StudentCount = g.Count() 
           })
           .ToList();

            var classIconMapping = new Dictionary<string, (string IconUrl, string AltText)>
        {
            { "CDA", ("/images/logo/cda_logo.png", "Logo CDA") },
            { "C Sharp", ("/images/logo/c-sharp-logo.png", "Logo C#") },
            { "Java", ("/images/logo/java-logo.png", "java icon") },
            { "devweb1", ("/images/logo/dev_logo.png", "web icon") },
            { "DevWeb2", ("/images/logo/dev_logo.png", "web icon") },
            { "réseau", ("/images/logo/reseau_logo.png", "reseau icon") },
            { "fullstack", ("/images/logo/reseau_logo.png", "reseau icon") },
            { "Ran", ("/images/logo/ran_logo.png", "ran icon") },
            { "Ran2", ("/images/logo/ran_logo.png", "ran icon") },
            { "Ran3", ("/images/logo/ran_logo.png", "ran icon") },
            { "Ran4", ("/images/logo/ran_logo.png", "ran icon") },
        };
            var classCards = classStudentCounts.Select(item =>
            {                
                if (classIconMapping.TryGetValue(item.Class.NameClass, out var iconInfo)) 
                {
                    // Si la classe est trouvée dans le dictionnaire
                    return new CardModel
                    {
                        Title = item.Class.NameClass, // Nom de la classe de la BDD 
                                                      // Title = c.NameClass, // Nom de la classe de la BDD 
                        Number = item.StudentCount.ToString(), // Nombre d'élèves calculé
                                                               // Number = counts.TryGetValue(c.ClasseId, out var count) ? count.ToString() : "0", // Nombre d'élèves 
                        Url = "../Classes/Class", // URL de la carte 
                        IconUrl = iconInfo.IconUrl, // <<< Obtient l'URL de l'icône depuis le dictionnaire
                        AltText = iconInfo.AltText // <<< Obtient le AltText depuis le dictionnaire
                    };
                }
                else
                {
                    // Si la classe n'est PAS trouvée dans le dictionnaire (classe non prévue ou nouvelle)
                    // Vous pouvez utiliser une icône par défaut ou gérer l'absence d'icône.
                    return new CardModel
                    {
                        Title = item.Class.NameClass, // Nom de la classe de la BDD 
                                                      // Title = c.NameClass, // Nom de la classe de la BDD 
                        Number = item.StudentCount.ToString(), // Nombre d'élèves calculé 
                                                               // Number = counts.TryGetValue(c.ClasseId, out var count) ? count.ToString() : "0", 
                        Url = "../Classes/Class",
                        IconUrl = "/images/logo/defaut.png", // <<< URL d'une icône par défaut
                        AltText = "Icône par défaut" // <<< Texte alternatif par défaut
                    };
                }
            }).ToList();


            return View(classCards);
        }
    }
}
