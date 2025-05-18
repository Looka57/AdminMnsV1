using Microsoft.AspNetCore.Mvc;
using AdminMnsV1.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims; // Pour les Claims
using Microsoft.AspNetCore.Identity; // NOUVEAU: Ajoutez ce using pour UserManager et SignInManager
using System.Collections.Generic;
using AdminMnsV1.Models.Students; // NOUVEAU: Pour List<Claim>



namespace AdminMnsV1.Controllers
{
    public class HomeController : Controller
    {
        //Injectez UserManager et SignInManager 
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public HomeController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: /Home/Login (Affiche le formulaire de connexion)
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }


        // Méthode appelée par le formulaire "Stagiaire"
        [HttpPost]
        [ValidateAntiForgeryToken] // Toujours utiliser pour les POST
        public async Task<IActionResult> LoginStudent(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", model); // Retourne à la vue Login avec les erreurs
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                //la connexion a reussi
                var user = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user); // ET ICI AUSSI


                //verifier que l'utilisateur a le role student
                if (roles.Contains("Student"))
                {
                    // Redirige vers la page spécifique du tableau de bord stagiaire
                    return RedirectToAction("DashboardStudent", "Dashboard");
                }

                else
                {
                    // Si l'utilisateur s'est connecté via le formulaire "Stagiaire" mais n'est PAS un "Student",
                    // vous pourriez vouloir le déconnecter et afficher une erreur,
                    // ou le rediriger vers une page générique/admin si son rôle le permet.
                    await _signInManager.SignOutAsync(); // Déconnecte l'utilisateur si son rôle ne correspond pas au formulaire
                    ModelState.AddModelError(string.Empty, "Vos informations de connexion sont valides, mais votre rôle ne vous permet pas d'accéder via ce formulaire.");
                    return View("Login", model);
                }


            }

            // Gérer les différents types d'échecs de connexion
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Compte verrouillé en raison de trop de tentatives de connexion échouées.");
                return View("Login", model);
            }
            else if (result.IsNotAllowed)
            {
                // L'utilisateur n'est pas autorisé à se connecter (par ex. email non confirmé si requis)
                ModelState.AddModelError(string.Empty, "Connexion non autorisée. Votre compte pourrait nécessiter une confirmation ou une activation.");
                return View("Login", model);
            }
            else
            {
                // Échec général (email ou mot de passe incorrect)
                ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
                return View("Login", model);
            }
        }



        // POST: /Home/LoginAdmin (Traite la soumission du formulaire Admin)
        [HttpPost]
        [ValidateAntiForgeryToken] // Toujours utiliser pour les POST
        public async Task<IActionResult> LoginAdmin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", model); // Retourne à la vue Login avec les erreurs
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Admin"))
                {
                    // Redirige vers la page spécifique du tableau de bord admin
                    return RedirectToAction("Dashboard", "Dashboard");
                }

                else
                {
                    // Si l'utilisateur s'est connecté via le formulaire "Admin" mais n'est PAS un "Admin",
                    // vous pourriez vouloir le déconnecter et afficher une erreur.
                    await _signInManager.SignOutAsync();
                    ModelState.AddModelError(string.Empty, "Vos informations de connexion sont valides, mais votre rôle ne vous permet pas d'accéder via ce formulaire.");
                    return View("Login", model);
                }
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Compte verrouillé.");
                return View("Login", model);
            }
            else if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "Connexion non autorisée.");
                return View("Login", model);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
                return View("Login", model);
            }
        }


        //DECONNEXION
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); // Utilisez SignInManager pour la déconnexion
            return RedirectToAction("Login", "Home"); // Redirige vers la page de connexion
        }
    }
}