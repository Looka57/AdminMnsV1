using Microsoft.EntityFrameworkCore;
using AdminMnsV1.Models;
using System.Security.Claims;
using AdminMnsV1.Models.Students;
using AdminMnsV1.Models.Experts;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using AdminMnsV1.Models.Classes;

namespace AdminMnsV1.Data
{
    public class ApplicationDbContext : IdentityDbContext<User> // Utilisation de IdentityDbContext pour la gestion des utilisateurs et des rôles
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Class> Classs { get; set; } // Représente la table "Class"
        public DbSet<Attend> Attends { get; set; } // Représente la table intermediaire "Attends"
        public DbSet<Student> Students { get; set; }   // Représente la table "Student"


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(u => u.Address)
                .IsRequired(false);

            modelBuilder.Entity<User>()
               .Property(u => u.City)
               .IsRequired(false);

            //modelBuilder.Entity<User>()
            //    .Property(u => u.Phone)
            //    .IsRequired(false);


            //modelBuilder.Entity<User>()
            //   .Property(u => u.Nationality)
            //   .IsRequired(false);

            //modelBuilder.Entity<User>()
            //    .Property(u => u.FranceTravailNumber)
            //    .IsRequired(false);

            //modelBuilder.Entity<User>()
            //    .Property(u => u.SocialSecurityNumber)
            //    .IsRequired(false);



            // Configuration de la clé primaire composite pour Attend
            modelBuilder.Entity<Attend>()
                 .HasKey(a => new { a.StudentId, a.ClasseId }); //// La clé primaire composite est StudentId + ClasseId


            // ***********Configuration des relations entre Attend, Student et Class******************

            // Configure la relation entre Attend et Student
            modelBuilder.Entity<Attend>()
                .HasOne(a => a.Student) // Une inscription a un étudiant
                .WithMany(s => s.Attends) // Un étudiant a plusieurs inscriptions
                .HasForeignKey(a => a.StudentId); // La clé étrangère est StudentId


            // Configure la relation entre Attend et Class
            modelBuilder.Entity<Attend>()
                .HasOne(a => a.Class) // Une inscription a une classe
                .WithMany(c => c.Attends) // Une classe a plusieurs inscriptions
                .HasForeignKey(a => a.ClasseId); // La clé étrangère est ClasseId




            //modelBuilder.Entity<Class>()
            //    .HasMany(c => c.Students)
            //    .WithMany(s => s.Classes)
            //   .UsingEntity<Attend>(
            //        j => j
            //            .HasOne(pt => pt.Student)
            //            .WithMany(t => t.Attends)
            //            .HasForeignKey(pt => pt.StudentId),
            //        j => j
            //            .HasOne(pt => pt.Class)
            //            .WithMany(p => p.Attends)
            //            .HasForeignKey(pt => pt.ClasseId),
            //        j => {
            //            j.HasKey(t => new { t.StudentId, t.ClasseId });
            //        });

        }
    }
}
