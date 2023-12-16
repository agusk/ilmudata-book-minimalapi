using Microsoft.EntityFrameworkCore;

namespace efcoretrans.Models;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Define precision for decimal property
        modelBuilder.Entity<Product>()
            .Property(product => product.Price)
            .HasPrecision(18, 2);
    }
}