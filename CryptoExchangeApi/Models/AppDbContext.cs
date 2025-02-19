using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CryptoExchangeApi.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CryptoCurrency> CryptoCurrency { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CryptoCurrency>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CryptoCu__3214EC07E2D5D417");

            entity.Property(e => e.CurrencyCode).HasMaxLength(8);
            entity.Property(e => e.CurrencyName).HasMaxLength(50);
            entity.Property(e => e.ExchangeRate).HasColumnType("decimal(18, 8)");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("UpdatedAtUTC");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
