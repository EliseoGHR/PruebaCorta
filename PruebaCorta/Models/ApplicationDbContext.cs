using Microsoft.EntityFrameworkCore;

namespace PruebaCorta.Models
{
    public class ApplicationDbContext  : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        DbSet<Usuario> Usuarios { get; set; }
    }
}
