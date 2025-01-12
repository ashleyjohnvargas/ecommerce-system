using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceSystem.Models
{
    public class ProductCopy
    {
        [Key]
        public int ProductId { get; set; } // Auto-incremented primary key
        public string? Name { get; set; } // Product name with a maximum length of 255

        public string? Description { get; set; } // Optional, allows variable-length text with no defined limit
        public decimal? Price { get; set; } // Decimal with precision 18, scale 2, must be non-negative
        public string? Color { get; set; } // Optional color with a maximum length of 100
        public string? Category { get; set; } // Optional category with a maximum length of 50
        public int? OriginalStock { get; set; } // Non-negative integer for original stock
        public int? CurrentStock { get; set; } // Non-negative integer for current stock
        public string? StockStatus { get; set; } // Optional stock status with a maximum length of 50

        public bool IsBeingSold { get; set; } = true; // Boolean field with a default of true (being sold)

        public bool IsDeleted { get; set; } = false; // Boolean field with a default of false (not deleted)
        public DateTime? DateAdded { get; set; } = DateTime.Now; // Defaults to current date and time

        // Navigation property for related ProductImages
        public virtual ICollection<ProductImageCopy> Images { get; set; }
    }
}
