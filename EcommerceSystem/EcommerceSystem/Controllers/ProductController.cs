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

        // This returns the all products page for Customer user
        public async Task<IActionResult> CustomerAllProducts()
        {
            // trigger the GetAllProductsAsync method from the ProductService to update database in Ecommerce based
            // on the products in the Inventory System
            List<Product> allProducts;
            allProducts = await _productService.GetAllProductsAsync();

            var products = _context.Products
                .Where(p => p.IsBeingSold && !p.IsDeleted)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.StockStatus,
                    ImagePath = _context.ProductImages
                        .Where(pi => pi.ProductId == p.Id)
                        .Select(pi => pi.FilePath)
                        .FirstOrDefault() // Get the first image for each product
                })
                .Where(p => !string.IsNullOrEmpty(p.ImagePath)) // Exclude products with no image
                .ToList();

            return View(products);
        }

        // Search action to return filtered products based on search query
        [HttpGet]
        public JsonResult Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                var allProducts = _context.Products
                    .Where(p => p.IsBeingSold && !p.IsDeleted) // Include only active products
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        // Format price with comma and peso symbol
                        FormattedPrice = "₱" + string.Format("{0:N0}", p.Price),
                        p.StockStatus,
                        // Fetch the first image from the navigation property
                        ImagePath = p.Images
                            .Select(pi => pi.FilePath)
                            .FirstOrDefault() // Get the first image for each product
                    })
                    .Where(p => !string.IsNullOrEmpty(p.ImagePath)) // Exclude products with no image
                    .ToList();

                return Json(allProducts);
            }


            var filteredProducts = _context.Products
                .Where(p => p.Name.Contains(query) && p.IsBeingSold && !p.IsDeleted) // Filter products by name and include only active products
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    // Format price with comma and peso symbol
                    FormattedPrice = "₱" + string.Format("{0:N0}", p.Price),
                    p.StockStatus,
                    // Fetch the first image from the navigation property
                    ImagePath = p.Images
                        .Select(pi => pi.FilePath)
                        .FirstOrDefault() // Get the first image for each product
                })
                .Where(p => !string.IsNullOrEmpty(p.ImagePath)) // Exclude products with no image
                .ToList();

            return Json(filteredProducts);
        }



        
        public IActionResult CustProductDetails(int id)
        {
            // Fetch the product details, including images and related products
            var product = _context.Products
                .Include(p => p.Images)
                .FirstOrDefault(p => p.Id == id && !p.IsDeleted && p.IsBeingSold);

            if (product == null)
            {
                return NotFound();
            }

            // Fetch related products in the same category
            var relatedProducts = _context.Products
                .Where(p => p.Category == product.Category && p.Id != product.Id && !p.IsDeleted && p.IsBeingSold)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.StockStatus,
                    Image = _context.ProductImages.Where(pi => pi.ProductId == p.Id).Select(pi => pi.FilePath).FirstOrDefault()
                })
                .Where(p => !string.IsNullOrEmpty(p.Image)) // Exclude products with no image
                .ToList();

            // Exclude the main image from the list of images
            var imagesExcludingMain = product.Images
                .Skip(1) // Skip the first image (assumed to be the main image)
                .Select(img => img.FilePath);

            // Create a model for the view
            var viewModel = new
            {
                product.Id,
                product.Name,
                product.Price,
                product.Description,
                MainImage = product.Images.FirstOrDefault()?.FilePath,
                Images = imagesExcludingMain,
                RelatedProducts = relatedProducts.Select(rp => new
                {
                    rp.Name,
                    rp.Price,
                    rp.StockStatus,
                    rp.Image
                })
            };

            return View("CustProductDetails", viewModel);
        }




         // returns product page html view

        public async Task<IActionResult> Product(int page = 1)
        {
            // Get the current user's ID from session
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (userId == 0)
            {
                return RedirectToAction("Authentication", "Account"); // If not logged in, redirect to login page
            }

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
            // var products = allProducts
            //                 .Where(p => !p.IsDeleted && p.IsBeingSold)  // Exclude soft-deleted products
            //                 .Skip((page - 1) * pageSize) // Skip products from previous pages
            //                 .Take(pageSize) // Take products for the current page
            //                 .ToList();

            var products = _context.Products
                            .Where(p => !p.IsDeleted && p.IsBeingSold)  // Exclude soft-deleted products
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

            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Authentication", "Account"); // or any page you prefer
            }

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
            // Get the current user's ID from session
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (userId == 0)
            {
                return RedirectToAction("Authentication", "Account"); // If not logged in, redirect to login page
            }

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
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Authentication", "Account"); // or any page you prefer
            }

            // Fetch products and include their associated images
            var products = _context.Products
                .Include(p => p.Images)
                            .Include(p => p.Images)
                            .Where(p => !p.IsDeleted && p.IsBeingSold)  // Exclude soft-deleted products
                            .ToList();
            return View(products);
        }

         // GET: EditProduct
        public IActionResult EditProductPage(int id)
        {

            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Authentication", "Account"); // or any page you prefer
            }
            
            var product = _context.Products.Find(id);
            if (product == null)
            {
                Console.WriteLine($"Id of Product: {id}");
                return NotFound();
            }
            Console.WriteLine($"Id of Product: {id}");
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



        // Delete Product in the Ecommerce by setting IsBeingSold to false
        // When a product is deleted in the Ecommerce, the IsDeleted is not set true, instead, the IsBeingSold is set to false in the Inventory System
        // It is not the job of the admin or manager of the Ecommerce to soft delete product by setting ISDeleted to true
        // That is the job of the admin or manager of the Inventory System
        // The Ecommerce admin or manager can only delete a product by setting IsBeingSold to false in the Inventory System
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            await _inventoryService.DeleteProductInInventorySystem(id);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction("Product");
        }    
    }
}
