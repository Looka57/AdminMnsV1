using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AdminMnsV1.Data;
using AdminMnsV1.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using AdminMnsV1.Repositories.Interfaces;
using AdminMnsV1.Repositories;
using AdminMnsV1.Services.Interfaces;
using AdminMnsV1.Services;



var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Nécessaire pour les pages Identity (même si non scaffoldées)
// Ces lignes enregistrent les services nécessaires pour que MVC (Modèle-Vue-Contrôleur) et les pages Razor (utilisées par Identity) fonctionnent dans l' application.

// Enregistrement du DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//C'est la configuration de Entity Framework Core. Il utiliser SQL Server et se connecte à la base de données via la chaîne de connexion "DefaultConnection" (définie dans appsettings.json).
//****ApplicationDbContext est votre pont vers la base de données.

// Enregistrement des Repositories
builder.Services.AddScoped<IStudentRepository, StudentRepository>(); // Interface vers Implémentation
builder.Services.AddScoped<IClassRepository, ClassRepository>();     // Interface vers Implémentation

// Enregistrement des Services
builder.Services.AddScoped<IStudentService, StudentService>();       // Interface vers Implémentation

//C'est la configuration clé d'ASP.NET Core Identity.
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Options de connexion
    options.SignIn.RequireConfirmedAccount = false; // <<< MIS À FALSE TEMPORAIREMENT POUR LE DÉBOGAGE
                                                    // Cela désactive la vérification de confirmation d'email

    // Options de mot de passe
    //Ce sont les règles de complexité pour les mots de passe des utilisateurs.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;

    // Options de verrouillage du compte
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>() //C'est la configuration clé d'ASP.NET Core Identity.
                                                      //Identity stocke toutes ses données (utilisateurs, mots de passe hachés, rôles, revendications) dans ApplicationDbContext, et donc dans la base de données SQL Server.
    .AddDefaultTokenProviders(); // Nécessaire pour la génération de tokens (réinitialisation mdp, confirmation email)


// Configurez le cookie d'application Identity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Home/Login";          // Votre page de connexion
    options.LogoutPath = "/Home/Logout";        // Votre page de déconnexion
    options.AccessDeniedPath = "/Home/AccessDenied"; // Votre page d'accès refusé erreur 404
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Durée de vie du cookie
    options.SlidingExpiration = true;          // Renouvelle le cookie à chaque requête s'il est à mi-vie
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); //Permet à application de servir des fichiers statiques comme les fichiers CSS (~/css/) et                  JavaScript (~/js/) qui sont listés dans le _Layout.cshtml.

app.UseRouting(); //Permet au middleware de routage d'identifier la bonne "endpoint" (le contrôleur et l'action) pour   une requête entrante.

//*****C'est l'ordre le plus important pour Identity.******
// Middleware d'authentification et d'autorisation DANS LE BON ORDRE

app.UseAuthentication(); // Gère l'authentification des utilisateurs (à partir du cookie d'authentification, par        exemple).
app.UseAuthorization();  // Gère l'autorisation des utilisateurs Vérifie si l'utilisateur identifié a la permission d'accéder à la ressource demandée. L'authentification doit toujours précéder l'autorisation.



//************* création des rôles au démarrage de l'application ****************

//est un excellent moyen de s'assurer que les rôles ("Admin", "Expert", "Student") existent dans la base de données au démarrage de l'application. C'est essentiel pour que User.IsInRole() fonctionne. 
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    string[] roleNames = { "Admin", "Expert", "Student" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    //     // ---LE CODE SUIVANT  POUR CRÉER L'UTILISATEUR ADMIN PAR DÉFAUT ---
    //    string adminEmail = "admin@mns.com"; // L'email de votre administrateur
    //    string adminPassword = "VotreMotDePasseAdmin123!"; // <--- TRÈS IMPORTANT : CHANGEZ CE MOT DE PASSE POUR LA PRODUCTION !!!
    //    string adminRole = "Admin";

    //    // Vérifie si l'utilisateur admin existe déjà
    //    if (await userManager.FindByNameAsync(adminEmail) == null)
    //    {
    //        var adminUser = new User // Utilisez votre modèle d'utilisateur 'User'
    //        {
    //            UserName = adminEmail,
    //            Email = adminEmail,
    //            EmailConfirmed = true, // Indique que l'email est déjà confirmé
    //            Status = "Null"
    //        };

    //        // Tente de créer l'utilisateur
    //        var result = await userManager.CreateAsync(adminUser, adminPassword);

    //        if (result.Succeeded)
    //        {
    //            // Si la création est réussie, assigne l'utilisateur au rôle Admin
    //            await userManager.AddToRoleAsync(adminUser, adminRole);
    //            Console.WriteLine($"Utilisateur '{adminEmail}' créé et assigné au rôle '{adminRole}'.");
    //        }
    //        else
    //        {
    //            // Affiche les erreurs si la création a échoué
    //            Console.WriteLine($"Erreur lors de la création de l'utilisateur admin :");
    //            foreach (var error in result.Errors)
    //            {
    //                Console.WriteLine($"- {error.Description}");
    //            }
    //        }
    //    }
    //    else
    //    {
    //        Console.WriteLine($"L'utilisateur '{adminEmail}' existe déjà dans la base de données.");
    //    }
    //}

    app.MapRazorPages(); // Permet aux pages Razor de fonctionner

    // Définit la route par défaut de votre application, qui dirigera les requêtes vers Home/Login si aucun contrôleur/action n'est spécifié.
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Login}/{id?}");

    app.Run();
}