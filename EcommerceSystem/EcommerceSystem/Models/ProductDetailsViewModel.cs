using System.ComponentModel.DataAnnotations;

namespace EcommerceSystem.Models
{
    public class ProductDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string StockStatus { get; set; }
        public List<ProductImage> Images { get; set; }
        public List<Product> RelatedProducts { get; set; }
    }
}



