using BookyApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BookyApp.Data
{
    public class ApplicationDbContext: DbContext
    {
        //Basic configuration for entity framework
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

        public DbSet<Category> Categories { get; set; }
    }
}
