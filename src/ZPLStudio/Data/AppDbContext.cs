using Microsoft.EntityFrameworkCore;
using ZPLStudio.Data.Entities;

namespace ZPLStudio.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<ListForTekartonView> ListForTekarton => Set<ListForTekartonView>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ListForTekartonView>(entity =>
        {
            // Представление без ключа, EF Core 6 работает с ним только для чтения.
            entity.HasNoKey();
            entity.ToView("LIST_FOR_TEKARTON_V", "MLSOFT");
        });
    }
}
