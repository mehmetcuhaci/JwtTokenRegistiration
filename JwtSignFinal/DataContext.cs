using JwtSignFinal.Models;
using Microsoft.EntityFrameworkCore;

namespace JwtSignFinal
{
    public class DataContext : DbContext
    {
        public DbSet<ApiUsers> Users { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApiUsers>().ToTable("Users");
        }
    }
}
