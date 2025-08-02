using EquityPositionBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace EquityPositionBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Position> Positions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Position>()
                .HasIndex(p => p.SecurityCode)
                .IsUnique();
        }
    }
}
