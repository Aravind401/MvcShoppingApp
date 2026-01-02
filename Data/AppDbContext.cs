using Microsoft.EntityFrameworkCore;
using MvcShoppingApp.Models;

namespace MvcShoppingApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

                // Seed initial products
                entity.HasData(
                    new Product
                    {
                        Id = 1,
                        Name = "Laptop",
                        Description = "High performance laptop with 16GB RAM",
                        Price = 999.99m,
                        Category = "Electronics",
                        ImageUrl = "/images/laptop.jpg",
                        StockQuantity = 50
                    },
                    new Product
                    {
                        Id = 2,
                        Name = "Smartphone",
                        Description = "Latest smartphone with 128GB storage",
                        Price = 699.99m,
                        Category = "Electronics",
                        ImageUrl = "/images/phone.jpg",
                        StockQuantity = 100
                    },
                    new Product
                    {
                        Id = 3,
                        Name = "Headphones",
                        Description = "Wireless noise-cancelling headphones",
                        Price = 199.99m,
                        Category = "Electronics",
                        ImageUrl = "/images/headphones.jpg",
                        StockQuantity = 75
                    }
                );
            });

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.Password).IsRequired();

                // Seed initial users
                entity.HasData(
                    new User
                    {
                        Id = 1,
                        Email = "admin@shop.com",
                        Password = "admin123",
                        FullName = "Admin User",
                        Role = "Admin"
                    },
                    new User
                    {
                        Id = 2,
                        Email = "customer@shop.com",
                        Password = "customer123",
                        FullName = "John Doe",
                        Role = "Customer"
                    }
                );
            });

            // Configure CartItem entity
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId);
            });
        }
    }
}