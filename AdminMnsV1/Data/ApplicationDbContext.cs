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
        public DbSet<Student>Students { get; set; }  // Représente la table "Student"


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


            // Configuration de la clé primaire composite pour Attend
            modelBuilder.Entity<Attend>()
                .HasKey(a => new { a.StudentId, a.ClasseId });

            // Configuration des relations entre Attend, Student et Class
            modelBuilder.Entity<Attend>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Attends)
                .HasForeignKey(a => a.StudentId);

            modelBuilder.Entity<Attend>()
                .HasOne(a => a.Class)
                .WithMany(c => c.Attends)
                .HasForeignKey(a => a.ClasseId);

            modelBuilder.Entity<Class>()
                .HasMany(c => c.Students)
                .WithMany(s => s.Classes)
               .UsingEntity<Attend>(
                    j => j
                        .HasOne(pt => pt.Student)
                        .WithMany(t => t.Attends)
                        .HasForeignKey(pt => pt.StudentId),
                    j => j
                        .HasOne(pt => pt.Class)
                        .WithMany(p => p.Attends)
                        .HasForeignKey(pt => pt.ClasseId),
                    j => {
                        j.HasKey(t => new { t.StudentId, t.ClasseId });
                    });

        }
    }
}
