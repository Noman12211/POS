using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace POS.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FoodItem> FoodItems { get; set; }
    public DbSet<FoodItemVariant> FoodItemVariants { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceItem> InvoiceItems { get; set; }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.Entity<FoodItem>(entity =>
    //    {
    //        entity.HasKey(e => e.Id)
    //            .HasName("PK__FoodItem__3214EC0778CC1A30");

    //        entity.Property(e => e.CreatedDate)
    //            .HasDefaultValueSql("(getdate())");

    //        entity.Property(e => e.IsActive)
    //            .HasDefaultValue(true);
    //    });

    //    modelBuilder.Entity<Invoice>(entity =>
    //    {
    //        entity.HasKey(e => e.Id)
    //            .HasName("PK__Invoices__3214EC0707D04C4F");

    //        entity.Property(e => e.InvoiceDate)
    //            .HasDefaultValueSql("(getdate())");
    //    });

    //    modelBuilder.Entity<InvoiceItem>(entity =>
    //    {
    //        entity.HasKey(e => e.Id)
    //            .HasName("PK__InvoiceI__3214EC07E5713070");

    //        entity.HasOne(d => d.FoodItem)
    //            .WithMany(p => p.InvoiceItems)
    //            .OnDelete(DeleteBehavior.ClientSetNull)
    //            .HasConstraintName("FK_InvoiceItems_FoodItems");

    //        entity.HasOne(d => d.Invoice)
    //            .WithMany(p => p.InvoiceItems)
    //            .OnDelete(DeleteBehavior.ClientSetNull)
    //            .HasConstraintName("FK_InvoiceItems_Invoices");
    //    });

    //    OnModelCreatingPartial(modelBuilder);
    //}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FoodItem>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__FoodItem__3214EC0778CC1A30");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);
        });


        modelBuilder.Entity<FoodItemVariant>(entity =>
        {
            entity.ToTable("FoodItemVariant");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.VariantName)
                .IsRequired();

            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.IsCustomPrice)
                .IsRequired();

            entity.Property(e => e.IsActive)
                .IsRequired();

            entity.HasOne(e => e.FoodItem)
                .WithMany(e => e.Variants)
                .HasForeignKey(e => e.FoodItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__Invoices__3214EC0707D04C4F");

            entity.Property(e => e.InvoiceDate)
                .HasDefaultValueSql("(getdate())");
        });


        modelBuilder.Entity<InvoiceItem>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__InvoiceI__3214EC07E5713070");

            entity.HasOne(d => d.FoodItem)
                .WithMany(p => p.InvoiceItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvoiceItems_FoodItems");

            entity.HasOne(d => d.Invoice)
                .WithMany(p => p.InvoiceItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvoiceItems_Invoices");
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}