using System.ComponentModel.DataAnnotations;

namespace EcommerceSystem.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public int ProductId { get; set; }  // Link the image to a specific product
        public string FilePath { get; set; } // The path of the uploaded image
    }
}
