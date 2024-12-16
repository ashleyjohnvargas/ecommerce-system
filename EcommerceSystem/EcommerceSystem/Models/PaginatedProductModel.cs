using System.ComponentModel.DataAnnotations;

using System.Collections.Generic;

namespace EcommerceSystem.Models
{
    public class PaginatedProductModel
    {
        public List<Product> Products { get; set; }  // List of products for the current page
        public int CurrentPage { get; set; }         // The current page number
        public int TotalPages { get; set; }          // The total number of pages
        public int PageSize { get; set; }            // The number of products per page
    }
}
