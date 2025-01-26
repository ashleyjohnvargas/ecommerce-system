using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommerceSystem.Models
{
    public class Order
    {
        public int OrderId { get; set; }  // Unique identifier for the order
        [ForeignKey("Customer")]
        public int? CustomerId { get; set; }  // Reference to the customer placing the order (nullable)
        public decimal? TotalPrice { get; set; }  // Final price (subtotal + tax + shipping)
        public string? PaymentMethod { get; set; }  // Chosen payment method
        public string? OrderStatus { get; set; }  // Status of the order (e.g., Pending, Shipped, Delivered)
        public DateTime? CreatedAt { get; set; }  // Timestamp when the order was placed (nullable)
        public bool IsDeleted { get; set; } = false; // Indicates if the record is soft-deleted

        // Navigation property for the related User (Customer)
        public virtual User? Customer { get; set; }
        //public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();  // Navigation property to the related OrderItems
    }
}
