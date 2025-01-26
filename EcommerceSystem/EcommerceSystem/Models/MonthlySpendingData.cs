using System.ComponentModel.DataAnnotations;

namespace EcommerceSystem.Models
{
    public class MonthlySpendingData
    {
        public string Month { get; set; }
        public decimal? TotalSpending { get; set; }
    }
}