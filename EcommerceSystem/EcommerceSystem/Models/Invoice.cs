using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommerceSystem.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        [ForeignKey("Order")]
        public int? OrderId { get; set; }

        [ForeignKey("Customer")]
        public int? CustomerId { get; set; }
        public DateTime? SaleDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentMethod { get; set; }

        // // Navigation property for the related Order and Customer
        // public virtual Order? Order { get; set; }  // Navigation property to the related Order
        // public virtual Customer? Customer { get; set; }  // Navigation property to the related Customer
    }
}
