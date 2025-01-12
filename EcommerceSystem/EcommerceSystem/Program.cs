    using EcommerceSystem.Models;
    using EcommerceSystem.Services;
    using Microsoft.EntityFrameworkCore;

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    // This service adds a a client code of the EcommerceSystem which is the ProductService
    // ProductService is a client that sends HTTP request which is HTTP GET to retrieve a list of products from the Inventory System
    // In the code below, the ProductService is a client of Inventory System
    // The URL of the Inventory System is provided below to specify if in what system the Product Service client is making an HTTP request for
    builder.Services.AddHttpClient<ProductService>(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5000/"); // Replace with Inventory System URL
    });

    // Inventory service
    builder.Services.AddHttpClient<InventoryService>(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5000/"); // Replace with Inventory System URL
    });


    // Add services to the container.
    builder.Services.AddControllersWithViews();

    // Configure DbContext
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Add session services to the container
    builder.Services.AddDistributedMemoryCache(); // Adds in-memory caching for session
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout as per your requirement
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

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    // Add session middleware to the request pipeline
    app.UseSession(); // This should come before UseAuthorization

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=CustomerIndex}");

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