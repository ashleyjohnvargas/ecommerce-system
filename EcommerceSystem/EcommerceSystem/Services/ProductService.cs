/*
This code is the HttpClient. This contains all the requests of this system, which is the EcommerceSystem to other systems.
But this code specifically contains all the requests of Ecommerce System related to Products model or stuff.
That's why as you can see below, this HttpClient contains the action GetAllProductsAsync, which is an HTTP GET request of Ecommerce
to the Inventory System because the Ecommerce is requesting for the list of all products from the Inventory System.
The provided URL or API route in the GetAllProductsAsync action is where this specific request is connecting to, which is the route:
api/Products/GetAllProducts. The GetAllProducts in the route is an action or API Endpoint in the ProductsController of the InventorySystem.
The ProductsController of the InventorySystem contains an action or API Endpoint which is the GetAllProducts() that returns the list
of all products in the Inventory. Then the GetAllProductsAsync action below receives the response as a list data structure. 
*/

using EcommerceSystem.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace EcommerceSystem.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            var response = await _httpClient.GetAsync("api/ProductsApi/GetAllProducts"); // Correct endpoint
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Product>>();
        }
    }
}
