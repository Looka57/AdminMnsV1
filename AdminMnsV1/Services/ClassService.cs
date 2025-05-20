// Services/ClassService.cs
using AdminMnsV1.Models; // Pour CardModel
using AdminMnsV1.Repositories.Interfaces; // Pour IClassRepository
using AdminMnsV1.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminMnsV1.Services
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepository;

        public ClassService(IClassRepository classRepository)
        {
            _classRepository = classRepository;
        }

        public async Task<List<CardModel>> GetClassCardModelsAsync()
        {
            // Récupère les classes avec leur nombre d'étudiants via le repository
            // Note: GetClassesWithStudentCountsAsync retourne un List<object> avec Class et StudentCount
            var classStudentCounts = await _classRepository.GetClassesWithStudentCountsAsync();

            // Logique de mapping des icônes (déplacée depuis le contrôleur)
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

            // Transformation des données en une liste de CardModel
            var classCards = classStudentCounts.Select(item =>
            {
                // Utilise la réflexion pour accéder aux propriétés du type anonyme
                var classProperty = item.GetType().GetProperty("Class");
                var studentCountProperty = item.GetType().GetProperty("StudentCount");

                var classEntity = classProperty?.GetValue(item) as AdminMnsV1.Models.Classes.Class;
                var studentCount = (int)(studentCountProperty?.GetValue(item) ?? 0);

                if (classEntity != null && classIconMapping.TryGetValue(classEntity.NameClass, out var iconInfo))
                {
                    return new CardModel
                    {
                        Title = classEntity.NameClass,
                        Number = studentCount.ToString(),
                        Url = "../Classes/Class", // Assure-toi que cette URL est correcte
                        IconUrl = iconInfo.IconUrl,
                        AltText = iconInfo.AltText
                    };
                }
                else
                {
                    return new CardModel
                    {
                        Title = classEntity?.NameClass ?? "Classe Inconnue",
                        Number = studentCount.ToString(),
                        Url = "../Classes/Class",
                        IconUrl = "/images/logo/defaut.png",
                        AltText = "Icône par défaut"
                    };
                }
            }).ToList();

            return classCards;
        }
    }
}