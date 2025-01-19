using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommerceSystem.Models
{
    public class OrderHistoryViewModel
    {
        public int OrderId { get; set; }
        public string? CreatedAt { get; set; } // The order date formatted as string
        public string? OrderStatus { get; set; }
        public int ItemsCount { get; set; }
    }

}