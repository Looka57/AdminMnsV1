using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AdminMnsV1.Data;
using AdminMnsV1.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Enregistrement de votre DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Ajout d'Identity avec votre classe User, les rôles et la configuration des options
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true; // IMPORTANT : Cette ligne est essentielle pour l'UI générée
    // Configuration des options d'Identity
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders(); // Nécessaire pour la génération de tokens (réinitialisation mdp, confirmation email)

var app = builder.Build();

// creation des roles
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

    //// Cr�er l'utilisateur Admin par d�faut (décommenter si vous voulez l'utiliser)
    //string adminUserEmail = "admin@example.com";
    //string adminPassword = "Admin123!";
    //var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
    //if (adminUser == null)
    //{
    //    var newAdminUser = new User
    //    {
    //        UserName = adminUserEmail,
    //        Email = adminUserEmail,
    //    };
    //    var result = await userManager.CreateAsync(newAdminUser, adminPassword);
    //    if (result.Succeeded)
    //    {
    //        await userManager.AddToRoleAsync(newAdminUser, "Admin");
    //    }
    //    else
    //    {
    //        Console.WriteLine("Erreur lors de la création de l'utilisateur Admin par défaut:");
    //        foreach (var error in result.Errors)
    //        {
    //            Console.WriteLine(error.Description);
    //        }
    //    }
    //}
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Généralement placé ici pour servir les fichiers statiques
app.UseRouting();

app.UseAuthentication(); // Gère l'authentification des utilisateurs
app.UseAuthorization();  // Gère l'autorisation des utilisateurs (UNE SEULE FOIS)

app.MapRazorPages(); // Permet aux pages Razor de fonctionner
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");
// Supprimez .WithStaticAssets(); si ce n'est pas une extension standard et que cela pose problème

app.Run();