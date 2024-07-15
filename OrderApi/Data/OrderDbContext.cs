using Microsoft.EntityFrameworkCore;
using OrderApi.Data.Models;

namespace OrderApi.Data;

public class OrderDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }

    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.Title).IsUnique();
            entity.Property(e => e.Title).HasMaxLength(40).IsRequired();
            entity.Property(e => e.Price).IsRequired();
            entity.Property(e => e.Discount).HasDefaultValue(0).IsRequired();
        });
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreationDate).IsRequired();
            
            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Buyer)
                .WithMany(u => u.Orders)
                .HasForeignKey(e => e.BuyerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}