using APIUsuarios.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace APIUsuarios.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Usuario>();
        entity.HasKey(u => u.Id);
        entity.Property(u => u.Nome)
              .IsRequired()
              .HasMaxLength(100);

        entity.Property(u => u.Email)
              .IsRequired();

        entity.Property(u => u.Senha)
              .IsRequired()
              .HasMaxLength(200);

        entity.Property(u => u.Ativo)
              .HasDefaultValue(true);

        entity.HasIndex(u => u.Email)
              .IsUnique();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Fallback para execuções sem DI configurado (ajuda nas migrations)
            optionsBuilder.UseSqlite("Data Source=apiusuarios.db");
        }
    }
}
