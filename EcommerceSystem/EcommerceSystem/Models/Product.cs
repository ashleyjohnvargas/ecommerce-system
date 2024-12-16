using System.ComponentModel.DataAnnotations;

namespace EcommerceSystem.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        [Required]
        public required string Category { get; set; }
        public bool IsDeleted { get; set; } // New column for soft delete
         // Add a collection of ProductImages
        public List<ProductImage> Images { get; set; }

        // New computed property for image status
        public string ImageStatus
        {
            get
            {
                if (Images == null || !Images.Any())
                {
                    return "No image yet";
                }
                else if (Images.Any())
                {
                    return "Image uploaded successfully";
                }
                return "No image uploaded";
            }
        }
    }
}
