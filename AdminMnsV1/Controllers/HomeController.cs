using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AdminMnsV1.Models; // Assurez-vous que c'est le bon namespace pour votre User et LoginViewModel
using Microsoft.AspNetCore.Identity; // Nécessaire pour SignInManager et UserManager
using System.Threading.Tasks; // Pour les méthodes asynchrones

namespace AdminMnsV1.Controllers;

public class HomeController : Controller
{
    private readonly SignInManager<User> _signInManager; // Service pour gérer les connexions
    private readonly UserManager<User> _userManager;     // Service pour gérer les utilisateurs et les rôles
    private readonly ILogger<HomeController> _logger;

    // Le constructeur est modifié pour injecter les services d'Identity
    public HomeController(
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        ILogger<HomeController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    // Cette action affichera votre page de connexion (pour les requêtes GET)
    // Elle remplace l'ancienne action Index() qui affichait la page de bienvenue
    public IActionResult Login() // Renommé de Index() à Login()
    {
        return View(); // Va chercher Views/Home/Login.cshtml
    }

    // Cette action gérera la soumission du formulaire de connexion (pour les requêtes POST)
    [HttpPost] // Indique que cette action répond aux requêtes POST
    [ValidateAntiForgeryToken] // Protection de sécurité essentielle contre les attaques CSRF
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null) // Renommé de Index() à Login()
    {
        // Conserve le returnUrl au cas où l'utilisateur ait été redirigé vers la page de connexion
        ViewData["ReturnUrl"] = returnUrl;

        // Vérifie si les données du modèle (email, mot de passe) sont valides
        if (ModelState.IsValid)
        {
            // Tente de connecter l'utilisateur.
            // 'lockoutOnFailure: false' signifie que le compte ne sera pas bloqué après un certain nombre d'échecs immédiats.
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("Utilisateur {Email} connecté avec succès.", model.Email);

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    // Redirige l'utilisateur en fonction de son rôle après une connexion réussie
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Dashboard", "Admin"); // Exemple : redirige vers l'action Dashboard d'un contrôleur Admin
                    }
                    else if (await _userManager.IsInRoleAsync(user, "Stagiaire"))
                    {
                        return RedirectToAction("Dashboard", "Stagiaire"); // Exemple : redirige vers l'action Dashboard d'un contrôleur Stagiaire
                    }
                    else
                    {
                        // Redirection par défaut si l'utilisateur n'a pas de rôle spécifique, ou revient à la page précédente
                        return LocalRedirect(returnUrl ?? "/");
                    }
                }
                return LocalRedirect(returnUrl ?? "/"); // Au cas où l'utilisateur soit connecté mais non trouvé (rare)
            }
            if (result.RequiresTwoFactor)
            {
                // Si la double authentification est activée pour l'utilisateur
                return RedirectToAction("LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
            }
            if (result.IsLockedOut)
            {
                // Si le compte de l'utilisateur est bloqué suite à trop de tentatives échouées
                _logger.LogWarning("Compte utilisateur {Email} bloqué.", model.Email);
                ModelState.AddModelError(string.Empty, "Votre compte est bloqué en raison de trop de tentatives de connexion échouées.");
                return View(model); // Affiche la vue avec l'erreur
            }
            else
            {
                // Si la connexion a échoué pour d'autres raisons (mauvais email/mot de passe)
                ModelState.AddModelError(string.Empty, "Tentative de connexion invalide. Veuillez vérifier votre email et mot de passe.");
                return View(model); // Affiche la vue avec l'erreur
            }
        }

        // Si le modèle n'est pas valide (ex: email manquant), réafficher la vue avec les messages d'erreur
        return View(model);
    }

    // Conservez vos autres actions MVC si vous les utilisez (Privacy, Error)
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}