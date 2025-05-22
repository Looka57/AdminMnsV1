using Microsoft.EntityFrameworkCore;
using AdminMnsV1.Models;
using System.Security.Claims;
using AdminMnsV1.Models.Students;
using AdminMnsV1.Models.Experts;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using AdminMnsV1.Models.Classes;
using AdminMnsV1.Models.Candidature;
using System.Reflection.Metadata;
using AdminMnsV1.Models.DocumentTypes;
using AdminMnsV1.Models.Documents;
using Microsoft.CodeAnalysis.Diagnostics;


namespace AdminMnsV1.Data
{
    public class ApplicationDbContext : IdentityDbContext<User> // Utilisation de IdentityDbContext pour la gestion des utilisateurs et des rôles
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SchoolClass> SchoolClass { get; set; } // Représente la table "Class"
        public DbSet<Attend> Attends { get; set; } // Représente la table intermediaire "Attends"
        public DbSet<Student> Students { get; set; }   // Représente la table "Student"
        public DbSet<Candidature> Candidatures { get; set; } // Représente la table "Candidature"
        public DbSet<CandidatureStatus> CandidatureStatuses { get; set; } // Represente la table CandidatureStatus

        public DbSet<Documents> Documents { get; set; } // Représente la table "Documents"
        public DbSet<DocumentType> DocumentTypes { get; set; } // Représente la table "DocumentType"




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


        //    modelBuilder.Entity<User>()
        //.HasDiscriminator<string>("Discriminator")
        //.HasValue<User>("User")
        //.HasValue<Administrator>("Administrator");


            modelBuilder.Entity<User>()
                .Property(u => u.Address)
                .IsRequired(false);

            modelBuilder.Entity<User>()
               .Property(u => u.City)
               .IsRequired(false);



            //-------------- Configuration de la clé primaire composite pour Attend----------------------
            modelBuilder.Entity<Attend>()
                 .HasKey(a => new { a.StudentId, a.ClasseId }); //// La clé primaire composite est StudentId + ClasseId


            // ***********Configuration des relations entre ATTEND, STUDENT et CLASS******************

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



            // ****************Configuration de la relations entre CANDIDATURE - USER - CLASS *********************

            // Configuration de la relation Candidature - User
            modelBuilder.Entity<Candidature>()
                .HasOne(c => c.User) // Une candidature a un utilisateur
                .WithMany(u => u.Candidatures)// Un utilisateur peut avoir plusieurs candidatures
                .HasForeignKey(c => c.UserId) // La clé étrangère est UserId
                .OnDelete(DeleteBehavior.Cascade); // Si l'utilisateur est supprimé, toutes ses candidatures le seront aussi

            // Configuration de la relation Candidature - SchoolClass
            modelBuilder.Entity<Candidature>()
                 .HasOne(c => c.Class)
                 .WithMany()
                 .HasForeignKey(c => c.ClassId)
                 .IsRequired(false) // La classe est optionnelle
                 .OnDelete(DeleteBehavior.NoAction); // Si la classe est supprimée, lsi tu veux empêcher la suppression de la classe tant qu'elle a des candidatures)

            // Configuration de la relation Candidature - CandidatureStatus
            modelBuilder.Entity<Candidature>()
               .HasOne(c => c.CandidatureStatus)
                .WithMany(cs => cs.Candidatures)
                .HasForeignKey(c => c.CandidatureStatutId)
                .OnDelete(DeleteBehavior.Restrict); // Si le statut de candidature est supprimé, toutes les candidatures associées le seront aussi



            // ****************Configuration de la relations entre DOCUMENT - USER - CANDIDATURE *********************


            // Configuration de la relation Document - DocumentType
            modelBuilder.Entity<Documents>()
                .HasOne(d => d.DocumentType)
                .WithMany(dt => dt.Documents) // DocumentType doit avoir public ICollection<Document> Documents
                .HasForeignKey(d => d.DocumentTypeId)
                .OnDelete(DeleteBehavior.Restrict); // Empêche la suppression d'un type si des documents y sont liés


            // Configuration de la relation Document - User (StudentUser)
            modelBuilder.Entity<Documents>()
                .HasOne(d => d.StudentUser) // Navigation property vers l'User (qui est le Student)
                .WithMany(u => u.Documents) // User doit avoir public ICollection<Document> Documents
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.NoAction); // Si l'User est supprimé, ses documents sont supprimés
            //Cela signifie que si un User(étudiant) est supprimé, la suppression sera bloquée
            // si des documents directs lui sont encore liés.

            // Configuration de la relation Document - User (Administrator)
            modelBuilder.Entity<Documents>()
                .HasOne(d => d.Admin) // Navigation property vers l'User (qui est l'Admin validateur)
                .WithMany() // L'entité User n'a pas forcément une collection "ValidatedDocuments" si tu ne l'as pas ajoutée. C'est OK.
                .HasForeignKey(d => d.AdminId)
                .IsRequired(false) // Permet à AdministratorId d'être NULL
                .OnDelete(DeleteBehavior.NoAction); // Si l'admin est supprimé  si tu tentes de supprimer un utilisateur qui est référencé comme AdminId dans un document,
                                                    // la suppression de cet utilisateur sera BLOQUÉE par la base de données.
                                                    // Tu devras d'abord mettre manuellement AdminId à NULL dans les documents concernés,
                                                    // ou supprimer les documents, avant de pouvoir supprimer l'administrateur.

            // Configuration Relation Candidature - User (UserId)
            modelBuilder.Entity<Candidature>()
                .HasOne(c => c.User)
                .WithMany(u => u.Candidatures) // Assure-toi que User a bien une ICollection<Candidature>
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade); // C'est souvent souhaitable ici : suppression user -> suppression candidatures

            // Configuration Relation Documents - Candidature (CandidatureId) - La nouvelle FK
            modelBuilder.Entity<Documents>()
                .HasOne(d => d.Candidature)
                .WithMany(c => c.Documents) // Assure-toi que Candidature a bien une ICollection<Documents>
                .HasForeignKey(d => d.CandidatureId)
                .IsRequired(false) // Si un document peut exister sans candidature (si tu as mis CandidatureId en int?)
                .OnDelete(DeleteBehavior.NoAction); // <-- METS CELA À NOACTION (ou Restrict)
                                                    // C'est crucial ici pour briser le cycle.
                                                    // Si tu veux supprimer les documents avec la candidature, tu devras le gérer manuellement
                                                    // dans ton code applicatif (avant de supprimer la candidature, tu supprimes ses documents liés).




        }
    }
}
