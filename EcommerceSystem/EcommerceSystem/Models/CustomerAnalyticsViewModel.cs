using System.ComponentModel.DataAnnotations;

namespace EcommerceSystem.Models
{
    public class CustomerAnalyticsViewModel
    {
        public List<CategoryData> CategoryData { get; set; }
        public List<MonthlySpendingData> MonthlySpendingData { get; set; }
    }

}