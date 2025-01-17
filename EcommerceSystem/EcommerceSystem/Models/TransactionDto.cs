using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceSystem.Models
{
    public class TransactionDto
    {
        public int? OrderId { get; set; }  // Foreign key (nullable)
        public decimal? TotalAmount { get; set; }  // Final transaction amount
        public decimal? PaidAmount { get; set; }  // Amount paid by the customer
        public decimal? Change { get; set; }  // Change returned to the customer
        public string? PaymentStatus { get; set; }  // Status of payment
        public string? PaymentMethod { get; set; }  // Chosen payment method
        public DateTime? TransactionDate { get; set; }  // Transaction timestamp

    }
}