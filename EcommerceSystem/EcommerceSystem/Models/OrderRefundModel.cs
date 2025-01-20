using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommerceSystem.Models
{
    public class OrderRefundModel
    {
        public int OrderId { get; set; }
        public DateTime RefundDate { get; set; }
    }
}