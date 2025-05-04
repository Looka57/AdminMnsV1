using Microsoft.EntityFrameworkCore;
using AdminMnsV1.Models;
using System.Security.Claims;

namespace AdminMnsV1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Class> Classs { get; set; } // Représente la table "Class"
        public DbSet<User> Users { get; set; } // Représente la table "User"
        public DbSet<Expert> Experts { get; set; } // Représente la table "expert"
        public DbSet<Student> Students { get; set; } // Représente la table "Studnts"
        public DbSet<Administrator> Administrators { get; set; } // Représente la table "Administrator"

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("Discriminator") //Indique à EF Core que nous utilisons une colonne nommée "Discriminator" pour distinguer les types dans la hiérarchie User.
                .HasValue<User>("User")
                .HasValue<Expert>("Expert_")
                .HasValue<Student>("Student")
                .HasValue<Administrator>("Administrator");
            //Définit les valeurs que prendra la colonne "Discriminator" pour chaque type concret dans la hiérarchie.

                // Indique explicitement que la propriété Discriminator n'est pas requise au niveau du modèle
                modelBuilder.Entity<User>()
                    .Property(u => u.Discriminator)
                    .IsRequired(false);

            //    // Indique explicitement que la propriété PasswordHash n'est pas requise au niveau du modèle
            //    modelBuilder.Entity<User>()
            //        .Property(u => u.PasswordHash)
            //        .IsRequired(false);
            //}
        }
    }
}
