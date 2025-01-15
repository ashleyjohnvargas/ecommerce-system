using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EcommerceSystem.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EcommerceSystem.Services 
{
    public class PosService
    {
        private readonly HttpClient _httpClient;

        public PosService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Send customer details to POS
        public async Task SyncCustomer(Customer customer)
        {
            var response = await _httpClient.PostAsJsonAsync("api/CustomerApi/SyncCustomer", customer);
            response.EnsureSuccessStatusCode();
        }

        // Send order details to POS
        public async Task CreateOrder(Order order)
        {
            var response = await _httpClient.PostAsJsonAsync("api/OrderApi/CreateOrder", order);
            response.EnsureSuccessStatusCode();
        }

        // Send order items to POS
        public async Task CreateOrderItems(List<OrderItemCopy> orderItems)
        {
            // // Serialize and send orderItems to the API endpoint
            // var jsonContent = JsonConvert.SerializeObject(orderItems);
            // var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsJsonAsync("api/OrderItemApi/CreateOrderItems", orderItems);
            response.EnsureSuccessStatusCode();
        }

        // Send invoice details to POS
        public async Task CreateInvoice(Invoice invoice)
        {
            var response = await _httpClient.PostAsJsonAsync("api/InvoiceApi/CreateInvoice", invoice);
            response.EnsureSuccessStatusCode();
        }

        // Send products to POS
        public async Task SyncProducts(List<ProductDto> productDtos)
        {
            var response = await _httpClient.PostAsJsonAsync("api/ProductsApi/SyncProducts", productDtos);
            response.EnsureSuccessStatusCode();
        }
    }

}