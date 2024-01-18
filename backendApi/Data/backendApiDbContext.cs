using backendApi.Models;
using Microsoft.EntityFrameworkCore;

namespace backendApi.Data
{
    public class backendApiDbContext : DbContext
    {
        public backendApiDbContext(DbContextOptions DBCO) : base(DBCO)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
    }
}
