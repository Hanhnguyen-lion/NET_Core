using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MyAPI;

public partial class SalesOnlineContext : DbContext
{
    public SalesOnlineContext()
    {
    }

    public SalesOnlineContext(DbContextOptions<SalesOnlineContext> options)
        : base(options)
    {
    }

    public virtual DbSet<InterestsV> InterestsVs { get; set; }

    public virtual DbSet<OrdersT> OrdersTs { get; set; }

    public virtual DbSet<ProductsT> ProductsTs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=sales_online;Port=5432;User Id=postgres;Password=root;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InterestsV>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("interests_v");

            entity.Property(e => e.Amount)
                .HasPrecision(19, 4)
                .HasColumnName("amount");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Interest).HasColumnName("interest");
            entity.Property(e => e.OrderDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("order_date");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Total)
                .HasPrecision(19, 4)
                .HasColumnName("total");
        });

        modelBuilder.Entity<OrdersT>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_orders_t_id");

            entity.ToTable("orders_t");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(19, 4)
                .HasColumnName("amount");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modify_date");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("order_date");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Total)
                .HasPrecision(19, 4)
                .HasComputedColumnSql("((quantity)::numeric * amount)", true)
                .HasColumnName("total");

            entity.HasOne(d => d.Product).WithMany(p => p.OrdersTs)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_orders_products");
        });

        modelBuilder.Entity<ProductsT>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("pk_product_t_id");

            entity.ToTable("products_t");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modify_date");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("order_date");
            entity.Property(e => e.Price)
                .HasPrecision(19, 4)
                .HasColumnName("price");
            entity.Property(e => e.ProductName)
                .HasColumnType("character varying")
                .HasColumnName("product_name");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Total)
                .HasPrecision(19, 4)
                .HasComputedColumnSql("((quantity)::numeric * price)", true)
                .HasColumnName("total");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
