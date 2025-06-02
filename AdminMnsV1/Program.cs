using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AdminMnsV1.Data;
using AdminMnsV1.Models; // Spécifique pour le modèle User
using AdminMnsV1.Models.CandidaturesModels; // Spécifique pour Candidature et CandidatureStatus
using AdminMnsV1.Models.Documents; // Spécifique pour Document et DocumentType
using AdminMnsV1.Models.Classes; // Spécifique pour SchoolClass
using AdminMnsV1.Models.ViewModels; // Spécifique pour les ViewModels si tu en as
using AdminMnsV1.Models.Experts; // Si tu as un modèle Expert
using AdminMnsV1.Models.Students; // Si tu as un modèle Student

// Usings pour les interfaces et implémentations de Repositories et Services (les chemins harmonisés)
using AdminMnsV1.Data.Repositories.Interfaces;
using AdminMnsV1.Data.Repositories.Implementation;
using AdminMnsV1.Application.Services.Interfaces;// Pour IEmailService
using AdminMnsV1.Application.Services.Implementation;// Pour EmailService
using AdminMnsV1.Repositories.Interfaces;
using AdminMnsV1.Services.Interfaces;
using AdminMnsV1.Services.Implementation;
using AdminMnsV1.Repositories.Implementation;
using AdminMnsV1.Data.Repositories;
using AdminMnsV1.Settings;
using Microsoft.AspNetCore.Identity.UI.Services;
using AdminMnsV1.Infrastructure;
using AdminMnsV1.Interfaces.IServices; // Pour SmtpSettings


var builder = WebApplication.CreateBuilder(args);



//Enregistrement des paramètres SMTP de appsettings.json
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

// Enregistrement du service d'E-mail pour l'injection de dépendances
builder.Services.AddScoped<IEmailService, EmailService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

// Enregistrement du DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuration d'ASP.NET Core Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configurez le cookie d'application Identity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Home/Login";
    options.LogoutPath = "/Home/Logout";
    options.AccessDeniedPath = "/Home/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(180);
    options.SlidingExpiration = true;
});


// ***********************************************************************************
// ENREGISTREMENT DES REPOSITORIES ET SERVICES (AVEC LA LIGNE CRUCIALE DU GÉNÉRIQUE)
// ***********************************************************************************

// ENREGISTREMENT CRUCIAL DU REPOSITORY GÉNÉRIQUE
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Enregistrement des Repositories spécifiques
// Assure-toi que TOUS les repositories spécifiques que tu as créés sont enregistrés ici
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICandidatureRepository, CandidatureRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();


// Si tu as d'autres repositories spécifiques (ex: ICandidatureStatusRepository), ajoute-les ici.
// Sinon, IGenericRepository<CandidatureStatus> sera résolu par la ligne AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Enregistrement des Services
// Assure-toi que TOUS les services que tu as créés sont enregistrés ici
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<IDashboardService, DashboardService>(); // Si ce service existe
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IDocumentTypeService, DocumentTypeService>();
builder.Services.AddScoped<IOnboardingService, OnboardingService>();

builder.Services.AddScoped<
    AdminMnsV1.Application.Services.Interfaces.ICandidatureService,
    AdminMnsV1.Application.Services.Implementation.CandidatureService
>();

//Oublie du password
builder.Services.AddScoped<IEmailSender, EmailSender>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Middleware d'authentification et d'autorisation DANS LE BON ORDRE
app.UseAuthentication();
app.UseAuthorization();

// Création des rôles et de l'utilisateur admin au démarrage
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

    string adminEmail = "admin@mns.com";
    string adminPassword = "VotreMotDePasseAdmin123!"; // CHANGE CE MOT DE PASSE EN PRODUCTION !
    string adminRole = "Admin";

    if (await userManager.FindByNameAsync(adminEmail) == null)
    {
        var adminUser = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            Status = "Null" // Assure-toi que cette propriété existe dans ton modèle User
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
            Console.WriteLine($"Utilisateur '{adminEmail}' créé et assigné au rôle '{adminRole}'.");
        }
        else
        {
            Console.WriteLine($"Erreur lors de la création de l'utilisateur admin :");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"- {error.Description}");
            }
        }
    }
    else
    {
        Console.WriteLine($"L'utilisateur '{adminEmail}' existe déjà dans la base de données.");
    }
}

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
