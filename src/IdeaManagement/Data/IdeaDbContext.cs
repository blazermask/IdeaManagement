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
        modelBuilder.Entity<Idea>(entity =>
        {
            entity.ToTable("Ideas");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Content)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(e => e.CreatedDate)
                .IsRequired()
                .HasColumnType("datetime");

            entity.Property(e => e.ModifiedDate)
                .IsRequired()
                .HasColumnType("datetime");
        });
    }
}
