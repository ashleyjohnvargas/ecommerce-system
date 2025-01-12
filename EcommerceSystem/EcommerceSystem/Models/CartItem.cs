using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommerceSystem.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; } // Primary Key

        [ForeignKey("Cart")]
        public int? CartId { get; set; } // Foreign Key to Cart table

        [ForeignKey("Product")]
        public int? ProductId { get; set; } // Foreign Key to Product table

        public int? Quantity { get; set; } // Quantity of the product in the cart

        public decimal? Subtotal { get; set; } // Subtotal for the product (Quantity * Price)

        // Navigation properties
        public virtual Cart? Cart { get; set; } // Navigation property for Cart
        public virtual Product? Product { get; set; } // Navigation property for Product
    }
}
