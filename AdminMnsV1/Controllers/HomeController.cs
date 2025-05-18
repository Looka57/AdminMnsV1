using Microsoft.AspNetCore.Mvc;
using AdminMnsV1.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims; // Pour les Claims
using Microsoft.AspNetCore.Identity; // NOUVEAU: Ajoutez ce using pour UserManager et SignInManager
using System.Collections.Generic; // NOUVEAU: Pour List<Claim>


namespace AdminMnsV1.Controllers
{
    public class HomeController : Controller
    {
        // NOUVEAU: Injectez UserManager et SignInManager (même si pas utilisés pour la vérif hardcodée)
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

        // POST: /Home/LoginStudent (Traite la soumission du formulaire Stagiaire)
        [HttpPost]
        [ValidateAntiForgeryToken] // Toujours utiliser pour les POST
        public async Task<IActionResult> LoginStudent(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // *** LOGIQUE DE VÉRIFICATION HARDCODÉE POUR STAGIAIRE (À REMPLACER PLUS TARD) ***
                if (model.Email == "stagiaire@mns.com" && model.Password == "stagiaire123")
                {
                    // Authentification réussie pour un stagiaire
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Email),
                        new Claim(ClaimTypes.Role, "Student"), // Attribuez le rôle 'Student' (EN ANGLAIS)
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe, // Gère l'option "Se souvenir de moi"
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Exemple de durée de vie du cookie
                    };

                    await HttpContext.SignInAsync(
                        IdentityConstants.ApplicationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // Redirige vers la page spécifique du tableau de bord stagiaire
                    return RedirectToAction("DashboardStudent", "Dashboard");
                }
                else
                {
                    // Message d'erreur si les identifiants hardcodés ne correspondent pas
                    ModelState.AddModelError(string.Empty, "Email ou mot de passe invalide.");
                }
            }
            // Si ModelState.IsValid est false ou si la connexion hardcodée échoue
            return View("Login", model); // Retourne à la vue Login avec les erreurs
        }

        // POST: /Home/LoginAdmin (Traite la soumission du formulaire Admin)
        [HttpPost]
        [ValidateAntiForgeryToken] // Toujours utiliser pour les POST
        public async Task<IActionResult> LoginAdmin(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // *** LOGIQUE DE VÉRIFICATION HARDCODÉE POUR ADMIN (À REMPLACER PLUS TARD) ***
                if (model.Email == "admin@mns.com" && model.Password == "admin123")
                {
                    // Authentification réussie pour un admin
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Email),
                        new Claim(ClaimTypes.Role, "Admin"), // Attribuez le rôle 'Admin'
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                    };

                    await HttpContext.SignInAsync(
                        IdentityConstants.ApplicationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // Redirige vers la page spécifique du tableau de bord admin
                    return RedirectToAction("Dashboard", "Dashboard");
                }
                else
                {
                    // Message d'erreur si les identifiants hardcodés ne correspondent pas
                    
                    ModelState.AddModelError(string.Empty, "Email ou mot de passe invalide.");
                }
            }
            // Si ModelState.IsValid est false ou si la connexion hardcodée échoue
            return View("Login", model); // Retourne à la vue Login avec les erreurs
        }

        // Action de déconnexion utilisant SignInManager (standard avec Identity)
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); // Utilisez SignInManager pour la déconnexion
            return RedirectToAction("Login", "Home"); // Redirige vers la page de connexion
        }
    }
}