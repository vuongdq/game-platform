using Microsoft.EntityFrameworkCore;
using GamePlatform.Models;

namespace GamePlatform.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();

        modelBuilder.Entity<Game>()
            .HasIndex(g => g.Name)
            .IsUnique();

        modelBuilder.Entity<Game>()
            .HasOne(g => g.Category)
            .WithMany()
            .HasForeignKey(g => g.CategoryId);

        modelBuilder.Entity<Game>()
            .Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<Game>()
            .Property(g => g.Description)
            .IsRequired()
            .HasMaxLength(500);

        modelBuilder.Entity<Game>()
            .Property(g => g.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        modelBuilder.Entity<Game>()
            .Property(g => g.GameUrl)
            .IsRequired()
            .HasMaxLength(500);

        modelBuilder.Entity<Category>()
            .Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<Category>()
            .Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(500);

        modelBuilder.Entity<Category>()
            .Property(c => c.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);
    }
} 