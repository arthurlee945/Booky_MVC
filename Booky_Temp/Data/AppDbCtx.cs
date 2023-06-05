using Booky_Temp.Model;
using Microsoft.EntityFrameworkCore;

namespace Booky_Temp.Data
{
	public class AppDbCtx:DbContext
	{
		public AppDbCtx(DbContextOptions<AppDbCtx> opts):base(opts) {}
		public DbSet<Category> Categories { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Category>().HasData(
				new Category() { Id = 1, Name = "Western", DisplayOrder=1 },
				new Category() { Id = 2, Name = "Thriller", DisplayOrder=2 }
			);
		}
	}
}
