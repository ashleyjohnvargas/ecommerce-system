using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EcommerceSystem.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set;}
        public DbSet<ProductImage> ProductImages { get; set;}
        public DbSet<User> Users { get; set; }
        public DbSet<Profile> UserProfiles { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }    
        public DbSet<Billing> Billings { get; set; }
        // public DbSet<TransactionDto> TransactionsDto { get; set; }
        // public DbSet<TransactionItemDto> TransactionItemsDto { get; set; }
        // public DbSet<PaymentDto> PaymentsDto { get; set; }
        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     base.OnModelCreating(modelBuilder);

        //     // Configure the one-to-many relationship between Product and ProductImage
        //     modelBuilder.Entity<ProductImage>()
        //         .HasOne(p => p.Product) // Each ProductImage has one Product
        //         .WithMany(p => p.Images) // Each Product has many ProductImages
        //         .HasForeignKey(p => p.ProductId); // ProductId is the foreign key
        // }

    }
}