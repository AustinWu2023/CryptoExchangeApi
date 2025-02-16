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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=AUSTIN-PC;Initial Catalog=CryptoExchange;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CryptoCurrency>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CryptoCu__3214EC0740BE07C5");

            entity.Property(e => e.CurrencyCode).HasMaxLength(3);
            entity.Property(e => e.CurrencyName).HasMaxLength(50);
            entity.Property(e => e.ExchangeRate).HasColumnType("decimal(18, 8)");
            entity.Property(e => e.UpdatedAtUTC).HasDefaultValueSql("(getutcdate())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
