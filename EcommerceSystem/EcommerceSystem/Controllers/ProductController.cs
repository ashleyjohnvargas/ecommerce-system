using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EcommerceSystem.Models;
using EcommerceSystem.Services;

namespace EcommerceSystem.Controllers 
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ProductService? _productService;

        public ProductController(ProductService productService, ILogger<ProductController> logger, ApplicationDbContext context)
        {
            _productService = productService;
            _logger = logger;
            _context = context;

        }

        public async Task<IActionResult> Product()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products from Inventory System");
                return View(new List<Product>()); // Return an empty list on error
            }
        }
    }
}