// ClassesControllerTests.cs

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AdminMnsV1.Controllers; // N'oubliez pas d'importer le bon namespace pour ClassesController
using AdminMnsV1.Data;
using AdminMnsV1.Models;
using AdminMnsV1.Models.Classes;
using AdminMnsV1.Models.Students;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminMnsV1.Tests
{
    [TestClass]
    public class ClassesControllerTests
    {
        private ApplicationDbContext _mockContext; // On utilise une instance réelle de DbContext en mémoire
        // Pas besoin de UserManager/RoleManager pour ce contrôleur/action, car il n'est pas utilisé ici

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Nom unique pour chaque test
                .Options;
            _mockContext = new ApplicationDbContext(options);

            // Populez la base de données en mémoire avec des données de test
            SeedDatabase();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _mockContext.Database.EnsureDeleted(); // Supprime la base de données en mémoire après chaque test
            _mockContext.Dispose();
        }

        private void SeedDatabase()
        {
            // Crée des classes
            var classeCda = new Class { ClasseId = 1, NameClass = "CDA", AcademicYear = 2024, StartDate = new DateOnly(2024, 9, 1), EndDate = new DateOnly(2025, 6, 30) };
            var classeJava = new Class { ClasseId = 2, NameClass = "Java", AcademicYear = 2024, StartDate = new DateOnly(2024, 9, 1), EndDate = new DateOnly(2025, 6, 30) };
            var classeReseau = new Class { ClasseId = 3, NameClass = "réseau", AcademicYear = 2024, StartDate = new DateOnly(2024, 9, 1), EndDate = new DateOnly(2025, 6, 30) };
            var classeEmpty = new Class { ClasseId = 4, NameClass = "ClasseVide", AcademicYear = 2024, StartDate = new DateOnly(2024, 9, 1), EndDate = new DateOnly(2025, 6, 30) };
            var classeOld = new Class { ClasseId = 5, NameClass = "AncienneClasse", AcademicYear = 2023, StartDate = new DateOnly(2023, 1, 1), EndDate = new DateOnly(2023, 12, 31) };

            _mockContext.Classs.AddRange(classeCda, classeJava, classeReseau, classeEmpty, classeOld);

            // Crée des élèves
            var student1 = new Student { Id = "s1", FirstName = "Alice", LastName = "Dupont", Status = "Stagiaire", IsDeleted = false };
            var student2 = new Student { Id = "s2", FirstName = "Bob", LastName = "Martin", Status = "Stagiaire", IsDeleted = false };
            var student3 = new Student { Id = "s3", FirstName = "Charlie", LastName = "Durand", Status = "Stagiaire", IsDeleted = false };
            var student4 = new Student { Id = "s4", FirstName = "David", LastName = "Lefevre", Status = "Terminé", IsDeleted = false }; // Non Stagiaire
            var student5 = new Student { Id = "s5", FirstName = "Eve", LastName = "Bernard", Status = "Stagiaire", IsDeleted = true }; // IsDeleted
            var student6 = new Student { Id = "s6", FirstName = "Frank", LastName = "Dubois", Status = "Stagiaire", IsDeleted = false };


            _mockContext.Students.AddRange(student1, student2, student3, student4, student5, student6);

            // Crée des associations Attends
            _mockContext.Attends.Add(new Attend { StudentId = student1.Id, ClasseId = classeCda.ClasseId });
            _mockContext.Attends.Add(new Attend { StudentId = student2.Id, ClasseId = classeCda.ClasseId });
            _mockContext.Attends.Add(new Attend { StudentId = student3.Id, ClasseId = classeJava.ClasseId });
            _mockContext.Attends.Add(new Attend { StudentId = student4.Id, ClasseId = classeCda.ClasseId }); // Non Stagiaire
            _mockContext.Attends.Add(new Attend { StudentId = student5.Id, ClasseId = classeJava.ClasseId }); // IsDeleted
            _mockContext.Attends.Add(new Attend { StudentId = student6.Id, ClasseId = classeReseau.ClasseId });

            _mockContext.SaveChanges();
        }

        // --- Tests pour l'action Class() ---

        [TestMethod]
        public void Class_ReturnsViewWithCorrectCardModels()
        {
            // Arrange
            var controller = new ClassesController(_mockContext);

            // Act
            var result = controller.Class() as ViewResult;

            // Assert
            Assert.IsNotNull(result, "Le résultat de l'action ne devrait pas être nul.");
            Assert.IsInstanceOfType(result, typeof(ViewResult), "Le résultat devrait être un ViewResult.");

            var model = result.Model as IEnumerable<CardModel>;
            Assert.IsNotNull(model, "Le modèle de la vue ne devrait pas être nul.");
            Assert.IsInstanceOfType(model, typeof(IEnumerable<CardModel>), "Le modèle devrait être de type IEnumerable<CardModel>.");

            // Vérifier le nombre total de cartes générées (devrait exclure les classes sans stagiaires qualifiés ou les classes "anciennes" si la logique de regroupement est basée sur les stagiaires actifs)
            // Dans votre contrôleur, vous groupez par `Class`. Donc toutes les classes avec au moins 1 étudiant qualifié auront une carte.
            // CDA a 2 stagiaires actifs, Java a 1, Réseau a 1. ClasseVide a 0. AncienneClasse a 0.
            Assert.AreEqual(3, model.Count(), "Le nombre de cartes générées devrait être correct (CDA, Java, Réseau).");

            // Vérifier la carte pour "CDA"
            var cdaCard = model.FirstOrDefault(c => c.Title == "CDA");
            Assert.IsNotNull(cdaCard, "La carte CDA devrait exister.");
            Assert.AreEqual("2", cdaCard.Number, "La classe CDA devrait avoir 2 stagiaires actifs."); // student1 et student2

            // Vérifier la carte pour "Java"
            var javaCard = model.FirstOrDefault(c => c.Title == "Java");
            Assert.IsNotNull(javaCard, "La carte Java devrait exister.");
            Assert.AreEqual("1", javaCard.Number, "La classe Java devrait avoir 1 stagiaire actif."); // student3

            // Vérifier la carte pour "réseau"
            var reseauCard = model.FirstOrDefault(c => c.Title == "réseau");
            Assert.IsNotNull(reseauCard, "La carte réseau devrait exister.");
            Assert.AreEqual("1", reseauCard.Number, "La classe réseau devrait avoir 1 stagiaire actif."); // student6


            // Vérifier que les icônes par défaut sont utilisées pour les classes non mappées (si vous en ajoutez)
            // Si vous n'avez pas de classes non mappées dans SeedDatabase qui apparaissent dans le résultat, ce test est moins critique ici.
            // Ou si vous testez une classe non mappée qui DOIT apparaître si elle a des étudiants.
            // Exemple : si vous avez une classe "Inconnue" avec des stagiaires, vous vérifieriez qu'elle a l'icône par défaut.
        }

        [TestMethod]
        public void Class_HandlesNoStudents_ReturnsEmptyOrCorrectCounts()
        {
            // Arrange
            // Nettoie la base de données après le Seed initial pour tester un scénario spécifique
            _mockContext.Attends.RemoveRange(_mockContext.Attends);
            _mockContext.Students.RemoveRange(_mockContext.Students);
            _mockContext.SaveChanges();

            // Crée des classes mais sans aucun stagiaire associé
            _mockContext.Classs.Add(new Class { ClasseId = 10, NameClass = "ClasseTest", AcademicYear = 2024, StartDate = new DateOnly(2024, 9, 1), EndDate = new DateOnly(2025, 6, 30) });
            _mockContext.SaveChanges();

            var controller = new ClassesController(_mockContext);

            // Act
            var result = controller.Class() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as IEnumerable<CardModel>;
            Assert.IsNotNull(model);
            Assert.AreEqual(0, model.Count(), "Aucune carte ne devrait être générée si aucune classe n'a de stagiaires actifs.");
        }

        [TestMethod]
        public void Class_HandlesStudentsWithIsDeletedTrue_ExcludesThemFromCount()
        {
            // Ce scénario est déjà couvert par le premier test avec student5 (IsDeleted = true)
            // Mais un test dédié peut être plus clair si nécessaire.
            // Arrange (seedDatabase est déjà prêt)
            var controller = new ClassesController(_mockContext);

            // Act
            var result = controller.Class() as ViewResult;
            var model = result.Model as IEnumerable<CardModel>;

            // Assert
            // Vérifier que student5 (IsDeleted=true) n'a pas été compté dans la classe Java
            var javaCard = model.FirstOrDefault(c => c.Title == "Java");
            Assert.IsNotNull(javaCard);
            Assert.AreEqual("1", javaCard.Number, "La classe Java ne devrait compter que les stagiaires non supprimés."); // student3
        }

        [TestMethod]
        public void Class_HandlesStudentsWithNonStagiaireStatus_ExcludesThemFromCount()
        {
            // Ce scénario est déjà couvert par le premier test avec student4 (Status = "Terminé")
            // Mais un test dédié peut être plus clair si nécessaire.
            // Arrange (seedDatabase est déjà prêt)
            var controller = new ClassesController(_mockContext);

            // Act
            var result = controller.Class() as ViewResult;
            var model = result.Model as IEnumerable<CardModel>;

            // Assert
            // Vérifier que student4 (Status="Terminé") n'a pas été compté dans la classe CDA
            var cdaCard = model.FirstOrDefault(c => c.Title == "CDA");
            Assert.IsNotNull(cdaCard);
            Assert.AreEqual("2", cdaCard.Number, "La classe CDA ne devrait compter que les stagiaires avec le statut 'Stagiaire'."); // student1 et student2
        }

        // Si votre vue avait des données dynamiques pour les élèves (pas juste le count),
        // vous auriez besoin d'une action ClassesController.StudentsInClass(int id)
        // et un test pour cette action. Mais d'après votre vue, les noms d'élèves sont statiques.
    }
}