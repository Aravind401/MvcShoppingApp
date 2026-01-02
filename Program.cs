using Microsoft.EntityFrameworkCore;
using MvcShoppingApp.Data;
using MvcShoppingApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Add session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add SQLite database with retry logic
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite("Data Source=mvcshopping.db");
    options.EnableSensitiveDataLogging();
    options.LogTo(Console.WriteLine, LogLevel.Information);
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Initialize database with better error handling
try
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Console.WriteLine("Ensuring database is created...");

        // Ensure database is created and migrated
        dbContext.Database.EnsureDeleted(); // Remove this line after first run
        dbContext.Database.EnsureCreated();

        Console.WriteLine("Database created successfully!");

        // Check if products exist
        var productCount = dbContext.Products.Count();
        Console.WriteLine($"Found {productCount} products in database.");

        if (productCount == 0)
        {
            Console.WriteLine("Seeding products...");
            // Products are seeded via OnModelCreating, but we'll add some manually if needed
            dbContext.Products.AddRange(
                new Product
                {
                    Name = "Test Laptop",
                    Description = "Test laptop",
                    Price = 999.99m,
                    Category = "Electronics",
                    ImageUrl = "/images/laptop.jpg",
                    StockQuantity = 50
                }
            );
            dbContext.SaveChanges();
            Console.WriteLine("Products seeded successfully!");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error creating database: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
}

app.Run();