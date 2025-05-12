using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AdminMnsV1.Data; 
using AdminMnsV1.Models; 


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Pour que votre DbContext soit disponible dans votre application via l'injection de dépendances, vous devez l'enregistrer dans le conteneur de services.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));//Ce code récupère la chaîne de connexion nommée "DefaultConnection" de votre fichier appsettings.json et configure EF Core pour utiliser le fournisseur SQL Server.


// Ajout d'Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Configuration des options d'Identity (facultatif)
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
    .AddDefaultTokenProviders();

// Configuration de l'authentification et de l'autorisation
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();


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

    //// Créer l'utilisateur Admin par défaut
    //string adminUserEmail = "admin@example.com"; // Remplacez par l'e-mail de votre administrateur
    //string adminPassword = "Admin123!"; // Remplacez par un mot de passe sécurisé
    //var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
    //if (adminUser == null)
    //{
    //    var newAdminUser = new User
    //    {
    //        UserName = adminUserEmail,
    //        Email = adminUserEmail,
    //        // Autres propriétés de l'administrateur
    //    };
    //    var result = await userManager.CreateAsync(newAdminUser, adminPassword);
    //    if (result.Succeeded)
    //    {
    //        await userManager.AddToRoleAsync(newAdminUser, "Admin");
    //    }
    //    else
    //    {
    //        // Gérer l'erreur (par exemple, journaliser)
    //        Console.WriteLine("Erreur lors de la création de l'utilisateur Admin par défaut :");
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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
