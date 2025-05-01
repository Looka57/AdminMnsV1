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
    }
}
