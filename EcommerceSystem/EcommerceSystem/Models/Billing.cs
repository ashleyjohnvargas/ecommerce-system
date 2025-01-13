using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace EcommerceSystem.Models
{
    public class Billing
    {
        public int BillingId { get; set; }  // Unique identifier for the billing or invoice record
        [ForeignKey("Order")]
        public int? OrderId { get; set; }  // Reference to the associated order (nullable)
        [ForeignKey("Customer")]
        public int? CustomerId { get; set; }  // Reference to the customer associated with the invoice (nullable)
        public DateTime? BillingDate { get; set; }  // The date the invoice was generated (nullable)
        public DateTime? DueDate { get; set; }  // Payment due date (nullable)
        public decimal? TotalAmount { get; set; }  // The total amount to be paid (nullable)
        public string? PaymentStatus { get; set; }  // Payment status (Pending, Paid, or Overdue)
        public string? BillingAddress { get; set; }  // Address to which the invoice is billed
        public decimal? TaxAmount { get; set; }  // Total tax applied to the order (nullable)
        public decimal? ShippingFee { get; set; }  // Fee charged for shipping (nullable)
        public decimal? GrandTotal { get; set; }  // Final amount including tax and shipping (nullable)
        public string? FirstName { get; set; }  // First name of the customer
        public string? LastName { get; set; }  // Last name of the customer
        public string? CompanyName { get; set; }  // Company name (if applicable)
        public string? PhoneNumber { get; set; }  // Phone number of the customer
        public string? EmailAddress { get; set; }  // Email address of the customer
        public string? PaymentMethod { get; set; }  // Chosen payment method

        // Navigation property for the related Order and Customer
        public virtual Order? Order { get; set; }  // Navigation property to the related Order
        public virtual User? Customer { get; set; }  // Navigation property to the related Customer
    }
}
