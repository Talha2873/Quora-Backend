using Microsoft.EntityFrameworkCore;
using QuoraBackend.Models;

namespace QuoraBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Follow> Follows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Prevent duplicate votes
            modelBuilder.Entity<Vote>()
                .HasIndex(v => new { v.UserId, v.QuestionId })
                .IsUnique();
        }
    }
}