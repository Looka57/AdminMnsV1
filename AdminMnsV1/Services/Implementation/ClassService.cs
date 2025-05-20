// Services/ClassService.cs
using AdminMnsV1.Models; // Pour CardModel, Class
using AdminMnsV1.Models.Classes; // Pour Class
using AdminMnsV1.Models.Students; // Pour Student
using AdminMnsV1.Models.ViewModels; // IMPORTANT : Pour ClassListViewModel
using AdminMnsV1.Repositories.Interfaces; // Pour IClassRepository, IStudentRepository
using AdminMnsV1.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminMnsV1.Services.Implementation
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepository;
        private readonly IStudentRepository _studentRepository; // Injectez le StudentRepository

        public ClassService(IClassRepository classRepository, IStudentRepository studentRepository)
        {
            _classRepository = classRepository;
            _studentRepository = studentRepository;
        }

        // Nouvelle méthode pour obtenir le ViewModel complet de la page des classes
        public async Task<ClassListViewModel> GetClassListPageViewModelAsync()
        {
            // 1. Récupérer les informations des classes avec le nombre d'élèves
            var classStudentCounts = await _classRepository.GetClassesWithStudentCountsAsync();

            // 2. Récupérer TOUS les élèves avec leurs détails de classe
            // Ceci est essentiel pour pouvoir les filtrer ensuite par classe
            var allStudents = await _studentRepository.GetAllStudentsWithDetailsAsync();

            // Logique de mapping des icônes (peut être déplacée dans un fichier de configuration si elle devient trop longue)
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

            var classesWithStudentsList = new List<ClassWithStudentsViewModel>();

            // Parcourir chaque classe pour construire les données
            foreach (var item in classStudentCounts)
            {
                // Accéder aux propriétés de l'objet anonyme (Class et StudentCount)
                var classProperty = item.GetType().GetProperty("Class");
                var studentCountProperty = item.GetType().GetProperty("StudentCount");

                var classEntity = classProperty?.GetValue(item) as SchoolClass;
                var studentCount = (int)(studentCountProperty?.GetValue(item) ?? 0);

                if (classEntity != null)
                {
                    var (iconUrl, altText) = classIconMapping.GetValueOrDefault(classEntity.NameClass, ("/images/logo/defaut.png", "Icône par défaut"));

                    // Créer la CardModel pour cette classe
                    var card = new CardModel
                    {
                        Title = classEntity.NameClass,
                        Number = studentCount.ToString(),
                        Url = "../Classes/Class", // Assurez-vous que cette URL est correcte
                        IconUrl = iconUrl,
                        AltText = altText
                    };

                    // Filtrer les élèves appartenant à cette classe spécifique
                    var studentsInThisClass = allStudents
                        .Where(s => s.Attends != null && s.Attends.Any(a => a.ClasseId == classEntity.ClasseId))
                        .ToList();

                    // Ajouter l'objet ClassWithStudentsViewModel à la liste
                    classesWithStudentsList.Add(new ClassWithStudentsViewModel
                    {
                        ClassCard = card,
                        StudentsInClass = studentsInThisClass,
                        ClassId = classEntity.ClasseId // L'ID de la classe
                    });
                }
            }

            // Retourner le ViewModel complet
            return new ClassListViewModel
            {
                ClassesWithStudents = classesWithStudentsList
            };
        }

        // Gardez cette méthode si elle est utilisée ailleurs, sinon vous pouvez la supprimer
        public async Task<List<CardModel>> GetClassCardModelsAsync()
        {
            // Votre implémentation existante pour récupérer les CardModels
            // ... (potentiellement juste appeler GetClassListPageViewModelAsync et prendre ClassCards)
            return new List<CardModel>(); // Placeholder, à remplacer par votre logique existante
        }
    }
}