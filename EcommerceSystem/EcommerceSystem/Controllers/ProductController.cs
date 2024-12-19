using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EcommerceSystem.Models;
using EcommerceSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace EcommerceSystem.Controllers 
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ProductService _productService;
        private readonly ApplicationDbContext _context;
        public ProductController(ILogger<ProductController> logger, ProductService productService, ApplicationDbContext context)
        {
            _logger = logger;
            _productService = productService;
            _context = context;
        }

        // returns product page html view
        public async Task<IActionResult> Product(int page = 1)
        {
            int pageSize = 10; // Number of products per page

            List<Product> allProducts;

            try
            {
                // Fetch all products from the InventorySystem using ProductService
                allProducts = await _productService.GetAllProductsAsync();
            }
            catch (Exception ex)
            {
                // Log any error if the request to the Inventory System fails
                _logger.LogError("Error fetching products from Inventory System: {Error}", ex.Message);
                allProducts = new List<Product>(); // Fallback to an empty list if an error occurs
            }

                // Debugging log to ensure products are fetched
            _logger.LogInformation("Fetched {ProductCount} products", allProducts.Count);

            // Store the products in memory (for temporary use)
            var totalProducts = allProducts.Count;
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            // Fetch products for the current page
            var products = allProducts
                            .Skip((page - 1) * pageSize) // Skip products from previous pages
                            .Take(pageSize) // Take products for the current page
                            .ToList();

            var model = new PaginatedProductModel
            {
                Products = products,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return View(model);
        }



        // Returns the Add Image page view for the selected product
        public IActionResult AddImagePage(int id)
        {
            // Fetch the product from the database and include the related images
            var product = _context.Products
                .Include(p => p.Images) // Include images for the product
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Return the Add Image page view with the selected product
            return View(product);
        }

        // Handle the image upload (this would save the image to the server and add metadata to the ProductImages table)
        [HttpPost]
        public async Task<IActionResult> AddImage(int id, IFormFile imageFile)
        {
            // Fetch the product from the database
            var product = _context.Products.FirstOrDefault(p => p.Id == id);

            if (product == null || imageFile == null)
            {
                return NotFound();
            }

            // Define the file path
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", imageFile.FileName);

            // Save the image file to the server    
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Add image details to the ProductImages table
            var productImage = new ProductImage
            {
                ProductId = product.Id,
                FilePath = "/images/products/" + imageFile.FileName // Save relative path
            };

            // Add the new ProductImage to the context and save it
            _context.ProductImages.Add(productImage);
            await _context.SaveChangesAsync();

            // Redirect to the Product page (or wherever you'd like)
            return RedirectToAction("Product");
        }




       public IActionResult ProductDetails()
        {
            // Fetch products and include their associated images
            var products = _context.Products.Include(p => p.Images).ToList();
            return View(products);
        }





        // // For pagination of Product Details
        // public async Task<IActionResult> ProductDetails(int page = 1)
        // {
        //     int pageSize = 9; // 9 products per page (3 rows of 3 products)

        //     // Fetch all products from the database
        //     var products = _context.Products.Include(p => p.Images);

        //     // Apply pagination
        //     var paginatedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        //     // Get the total number of products to calculate total pages
        //     var totalProducts = products.Count();
        //     var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

        //     // Create a PaginatedProductModel to pass the paginated data
        //     var model = new PaginatedProductModel
        //     {
        //         Products = paginatedProducts,
        //         CurrentPage = page,
        //         TotalPages = totalPages,
        //         PageSize = pageSize
        //     };

        //     return View(model);
        // }



    }
}
