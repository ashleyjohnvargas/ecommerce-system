using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EcommerceSystem.Models;

namespace EcommerceSystem.Controllers.Api 
{
    [Route("api/[controller]/[action]")]
    [ApiController] // Specifies that this is an API controller
    public class ProductsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsApiController(ApplicationDbContext context)
        {
            _context = context;
        }


    //Route: api/ProductsApi/EditProductFromInventory
    public IActionResult EditProductFromInventory([FromBody] Product product)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid product data.");
        }

        // Find the product in the EcommerceSystem database
        var existingProduct = _context.Products.FirstOrDefault(p => p.Id == product.Id);

        if (existingProduct == null)
        {
            return NotFound($"Product with ID {product.Id} not found.");
        }

        // Update the product's details
        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.Color = product.Color;
        existingProduct.Category = product.Category;
        existingProduct.OriginalStock = product.OriginalStock;
        existingProduct.CurrentStock = product.CurrentStock;
        existingProduct.StockStatus = product.StockStatus;
        // existingProduct.IsBeingSold = product.IsBeingSold;
        // existingProduct.IsDeleted= product.IsDeleted;

        // Save changes to the database
        _context.SaveChanges();

        return Ok(new { Message = "Product updated successfully in InventorySystem." });
    }


       
    }
}