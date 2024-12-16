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
        // Temporary storage for products (e.g., in-memory cache)
        private static List<Product> _temporaryProductList = new List<Product>();
        //private readonly ApplicationDbContext _context;
        private readonly ApplicationDbContext _context; // Inject ApplicationDbContext
        public ProductController(ILogger<ProductController> logger, ProductService productService, ApplicationDbContext context)
        {
            _logger = logger;
            _productService = productService;
            _context = context; // Assign ApplicationDbContext to a field
        }

        // returns product page html view
        public async Task<IActionResult> Product(int page = 1)
        {
            int pageSize = 10; // Number of products per page

            // Fetch all products from the Inventory System using ProductService
            var allProducts = await _productService.GetAllProductsAsync();

            // Store the products in memory (for temporary use)
            _temporaryProductList = allProducts;

            // Calculate total products and pages
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
            // Get the selected product from the temporary list
            var product = _temporaryProductList.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Return the Add Image page view with the selected product
            return View(product);
        }

        // Handle the image upload (this would typically save the image to the server or database)
       [HttpPost]
        public async Task<IActionResult> AddImage(int id, IFormFile imageFile)
        {
            var product = _temporaryProductList.FirstOrDefault(p => p.Id == id);

            if (product == null || imageFile == null)
            {
                return NotFound();
            }

            // Example: Save the image file (this is just an example - adjust to your actual implementation)
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", imageFile.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Add image details to the product
           var productImage = new ProductImage
            {
                ProductId = product.Id,
                FilePath = "/images/products/" + imageFile.FileName
            };

            // Save the ProductImage to the database using ApplicationDbContext
            _context.ProductImages.Add(productImage);
            await _context.SaveChangesAsync();

            // Assuming a method to save the product image info (this could be a call to your DbContext)
            // In this case, you can simply add the image to the product for the temporary list
            product.Images = product.Images ?? new List<ProductImage>();
            product.Images.Add(productImage);

            // After adding the image, redirect to the product page or the product list page
            return RedirectToAction("Product");
        }



       public async Task<IActionResult> ProductDetails()
        {
            // Fetch all products from the temporary list
            var products = _temporaryProductList;

            // Fetch images for each product from the database
            foreach (var product in products)
            {
                product.Images = await _context.ProductImages
                                            .Where(pi => pi.ProductId == product.Id)
                                            .ToListAsync();
            }

            // Pass the list of products to the view
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
