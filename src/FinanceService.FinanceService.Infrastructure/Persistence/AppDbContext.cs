using Microsoft.EntityFrameworkCore;
using FinanceService.Domain.Entities;

namespace FinanceService.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Currency> Currencies => Set<Currency>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CharCode).IsRequired().HasMaxLength(10);
            entity.Property(e => e.RateToRub).HasPrecision(18, 4);
            entity.HasIndex(e => e.CharCode).IsUnique();
        });
    }
}