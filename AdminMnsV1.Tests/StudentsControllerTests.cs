using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AdminMnsV1.Controllers;
using Microsoft.AspNetCore.Identity;
using AdminMnsV1.Models;
using AdminMnsV1.Data;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AdminMnsV1.Models.Students;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using static System.Net.Mime.MediaTypeNames;




namespace AdminMnsV1.Tests
{
    [TestClass]
    public class StudentsControllerTests
    {
        private Mock<UserManager<User>> _mockUserManager;
        private Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private ApplicationDbContext _mockContext;
        private Mock<IWebHostEnvironment> _mockEnvironment;

        [TestInitialize]
        public void Setup()
        {
            var userStore = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);

            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _mockContext = new ApplicationDbContext(options);

            _mockEnvironment = new Mock<IWebHostEnvironment>();
            _mockEnvironment.Setup(e => e.WebRootPath).Returns("wwwroot");

            _mockContext.Classs.Add(new AdminMnsV1.Models.Classes.SchoolClass { ClasseId = 1, NameClass = "Classe A" });
            _mockContext.Classs.Add(new AdminMnsV1.Models.Classes.SchoolClass { ClasseId = 2, NameClass = "Classe B" });
            _mockContext.SaveChanges();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _mockContext.Database.EnsureDeleted();
            _mockContext.Dispose();
        }


