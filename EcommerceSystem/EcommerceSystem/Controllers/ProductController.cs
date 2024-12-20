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
        private readonly InventoryService _inventoryService;
        public ProductController(ILogger<ProductController> logger, ProductService productService, ApplicationDbContext context, InventoryService inventoryService)
        {
            _logger = logger;
            _productService = productService;
            _context = context;
            _inventoryService = inventoryService;
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


        // GET: EditProduct
        public IActionResult EditProductPage(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

// Action for editing product details
        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product, IFormFile? Image)
        {
            if (!ModelState.IsValid)
            {
                // Return the user back to the form if the model state is invalid
                Console.WriteLine("Error Editing Product!");
                return View("EditProductPage", product);
            }

            // Handle the image upload if a new image is provided
            if (Image != null && Image.Length > 0)
            {
                // Define the file path to save the image
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", Image.FileName);

                // Save the image to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                // Create a new ProductImage record
                var productImage = new ProductImage
                {
                    ProductId = product.Id,
                    FilePath = $"/images/products/{Image.FileName}"
                };

                // Add the new ProductImage to the context
                _context.ProductImages.Add(productImage);
            }

            // Update the product details in the Inventory System
            await _inventoryService.UpdateProductInInventorySystem(product);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Redirect the user to the product list or details page
            return RedirectToAction("Product");
        }
            

    }
}





/* [HttpPost]
        public async Task<IActionResult> EditProduct(Product product, IFormFile? Image)
        {
            if (!ModelState.IsValid)
            {
                // Return the user back to the form if the model state is invalid
                Console.WriteLine("Error Editing Product!");
                return View("EditProductPage", product);
            }

            // Find the existing product in the database
            var existingProduct = await _context.Products.FindAsync(product.Id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            // Update the product fields with the new values from the form
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Color = product.Color;
            existingProduct.Category = product.Category;

            // Handle the image upload if a new image is provided
            if (Image != null && Image.Length > 0)
            {
                // Define the file path to save the image
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", Image.FileName);

                // Save the image to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                // Create a new ProductImage record
                var productImage = new ProductImage
                {
                    ProductId = product.Id,
                    FilePath = $"/images/products/{Image.FileName}"
                };

                // Add the new ProductImage to the context
                _context.ProductImages.Add(productImage);
            }

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Redirect the user to the product list or details page
            return RedirectToAction("Product");
        }*/