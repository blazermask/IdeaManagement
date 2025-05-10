using Microsoft.EntityFrameworkCore;
using IdeaManagement.Models;

namespace IdeaManagement.Data;

public class IdeaDbContext : DbContext
{
    public DbSet<Idea> Ideas { get; set; }

    public IdeaDbContext(DbContextOptions<IdeaDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Idea>()
            .HasKey(i => i.Id);

        modelBuilder.Entity<Idea>()
            .Property(i => i.Content)
            .IsRequired();
    }
}
