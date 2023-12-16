using Microsoft.EntityFrameworkCore;

namespace secrestapi.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ApiUser> Users { get; set; }
}