        //**************TEST UNITAIRE CREATION  STAGIAIRE***********
        [TestMethod]
        public async Task Create_Post_ValidModel_UserAndAttendCreatedAndRedirects()
        {
            // Arrange (Préparation)
            var controller = new StudentsController(_mockUserManager.Object, _mockRoleManager.Object, _mockContext, _mockEnvironment.Object);

            var tempData = new TempDataDictionary(
       new DefaultHttpContext(),
       Mock.Of<ITempDataProvider>());

            controller.TempData = tempData;

            var createModel = new StudentCreateViewModel
            {
                FirstName = "Alice",
                LastName = "Dupont",
                Email = "alice.dupont@example.com",
                Password = "SecurePassword123!",
                Sexe = "Female",
                BirthDate = new System.DateTime(2000, 1, 1),
                Nationality = "Française",
                Address = "10 Rue de la Paix",
                City = "Paris",
                Phone = "0612345678",
                SocialSecurityNumber = "123456789012345",
                FranceTravailNumber = "FT12345",
                Status = "Candidat",
                ClassId = 1
            };

            // Ajoute ceci dans ton test Create_Post_ValidModel_UserAndAttendCreatedAndRedirects()
            // Configure le mock UserManager pour simuler un succès lors de la création de l'utilisateur
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>())) // <-- Ta définition de la méthode mockée
                 .Callback<User, string>((user, password) =>
                 { // <-- ICI se trouve le .Callback()
                   // C'est le code qui s'exécute lorsque CreateAsync est appelé
                   // Simule l'assignation de l'ID par UserManager
                     user.Id = Guid.NewGuid().ToString();
                 })
                 .ReturnsAsync(IdentityResult.Success); // <-- Et ensuite tu définis ce que la méthode retourne
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(1);
            mockFile.Setup(f => f.FileName).Returns("test.png");
            createModel.PhotoFile = mockFile.Object;


            // Act (Action)
            // Retire le "as RedirectToActionResult" ici
            var result = await controller.Create(createModel);

            // Assert (Vérification)

            // 1. Vérifie que la méthode CreateAsync du UserManager a été appelée une fois.
            _mockUserManager.Verify(um => um.CreateAsync(It.IsAny<Student>(), It.IsAny<string>()), Times.Once());

            // 2. Vérifie que l'entrée Attend a été ajoutée dans la base de données en mémoire
            Assert.AreEqual(1, _mockContext.Attends.Count(), "Une entrée Attend devrait avoir été ajoutée.");
            Assert.AreEqual(createModel.ClassId, _mockContext.Attends.First().ClasseId, "L'ID de la classe dans Attend doit correspondre.");

            // 3. Vérifie que le résultat de l'action est une redirection (RedirectToActionResult)
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult), "Le résultat devrait être une redirection.");
            // 4. Caste le résultat vers le type RedirectToActionResult pour accéder à ses propriétés
            var redirectToActionResult = (RedirectToActionResult)result;

            // 5. Vérifie le nom de l'action de redirection sur l'instance de l'objet
            Assert.AreEqual("Student", redirectToActionResult.ActionName, "La redirection devrait être vers l'action 'Student'.");


        }

        [TestMethod]
        public async Task Create_Post_InvalidModel_ReturnsViewWithModelAndErrors()
        {
            // Arrange
            var controller = new StudentsController(_mockUserManager.Object, _mockRoleManager.Object, _mockContext, _mockEnvironment.Object);

            // Crée un modèle invalide (ex: FirstName est requis, mais on ne le définit pas ici)
            var createModel = new StudentCreateViewModel
            {
                // FirstName = "...", // Manquant pour provoquer une erreur de validation
                LastName = "Candidat",
                Email = "test@example.com",
                Password = "Password123!",
                Status = "Candidat",
                ClassId = 1
            };

            // Simule manuellement une erreur de validation du modèle dans ModelState
            controller.ModelState.AddModelError("FirstName", "Le prénom est requis.");

            // Act
            var result = await controller.Create(createModel);

            // Assert
            // 1. Vérifie que le résultat est une vue (ViewResult)
            Assert.IsInstanceOfType(result, typeof(ViewResult), "Le résultat devrait être une vue.");
            var viewResult = (ViewResult)result;

            // 2. Vérifie que le nom de la vue est correct
            Assert.AreEqual("~/Views/Students/Formulaire.cshtml", viewResult.ViewName, "La vue retournée devrait être 'Formulaire.cshtml'.");

            // 3. Vérifie que le modèle d'état est invalide
            Assert.IsFalse(controller.ModelState.IsValid, "Le ModelState devrait être invalide.");

            // 4. Vérifie que l'erreur spécifique est présente dans ModelState
            Assert.IsTrue(controller.ModelState.ContainsKey("FirstName"), "Le ModelState devrait contenir une erreur pour 'FirstName'.");
            Assert.AreEqual("Le prénom est requis.", controller.ModelState["FirstName"].Errors.First().ErrorMessage);

            // 5. Vérifie que UserManager.CreateAsync n'a PAS été appelé, car la validation a échoué en amont
            _mockUserManager.Verify(um => um.CreateAsync(It.IsAny<Student>(), It.IsAny<string>()), Times.Never());

            // ... (code précédent de ton test)

            // 6. Vérifie que le modèle renvoyé à la vue est le même que celui fourni (et contient les classes disponibles)

            // D'abord, on vérifie que le modèle est du bon type. Cette ligne ne retourne rien.
            Assert.IsInstanceOfType(viewResult.Model, typeof(StudentCreateViewModel), "Le modèle retourné à la vue devrait être de type StudentCreateViewModel.");

            // Ensuite, on caste le modèle pour l'utiliser dans la variable returnedModel
            var returnedModel = (StudentCreateViewModel)viewResult.Model;

            Assert.AreEqual(createModel, returnedModel, "Le modèle retourné à la vue devrait être le modèle d'entrée.");

            Assert.IsNotNull(returnedModel.AvailableClasses, "La liste des classes disponibles ne doit pas être nulle.");
            Assert.IsTrue(returnedModel.AvailableClasses.Any(), "La SelectList des classes disponibles devrait être populée.");
        }


        [TestMethod]
        public async Task Create_Post_UserManagerFails_ReturnsViewWithModelAndErrors()
        {
            // Arrange
            var controller = new StudentsController(_mockUserManager.Object, _mockRoleManager.Object, _mockContext, _mockEnvironment.Object);

            var createModel = new StudentCreateViewModel
            {
                FirstName = "Bob",
                LastName = "Martin",
                Email = "existing@example.com", // Cet email va causer l'échec de UserManager
                Password = "Password123!",
                Status = "Candidat",
                ClassId = 1
            };

            // Configure le mock UserManager pour simuler un échec
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<Student>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Email 'existing@example.com' est déjà pris." }));

            // Act
            var result = await controller.Create(createModel);

            // Assert
            // 1. Vérifie que le résultat est une vue (ViewResult)
            Assert.IsInstanceOfType(result, typeof(ViewResult), "Le résultat devrait être une vue.");
            var viewResult = (ViewResult)result;

            // 2. Vérifie le nom de la vue
            Assert.AreEqual("~/Views/Students/Formulaire.cshtml", viewResult.ViewName, "La vue retournée devrait être 'Formulaire.cshtml'.");

            // 3. Vérifie que le modèle d'état est invalide (à cause de l'erreur Identity)
            Assert.IsFalse(controller.ModelState.IsValid, "Le ModelState devrait être invalide après l'échec d'Identity.");

            // 4. Vérifie que l'erreur d'Identity est présente dans ModelState
            var modelStateErrors = controller.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            Assert.IsTrue(modelStateErrors.Contains("Email 'existing@example.com' est déjà pris."), "L'erreur spécifique d'Identity devrait être présente.");

            // 5. Vérifie que UserManager.CreateAsync a bien été appelé une fois (même s'il a échoué)
            _mockUserManager.Verify(um => um.CreateAsync(It.IsAny<Student>(), It.IsAny<string>()), Times.Once());

            // ... (code précédent de ton test)

            // 6. Vérifie que le modèle retourné à la vue est le même et contient les classes disponibles

            // 6a. D'abord, on s'assure que le modèle est bien de type StudentCreateViewModel.
            // Cette ligne est une assertion pure et ne retourne aucune valeur.
            Assert.IsInstanceOfType(viewResult.Model, typeof(StudentCreateViewModel), "Le modèle retourné devrait être de type StudentCreateViewModel.");

            // 6b. Ensuite, on caste explicitement le modèle pour pouvoir l'utiliser dans la suite des assertions.
            var returnedModel = (StudentCreateViewModel)viewResult.Model;

            Assert.IsNotNull(returnedModel.AvailableClasses, "La liste des classes disponibles ne doit pas être nulle.");
            Assert.IsTrue(returnedModel.AvailableClasses.Any(), "La SelectList des classes disponibles devrait être populée.");
        }


        //*******************TEST UNITAIRE MODIFICATION  STAGIAIRE***************


        [TestMethod]
        public async Task Modify_Post_InvalidModel_ReturnsRedirectWithErrorMessage()
        {
            // Arrange
            var controller = new StudentsController(_mockUserManager.Object, _mockRoleManager.Object, _mockContext, _mockEnvironment.Object);

            // Initialiser TempData
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            // Créer un modèle invalide (ex: FirstName manquant)
            var invalidModel = new StudentEditViewModel
            {
                UserId = "student1",
                LastName = "Doe",
                // FirstName manquant ou autre champ requis non renseigné
                Email = "test@example.com",
                Role = "Stagiaire",
                ClassId = 1
            };

            // Simuler manuellement une erreur de validation
            controller.ModelState.AddModelError("FirstName", "Le prénom est requis.");

            // Act
            var result = await controller.Modify(invalidModel);

            // Assert
            // 1. Vérifier que le résultat est une redirection
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult), "Le résultat devrait être une redirection.");
            var redirectToActionResult = (RedirectToActionResult)result;

            // 2. Vérifier que la redirection est vers l'action "Student"
            Assert.AreEqual("Student", redirectToActionResult.ActionName, "La redirection devrait être vers l'action 'Student'.");

            // 3. Vérifier que TempData contient le message d'erreur
            Assert.IsNotNull(controller.TempData["ErreurMessage"], "TempData devrait contenir un message d'erreur.");
            Assert.AreEqual("Informations invalides fournies.", controller.TempData["ErreurMessage"]);

            // 4. Vérifier qu'aucune opération de UserManager ou de BDD n'a été tentée
            _mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Never());
            _mockUserManager.Verify(um => um.UpdateAsync(It.IsAny<User>()), Times.Never());
            // Pour _mockContext.Attends.Count(), c'est directement sur l'instance InMemory, pas sur un mock.
            // Donc, on vérifie que la base de données est vide ou n'a pas été modifiée.
            Assert.AreEqual(0, _mockContext.Attends.Count(), "Aucune entrée Attend ne devrait exister ou avoir été modifiée.");
            // Si vous aviez mocké _mockContext, vous pourriez faire un Verify(c => c.SaveChangesAsync()...)
        }


        //PAS TROUVE DANS LA BDD
        [TestMethod]
        public async Task Modify_Post_StudentNotFound_ReturnsRedirectWithErrorMessage()
        {
            // Arrange
            var controller = new StudentsController(_mockUserManager.Object, _mockRoleManager.Object, _mockContext, _mockEnvironment.Object);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var model = new StudentEditViewModel
            {
                UserId = "nonexistent_id",
                LastName = "Doe",
                FirstName = "John",
                Email = "john.doe@example.com",
                Role = "Stagiaire",
                ClassId = 1
            };

            // Simuler que UserManager.FindByIdAsync ne trouve aucun utilisateur
            _mockUserManager.Setup(um => um.FindByIdAsync(model.UserId))
                            .ReturnsAsync((User)null); // Retourne null car l'utilisateur n'est pas trouvé

            // Act
            var result = await controller.Modify(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult), "Le résultat devrait être une redirection.");
            var redirectToActionResult = (RedirectToActionResult)result;

            Assert.AreEqual("Student", redirectToActionResult.ActionName, "La redirection devrait être vers l'action 'Student'.");
            Assert.IsNotNull(controller.TempData["ErreurMessage"], "TempData devrait contenir un message d'erreur.");
            Assert.AreEqual("Stagiaire non trouvé.", controller.TempData["ErreurMessage"]);

            // Vérifier que UpdateAsync n'a pas été appelé
            _mockUserManager.Verify(um => um.UpdateAsync(It.IsAny<User>()), Times.Never());
            // Vérifier que SaveChangesAsync n'a pas été appelé sur le contexte
            // Pour _mockContext (instance réelle InMemory), la vérification du Count() suffit
            Assert.AreEqual(0, _mockContext.Attends.Count(), "Aucune entrée Attend ne devrait avoir été ajoutée ou modifiée.");
        }

        //Ce test gère le cas où _userManager.UpdateAsync échoue, par exemple, à cause d'un e-mail en double ou d'une validation d'identité

        [TestMethod]
        public async Task Modify_Post_UserManagerFails_ReturnsRedirectWithErrorMessage()
        {
            // Arrange
            var controller = new StudentsController(_mockUserManager.Object, _mockRoleManager.Object, _mockContext, _mockEnvironment.Object);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var userId = Guid.NewGuid().ToString();
            var existingUser = new Student // Assurez-vous que Student hérite de User
            {
                Id = userId,
                FirstName = "Old",
                LastName = "Name",
                Email = "old@example.com",
                UserName = "old@example.com",
                CreationDate = DateTime.Now // Assurez-vous d'initialiser CreationDate
            };

            var model = new StudentEditViewModel
            {
                UserId = userId,
                FirstName = "New",
                LastName = "Name",
                Email = "duplicate@example.com", // Simule un e-mail en double
                Role = "Stagiaire",
                ClassId = 1
            };

            // Simuler que FindByIdAsync trouve l'utilisateur
            _mockUserManager.Setup(um => um.FindByIdAsync(userId))
                            .ReturnsAsync(existingUser);

            // Simuler que UpdateAsync échoue
            _mockUserManager.Setup(um => um.UpdateAsync(It.IsAny<User>()))
                            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Email 'duplicate@example.com' est déjà pris." }));

            // Act
            var result = await controller.Modify(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult), "Le résultat devrait être une redirection.");
            var redirectToActionResult = (RedirectToActionResult)result;

            Assert.AreEqual("Student", redirectToActionResult.ActionName, "La redirection devrait être vers l'action 'Student'.");
            Assert.IsNotNull(controller.TempData["ErreurMessage"], "TempData devrait contenir un message d'erreur.");
            StringAssert.Contains(controller.TempData["ErreurMessage"].ToString(), "Erreur Identity lors de la mise à jour du stagiaire", "Le message d'erreur doit indiquer un échec Identity.");

            // Vérifier que SaveChangesAsync n'a pas été appelé sur le contexte après l'échec de UserManager
            // Pas de SaveChangesAsync car l'échec d'Identity le précède.
            Assert.AreEqual(0, _mockContext.Attends.Count(), "Aucune entrée Attend ne devrait avoir été ajoutée ou modifiée.");
        }


        //Ce test vérifie le scénario de succès complet, y compris la mise à jour de l'utilisateur, la mise à jour de la classe Attends et la redirection

        [TestMethod]
        public async Task Modify_Post_ValidModel_SuccessAndRedirects()
        {
            // Arrange
            var controller = new StudentsController(_mockUserManager.Object, _mockRoleManager.Object, _mockContext, _mockEnvironment.Object);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var userId = Guid.NewGuid().ToString();
            var originalClassId = 1; // Classe initiale
            var updatedClassId = 2;  // Nouvelle classe après modification

            // Simuler un utilisateur existant dans la DB en mémoire
            var existingUser = new Student
            {
                Id = userId,
                FirstName = "Old",
                LastName = "User",
                Email = "old.user@example.com",
                UserName = "old.user@example.com",
                CreationDate = new DateTime(2023, 1, 1),
                Status = "Stagiaire"
            };
            // Ajouter l'entrée Attend initiale
            _mockContext.Attends.Add(new Attend { StudentId = userId, ClasseId = originalClassId });
            _mockContext.SaveChanges(); // Sauvegarder la setup initiale

            var model = new StudentEditViewModel
            {
                UserId = userId,
                FirstName = "Updated",
                LastName = "User",
                Email = "updated.user@example.com",
                Sexe = "Male",
                BirthDate = new DateTime(1995, 5, 10),
                Nationality = "Canadienne",
                Address = "20 New St",
                City = "Montreal",
                Phone = "0987654321",
                SocialSecurityNumber = "987654321098765",
                FranceTravailNumber = "FT98765",
                Role = "Stagiaire",
                ClassId = updatedClassId // Changement de classe
            };

            // Configurer _mockUserManager pour trouver l'utilisateur et réussir la mise à jour
            _mockUserManager.Setup(um => um.FindByIdAsync(userId))
                            .ReturnsAsync(existingUser);
            _mockUserManager.Setup(um => um.UpdateAsync(It.IsAny<User>()))
                            .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await controller.Modify(model);

            // Assert
            // 1. Vérifier le type de résultat et la redirection
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult), "Le résultat devrait être une redirection.");
            var redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual("Student", redirectToActionResult.ActionName, "La redirection devrait être vers l'action 'Student'.");

            // 2. Vérifier que le message de succès est dans TempData
            Assert.IsNotNull(controller.TempData["SuccesMessage"], "TempData devrait contenir un message de succès.");
            StringAssert.Contains(controller.TempData["SuccesMessage"].ToString(), "a été mis à jour avec succès.", "Le message de succès doit confirmer la mise à jour.");

            // 3. Vérifier que UserManager.FindByIdAsync a été appelé
            _mockUserManager.Verify(um => um.FindByIdAsync(userId), Times.Once());
            // 4. Vérifier que UserManager.UpdateAsync a été appelé
            _mockUserManager.Verify(um => um.UpdateAsync(It.IsAny<User>()), Times.Once());

            // 5. Vérifier les modifications dans la DB en mémoire (Attends)
            // L'ancienne entrée devrait avoir été supprimée
            Assert.IsFalse(_mockContext.Attends.Any(a => a.StudentId == userId && a.ClasseId == originalClassId), "L'ancienne liaison de classe devrait avoir été supprimée.");
            // La nouvelle entrée devrait avoir été ajoutée
            Assert.IsTrue(_mockContext.Attends.Any(a => a.StudentId == userId && a.ClasseId == updatedClassId), "La nouvelle liaison de classe devrait avoir été ajoutée.");

            // 6. Vérifier que SaveChangesAsync a été appelé pour le contexte de la base de données
            // Comme _mockContext est une instance réelle (InMemory), la vérification du contenu
            // de la base en mémoire (points 5) est une preuve suffisante que SaveChangesAsync a été appelé.
        }


        //************TEST UNITAIRE SUPPRESSION  STAGIAIRE***********
        [TestMethod]

        public async Task Delete_Post_StudentNotFound_ReturnsRedirectWithErrorMessage()
        {
            //Arrange 
            var controller = new StudentsController(_mockUserManager.Object, _mockRoleManager.Object, _mockContext, _mockEnvironment.Object);
            // Initialiser TempData car le contrôleur l'utilise pour les messages d'erreur
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var userIdToDelete = "nonexistent_id"; // ID d'un utilisateur qui n'existe pas

            // Simuler que UserManager.FindByIdAsync ne trouve pas l'utilisateur
            _mockUserManager.Setup(um => um.FindByIdAsync(userIdToDelete))
                            .ReturnsAsync((User)null); // Retourne null car l'utilisateur n'est pas trouvé

            //Act
            var result = await controller.Delete(userIdToDelete);

            //Assert
            // 1. Vérifier que le résultat est une redirection
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult), "Le résultat devrait être une redirection.");
            var redirectToActionResult = (RedirectToActionResult)result;

            // 2. Vérifier que la redirection est vers l'action "Student"
            Assert.AreEqual("Student", redirectToActionResult.ActionName, "La redirection devrait être vers l'action 'Student'.");

            // 3. Vérifier que TempData contient le message d'erreur
            Assert.IsNotNull(controller.TempData["ErreurMessage"], "TempData devrait contenir un message d'erreur.");
            Assert.AreEqual("Stagiaire non trouvé.", controller.TempData["ErreurMessage"]);

            // 4. Vérifier que UserManager.DeleteAsync n'a pas été appelé
            _mockUserManager.Verify(um => um.DeleteAsync(It.IsAny<User>()), Times.Never());

            // 5. Vérifier que SaveChangesAsync n'a pas été appelé sur le contexte
            Assert.AreEqual(0, _mockContext.Attends.Count());


        }


        //Ce test gère le cas où _userManager.DeleteAsync échoue, par exemple, à cause d'une contrainte d'intégrité non gérée ou d'une autre raison.

        // ... (dans la classe StudentsControllerTests.cs) ...

        [TestMethod]
        public async Task Delete_Post_UserManagerUpdateFails_ReturnsRedirectWithErrorMessage()
        {
            // Arrange
            var controller = new StudentsController(_mockUserManager.Object, _mockRoleManager.Object, _mockContext, _mockEnvironment.Object);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var userIdToDelete = Guid.NewGuid().ToString();
            var existingUser = new Student { Id = userIdToDelete, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", IsDeleted = false }; // Assurez-vous IsDeleted=false initialement

            // Simuler que FindByIdAsync trouve l'utilisateur
            _mockUserManager.Setup(um => um.FindByIdAsync(userIdToDelete))
                            .ReturnsAsync(existingUser);

            // Simuler que UpdateAsync échoue (car c'est UpdateAsync qui est appelé dans le contrôleur)
            _mockUserManager.Setup(um => um.UpdateAsync(It.Is<User>(u => u.Id == userIdToDelete && u.IsDeleted == true))) // Vérifie que l'Update est sur l'utilisateur correct avec IsDeleted=true
                            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Erreur de base de données lors de la mise à jour IsDeleted." }));

            // Act
            var result = await controller.Delete(userIdToDelete);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult), "Le résultat devrait être une redirection.");
            var redirectToActionResult = (RedirectToActionResult)result;

            Assert.AreEqual(nameof(StudentsController.Student), redirectToActionResult.ActionName, "La redirection devrait être vers l'action 'Student'.");
            Assert.IsNotNull(controller.TempData["ErreurMessage"], "TempData devrait contenir un message d'erreur.");
            StringAssert.Contains(controller.TempData["ErreurMessage"].ToString(), "Erreur lors du marquage du stagiaire comme supprimé", "Le message d'erreur doit indiquer un échec de la suppression logique.");

            // Vérifier que FindByIdAsync a été appelé
            _mockUserManager.Verify(um => um.FindByIdAsync(userIdToDelete), Times.Once());
            // Vérifier que UpdateAsync a été appelé
            _mockUserManager.Verify(um => um.UpdateAsync(It.IsAny<User>()), Times.Once());
            // Vérifier que l'état IsDeleted de l'utilisateur n'a pas été réellement modifié (dans le mock)
          
        }
      
        [TestMethod]
        public async Task Delete_Post_ValidId_SuccessAndRedirects()
        {
            // Arrange
            var controller = new StudentsController(_mockUserManager.Object, _mockRoleManager.Object, _mockContext, _mockEnvironment.Object);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var userIdToDelete = Guid.NewGuid().ToString();
            // Créons un Student qui hérite de User et a une propriété IsDeleted
            var existingUser = new Student { Id = userIdToDelete, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", IsDeleted = false };
            var classId = 1;

            // Ajouter une entrée Attend pour cet utilisateur pour vérifier la suppression
            _mockContext.Attends.Add(new Attend { StudentId = userIdToDelete, ClasseId = classId });
            _mockContext.SaveChanges(); // Sauvegarder pour que l'entrée existe dans la DB en mémoire

            // Simuler que FindByIdAsync trouve l'utilisateur
            _mockUserManager.Setup(um => um.FindByIdAsync(userIdToDelete))
                            .ReturnsAsync(existingUser);

            // Simuler que UpdateAsync réussit (car c'est UpdateAsync qui est appelé pour la soft delete)
            // On peut même vérifier que l'objet passé à UpdateAsync a bien IsDeleted=true
            _mockUserManager.Setup(um => um.UpdateAsync(It.Is<User>(u => u.Id == userIdToDelete && u.IsDeleted == true)))
                            .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await controller.Delete(userIdToDelete);

            // Assert
            // 1. Vérifier que le résultat est une redirection
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult), "Le résultat devrait être une redirection.");
            var redirectToActionResult = (RedirectToActionResult)result;

            // 2. Vérifier que la redirection est vers l'action "Student"
            Assert.AreEqual(nameof(StudentsController.Student), redirectToActionResult.ActionName, "La redirection devrait être vers l'action 'Student'.");

            // 3. Vérifier que TempData contient le message de succès
            Assert.IsNotNull(controller.TempData["SuccesMessage"], "TempData devrait contenir un message de succès.");
            StringAssert.Contains(controller.TempData["SuccesMessage"].ToString(), "a été marqué comme supprimé.", "Le message de succès doit confirmer la suppression logique.");

            // 4. Vérifier que UserManager.FindByIdAsync a été appelé
            _mockUserManager.Verify(um => um.FindByIdAsync(userIdToDelete), Times.Once());
            // 5. Vérifier que UserManager.UpdateAsync a été appelé (pour la soft delete)
            _mockUserManager.Verify(um => um.UpdateAsync(It.IsAny<User>()), Times.Once());

            // 6. Vérifier que la propriété IsDeleted de l'objet utilisateur mocké a été mise à jour
            Assert.IsTrue(existingUser.IsDeleted, "L'utilisateur devrait être marqué comme supprimé (soft delete).");

            // 7. Vérifier que les entrées Attends n'ont PAS été supprimées
            // Puisque c'est une soft delete de l'utilisateur, les entrées dans Attends devraient rester
            Assert.IsTrue(_mockContext.Attends.Any(a => a.StudentId == userIdToDelete), "Les liaisons de classe ne devraient PAS avoir été supprimées en cas de soft delete.");
        }
    }
}