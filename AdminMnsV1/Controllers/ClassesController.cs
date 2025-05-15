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
           .Where(a => a.Class != null)
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
            var classCards = classStudentCounts.Select(item => // Si vous utilisez l'option 1 (GroupBy simple)
                                                               // var classCards = allClassEntities.Select(c => // Si vous utilisez l'option 1 étendue (avec classes vides)
            {
                // Tenter de trouver les informations de l'icône dans le dictionnaire en utilisant le nom de la classe
                if (classIconMapping.TryGetValue(item.Class.NameClass, out var iconInfo)) // Si Option 1 (GroupBy simple)
                                                                                          // if (classIconMapping.TryGetValue(c.NameClass, out var iconInfo)) // Si Option 1 étendue
                {
                    // Si la classe est trouvée dans le dictionnaire
                    return new CardModel
                    {
                        Title = item.Class.NameClass, // Nom de la classe de la BDD (si Option 1 simple)
                                                      // Title = c.NameClass, // Nom de la classe de la BDD (si Option 1 étendue)
                        Number = item.StudentCount.ToString(), // Nombre d'élèves calculé (si Option 1 simple)
                                                               // Number = counts.TryGetValue(c.ClasseId, out var count) ? count.ToString() : "0", // Nombre d'élèves (si Option 1 étendue)
                        Url = "../Classes/Class", // URL de la carte (peut être dynamique si besoin)
                        IconUrl = iconInfo.IconUrl, // <<< Obtient l'URL de l'icône depuis le dictionnaire
                        AltText = iconInfo.AltText // <<< Obtient le AltText depuis le dictionnaire
                                                   // Ajoutez d'autres propriétés de CardModel si besoin
                    };
                }
                else
                {
                    // Si la classe n'est PAS trouvée dans le dictionnaire (classe non prévue ou nouvelle)
                    // Vous pouvez utiliser une icône par défaut ou gérer l'absence d'icône.
                    return new CardModel
                    {
                        Title = item.Class.NameClass, // Nom de la classe de la BDD (si Option 1 simple)
                                                      // Title = c.NameClass, // Nom de la classe de la BDD (si Option 1 étendue)
                        Number = item.StudentCount.ToString(), // Nombre d'élèves calculé (si Option 1 simple)
                                                               // Number = counts.TryGetValue(c.ClasseId, out var count) ? count.ToString() : "0", // Nombre d'élèves (si Option 1 étendue)
                        Url = "../Classes/Class",
                        IconUrl = "/images/logo/defaut.png", // <<< URL d'une icône par défaut
                        AltText = "Icône par défaut" // <<< Texte alternatif par défaut
                                                     // Ajoutez d'autres propriétés de CardModel si besoin
                    };
                }
            }).ToList();


            return View(classCards);
        }
    }
}
