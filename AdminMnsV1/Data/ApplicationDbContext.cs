using Microsoft.EntityFrameworkCore;
using AdminMnsV1.Models;
using System.Security.Claims;
using AdminMnsV1.Models.Students;
using AdminMnsV1.Models.Experts;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AdminMnsV1.Data
{
    public class ApplicationDbContext : IdentityDbContext<User> // Utilisation de IdentityDbContext pour la gestion des utilisateurs et des rôles
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //public DbSet<Class> Classs { get; set; } // Représente la table "Class"


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(u => u.Address)
                .IsRequired(false);

            modelBuilder.Entity<User>()
               .Property(u => u.City)
               .IsRequired(false);

            modelBuilder.Entity<User>()
               .Property(u => u.Phone)
               .IsRequired(false);

            modelBuilder.Entity<User>()
               .Property(u => u.Status)
               .IsRequired(false);
        }
    }
}
