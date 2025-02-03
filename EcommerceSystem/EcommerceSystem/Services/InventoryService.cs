using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EcommerceSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceSystem.Services
{
    public class InventoryService
    {
        private readonly HttpClient _httpClient;

        public InventoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public virtual async Task<bool> UpdateProductInInventorySystem(Product product)
        {
            try
            {
                // InventorySystem API endpoint for editing product details
                string inventoryApiUrl = "api/ProductsApi/EditProductFromEcommerce";

                // Send the product details as JSON to the InventorySystem API
                // It used "PostAsJsonAsync" because product is not yet in JSON format, it is still an object or instance of Product model
                // Thus, there is a word "Json" in "PostAsJsonAsync"
                // But if product is already in JSON format, then it can use "PostAsync" instead
                // Observe the productDto sample below. Go there and read my comment.
                var response = await _httpClient.PostAsJsonAsync(inventoryApiUrl, product);

                // Check if the request was successful
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Log the exception (implement a logging mechanism as needed)
                Console.WriteLine($"Error updating product in InventorySystem: {ex.Message}");
                return false;
            }

            // The productDto is also an object or instance of ProductDto model

            // var productDto = new
            // {
            //     Id = product.Id,
            //     Name = product.Name,
            //     Description = product.Description,
            //     Price = product.Price,
            //     Color = product.Color,
            //     Category = product.Category
            // };

            // IN ORDER TO CONVERT THE productDto INTO JSON FORMAT, the following code is used below:

            // var jsonContent = JsonSerializer.Serialize(productDto);
            // var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // THE content is a JSON content that contains the productDto in JSON format, therefore it will be the one to be sent to the InventorySystem API

            // var response = await _httpClient.PostAsync("https://inventorysystem.example.com/api/products/edit", content);

            // return response.IsSuccessStatusCode;
        }


        public virtual async Task<bool> DeleteProductInInventorySystem(int productId)
        {
            try
            {
                // InventorySystem API endpoint for setting a product as not being sold
                string inventoryApiUrl = $"api/ProductsApi/SetProductAsNotBeingSold/{productId}";

                // Create an empty content for the PUT request
                // The empty content in the StringContent object is used to fulfill the requirement of the PutAsync method,
                //  which expects an HttpContent parameter. In this case, the PUT request does not need to send any data in 
                //  the body, so an empty StringContent is provided.
                var content = new StringContent(string.Empty);

                // Send a PUT request to the InventorySystem API
                var response = await _httpClient.PutAsync(inventoryApiUrl, content);

                // Check if the request was successful
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Log the exception (implement a logging mechanism as needed)
                Console.WriteLine($"Error deleting product in InventorySystem: {ex.Message}");
                return false;
            }
        }
    }
}



