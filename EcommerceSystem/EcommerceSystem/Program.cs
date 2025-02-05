    using EcommerceSystem.Models;
    using EcommerceSystem.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using static EcommerceSystem.Controllers.AccountController;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// This service adds a a client code of the EcommerceSystem which is the ProductService
// ProductService is a client that sends HTTP request which is HTTP GET to retrieve a list of products from the Inventory System
// In the code below, the ProductService is a client of Inventory System
// The URL of the Inventory System is provided below to specify if in what system the Product Service client is making an HTTP request for
builder.Services.AddHttpClient<ProductService>(client =>
{
    client.BaseAddress = new Uri("https://gizmodeinventorysystem2.azurewebsites.net/"); // Replace with Inventory System URL
    //client.DefaultRequestHeaders.Add("Authorization", "Bearer YOUR_ACCESS_TOKEN");

});

// Inventory service
builder.Services.AddHttpClient<InventoryService>(client =>
{
    client.BaseAddress = new Uri("https://gizmodeinventorysystem2.azurewebsites.net/"); // Replace with Inventory System URL
});

////// POS Service
builder.Services.AddHttpClient<PosService>(client =>
{
    client.BaseAddress = new Uri("https://gizmodepossystem.azurewebsites.net"); // Replace with POS URL
    // client.DefaultRequestHeaders.Add("Accept", "application/json");
});

//builder.Services.AddHttpClient<ProductService>(client =>
//{
//    client.BaseAddress = new Uri("https://gizmodeinventorysystem2.azurewebsites.net/");
//    client.DefaultRequestHeaders.Add("Authorization", "Bearer YOUR_ACCESS_TOKEN");
//});

//Local host

//builder.Services.AddHttpClient<ProductService>(client =>
//{
//    client.BaseAddress = new Uri("https://localhost:44341/"); // Replace with Inventory System URL
//});

////Inventory service
//builder.Services.AddHttpClient<InventoryService>(client =>
//{
//    client.BaseAddress = new Uri("https://localhost:44341/"); // Replace with Inventory System URL
//});

////POS Service
//builder.Services.AddHttpClient<PosService>(client =>
//{
//    client.BaseAddress = new Uri("https://localhost:44359/"); // Replace with POS URL
//    // client.DefaultRequestHeaders.Add("Accept", "application/json");
//});


//builder.Services.AddHttpClient<ProductService>(client =>
//{
//    client.BaseAddress = new Uri("https://gizmodeinventorysystem2.azurewebsites.net"); // Replace with Inventory System URL
//});

////// Inventory service
//builder.Services.AddHttpClient<InventoryService>(client =>
//{
//    client.BaseAddress = new Uri("https://gizmodeinventorysystem.azurewebsites.net"); // Replace with Inventory System URL
//});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

//builder.Services.AddRazorPages();
// Configure DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,                // Maximum number of retries
                maxRetryDelay: TimeSpan.FromSeconds(5),  // Time to wait between retries
                errorNumbersToAdd: null          // Optional: specify specific SQL error numbers to retry on
                )
            )
        
        );

    // Add session services to the container
    //builder.Services.AddDistributedMemoryCache(); // Adds in-memory caching for session
    //builder.Services.AddSession(options =>
    //{
    //    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout as per your requirement
    //    options.Cookie.HttpOnly = true; // Only accessible by the server
    //    options.Cookie.IsEssential = true; // Necessary for session management
    //});
// Add background service for checking inactive users
builder.Services.AddHostedService<InactiveUserChecker>();

// configure authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Login/LoginPage"; // Redirect to login if unauthorized
            options.LogoutPath = "/Logout";   // Redirect when logging out
            options.AccessDeniedPath = "/Login/AccessDenied"; // Handle access denied scenario
        });


// Add session services to the container
builder.Services.AddDistributedMemoryCache(); // Adds in-memory caching for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Set session timeout as per your requirement
    options.Cookie.HttpOnly = true; // Only accessible by the server
    options.Cookie.IsEssential = true; // Necessary for session management
});

var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseStatusCodePagesWithReExecute("/Home/NotFound", "?code={0}"); // Handle 404 page
        app.UseHsts();
    }

app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    // Add session middleware to the request pipeline
    app.UseSession(); // This should come before UseAuthorization
    app.UseAuthentication();  // If using authentication
    app.UseAuthorization();

// View for prospective users
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=CustomerIndex}");

// View for prospective users
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Initial}");

// View for admin
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}");

// View for admin
//app.MapControllerRoute(
//      name: "default",
//      pattern: "{controller=Product}/{action=Product}");


app.Run();

    /*
    Things/Services that are added in the container:

    1. Add the DbContext: builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    2. Add the HttpClient: builder.Services.AddHttpClient<ProductService>(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5263/"); // Replace with Inventory System URL
    });

    3. Add the Controllers:
    builder.Services.AddControllersWithViews();
    builder.Services.AddControllers();
    */