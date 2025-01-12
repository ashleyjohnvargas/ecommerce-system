using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommerceSystem.Models
{
    public class ProductImageCopy
    {
        [Key]
        public int ImageId { get; set; } // Primary key

        [ForeignKey(nameof(ProductCopy))] // Specifies that ProductId links to the Product navigation property
        public int? ProductId { get; set; } // Foreign key linking to Product

        public string? FilePath { get; set; } // Path of the uploaded image

        // Navigation property for the related Product
        public virtual ProductCopy? ProductCopy { get; set; }
    }
}
