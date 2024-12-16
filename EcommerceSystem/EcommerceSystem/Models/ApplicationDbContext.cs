using Microsoft.EntityFrameworkCore;

namespace EcommerceSystem.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set;}
        public DbSet<ProductImage> ProductImages { get; set;}
    }
}