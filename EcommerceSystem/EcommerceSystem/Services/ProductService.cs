/*
This code is the HttpClient. This contains all the requests of this system, which is the EcommerceSystem to other systems.
But this code specifically contains all the requests of Ecommerce System related to Products model or stuff.
That's why as you can see below, this HttpClient contains the action GetAllProductsAsync, which is an HTTP GET request of Ecommerce
to the Inventory System because the Ecommerce is requesting for the list of all products from the Inventory System.
The provided URL or API route in the GetAllProductsAsync action is where this specific request is connecting to, which is the route:
api/ProductsApi/GetAllProducts. The GetAllProducts in the route is an action or API Endpoint in the ProductsController of the InventorySystem.
The ProductsController of the InventorySystem contains an action or API Endpoint which is the GetAllProducts() that returns the list
of all products in the Inventory. Then the GetAllProductsAsync action below receives the response as a list data structure. 
*/

using EcommerceSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace EcommerceSystem.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;

        public ProductService(HttpClient httpClient, ApplicationDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            var response = await _httpClient.GetAsync("api/ProductsApi/GetAllProducts"); // Correct endpoint
            response.EnsureSuccessStatusCode();
            var products = await response.Content.ReadFromJsonAsync<List<Product>>();

            // Persist the products into the EcommerceSystem database
            if (products != null && products.Any())
            {
                await PersistProductsAsync(products);
            }

            return products ?? new List<Product>(); // Return an empty list if no products are found
        }

       private async Task PersistProductsAsync(List<Product> products)
        {
            // Delete all existing products in the database before adding new ones
            try
            {
                Console.WriteLine("Deleting existing products...");
                _context.Products.RemoveRange(_context.Products); // Removes all existing products
                await _context.SaveChangesAsync(); // Save changes after deletion
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting products: {ex.Message}");
                // Optionally log additional details about the exception, such as stack trace
            }

            // Reset the identity of the Products table to 1 before adding new records
            try
            {
                Console.WriteLine("Resetting identity for Products table...");
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Products', RESEED, 0)"); // Resets identity to 1
                Console.WriteLine("Identity reset successful.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error resetting identity: {ex.Message}");
                // Optionally log additional details about the exception
            }

            // Add new products to the database
            foreach (var product in products)
            {
                // No need to check if the product exists because the table has been cleared
                _context.Products.Add(new Product
                {
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Color = product.Color,
                    Category = product.Category,
                    OriginalStock = product.OriginalStock,
                    CurrentStock = product.CurrentStock,
                    StockStatus = product.StockStatus,
                    IsBeingSold = product.IsBeingSold,
                    IsDeleted = product.IsDeleted,
                    DateAdded = product.DateAdded
                });

                Console.WriteLine($"New product added: {product.Name}");
            }

            // Attempt to save changes to the database, handle exceptions if any occur
            try
            {
                Console.WriteLine("Saving new products...");
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving products: {ex.Message}");
                // Optionally log additional details about the exception, such as stack trace
            }
        }


    }
}
