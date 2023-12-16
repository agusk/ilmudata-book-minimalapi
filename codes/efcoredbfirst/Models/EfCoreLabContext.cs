using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace efcoredbfirst.Models;

public partial class EfCoreLabContext : DbContext
{
    public EfCoreLabContext()
    {
    }

    public EfCoreLabContext(DbContextOptions<EfCoreLabContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; }

//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//         => optionsBuilder.UseSqlServer("server=localhost;database=EfCoreLab;uid=tester;pwd=pass123;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Product__3214EC0730219588");

            entity.ToTable("Product");

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
