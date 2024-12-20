using Microsoft.EntityFrameworkCore;

namespace EcommerceSystem.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set;}
        public DbSet<ProductImage> ProductImages { get; set;}


        // Uncomment this if you need to bring back the foreign key relationship of Products and ProductImages
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