using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceSystem.Models
{
    public class TransactionItemDto
    {
        public int? TransactionId { get; set; }  // Foreign key to Transactions table
        public int? ProductId { get; set; }  // Foreign key to Products table
        public int? Quantity { get; set; }  // Quantity of the product
        public decimal? Subtotal { get; set; }  // Subtotal price for the product
    }
}