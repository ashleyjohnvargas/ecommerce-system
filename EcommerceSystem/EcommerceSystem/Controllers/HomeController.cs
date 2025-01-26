using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EcommerceSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EcommerceSystem.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    // This returns the HTML file of the HomePage or Index Page
    public IActionResult Index()
    {
        return View();
    }

    // This returns the Index or HomePage of the Customer user
    public IActionResult CustomerIndex()
    {
        // Get the logged-in user/customer's id from session
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction("Authentication", "Account"); // or any page you prefer
        }

        // 1. Get all unique categories ordered by the customer
        var orderedProducts = _context.OrderItems
            .Where(oi => oi.Order.CustomerId == userId)
            .GroupBy(oi => oi.Product.Category)
            .Select(g => new
            {
                Category = g.Key,
                OrdersCount = g.Count()
            }).ToList();

        var totalOrders = orderedProducts.Sum(op => op.OrdersCount);

        var categoryData = orderedProducts.Select(op => new CategoryData
        {
            Category = op.Category,
            Percentage = totalOrders > 0 ? (op.OrdersCount / (double)totalOrders) * 100 : 0
        }).ToList();

        // 2. Get total spending per month by the customer
        var monthlySpendingData = _context.Orders
            .Where(o => o.CustomerId == userId && o.CreatedAt.HasValue)
            .GroupBy(o => o.CreatedAt.Value.Month)
            .Select(g => new
            {
                Month = g.Key,
                TotalSpending = g.Sum(o => o.TotalPrice)
            })
            .OrderBy(g => g.Month)
            .ToList();

        var monthNames = new[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

        var monthlySpending = monthlySpendingData.Select(ms => new MonthlySpendingData
        {
            Month = monthNames[ms.Month - 1],
            TotalSpending = ms.TotalSpending
        }).ToList();

        // Create the view model
        var viewModel = new CustomerAnalyticsViewModel
        {
            CategoryData = categoryData,
            MonthlySpendingData = monthlySpending
        };

        return View(viewModel);
    }



    // This returns the HTML file of the About page
    public IActionResult About()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
