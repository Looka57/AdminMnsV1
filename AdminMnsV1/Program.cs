using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AdminMnsV1.Data;
using AdminMnsV1.Models;
using Microsoft.AspNetCore.Authentication.Cookies; // Assurez-vous que c'est bien présent

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Nécessaire pour les pages Identity (même si non scaffoldées)

// Enregistrement de votre DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- NOUVELLE CONFIGURATION SIMPLIFIÉE POUR IDENTITY ET LES COOKIES ---
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Options de connexion
    options.SignIn.RequireConfirmedAccount = false; // <<< MIS À FALSE TEMPORAIREMENT POUR LE DÉBOGAGE
                                                    // Cela désactive la vérification de confirmation d'email
                                                    // Ce n'est pas lié à votre problème de base de données mais peut causer des blocages

    // Options de mot de passe
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
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders(); // Nécessaire pour la génération de tokens (réinitialisation mdp, confirmation email)

// Configurez le cookie d'application Identity ici, CELA REMPLACE L'ANCIEN BLOC AddCookie()
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Home/Login";          // Votre page de connexion
    options.LogoutPath = "/Home/Logout";        // Votre page de déconnexion
    options.AccessDeniedPath = "/Home/AccessDenied"; // Votre page d'accès refusé
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Durée de vie du cookie
    options.SlidingExpiration = true;          // Renouvelle le cookie à chaque requête s'il est à mi-vie
});
// --- FIN NOUVELLE CONFIGURATION ---


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Généralement placé ici pour servir les fichiers statiques

app.UseRouting();

// Middleware d'authentification et d'autorisation DANS LE BON ORDRE
app.UseAuthentication(); // Gère l'authentification des utilisateurs
app.UseAuthorization();  // Gère l'autorisation des utilisateurs

// création des rôles au démarrage de l'application
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    // Assurez-vous que le rôle "Student" est bien créé
    string[] roleNames = { "Admin", "Expert", "Student" }; // <<< Le rôle "Student" est ici
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

app.MapRazorPages(); // Permet aux pages Razor de fonctionner
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();