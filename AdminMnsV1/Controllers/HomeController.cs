// Controllers/HomeController.cs (Aucun changement majeur ici, il gère l'authentification)
using Microsoft.AspNetCore.Mvc;
using AdminMnsV1.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity; // Pour UserManager et SignInManager
using System.Linq; // Pour .Contains


//Le HomeController est dédié à l'authentification (connexion, déconnexion).


namespace AdminMnsV1.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public HomeController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        //UserManager et SignInManager sont des services fondamentaux fournis par ASP.NET Core Identity. Il est tout à fait normal de les injecter directement dans les contrôleurs qui gèrent l'authentification, car ils sont au cœur de cette fonctionnalité.

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginStudent(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Student"))
                {
                    return RedirectToAction("DashboardStudent", "Dashboard");
                }
                else
                {
                    await _signInManager.SignOutAsync();
                    ModelState.AddModelError(string.Empty, "Vos informations de connexion sont valides, mais votre rôle ne vous permet pas d'accéder via ce formulaire.");
                    return View("Login", model);
                }
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Compte verrouillé en raison de trop de tentatives de connexion échouées.");
                return View("Login", model);
            }
            else if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "Connexion non autorisée. Votre compte pourrait nécessiter une confirmation ou une activation.");
                return View("Login", model);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
                return View("Login", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAdmin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("Dashboard", "Dashboard");
                }
                else
                {
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

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Home");
        }
    }
}