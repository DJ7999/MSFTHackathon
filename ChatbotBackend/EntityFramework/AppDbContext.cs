using ChatbotBackend.Models.DbModel;
using Microsoft.EntityFrameworkCore;

namespace ChatbotBackend.EntityFramework
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<Retirement> Retirements { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User table configuration
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Name)
                .IsUnique();

            // Portfolio configuration
            modelBuilder.Entity<Portfolio>()
                .HasKey(p => new { p.UserId, p.AssetName }); // Composite primary key

            modelBuilder.Entity<Portfolio>()
                .HasOne(p => p.User)
                .WithMany(u => u.Portfolios)
                .HasForeignKey(p => p.UserId);

            // Goal configuration
            modelBuilder.Entity<Goal>()
                .HasKey(g => new { g.UserId, g.Name }); // Composite primary key

            modelBuilder.Entity<Goal>()
                .HasOne(g => g.User)
                .WithMany(u => u.Goals)
                .HasForeignKey(g => g.UserId);

            // Retirement configuration (1:1)
            modelBuilder.Entity<Retirement>()
                .HasKey(r => r.UserId); // Optional: make UserId the PK if it's 1:1

            modelBuilder.Entity<Retirement>()
                .HasOne(r => r.User)
                .WithOne(u => u.Retirement)
                .HasForeignKey<Retirement>(r => r.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
