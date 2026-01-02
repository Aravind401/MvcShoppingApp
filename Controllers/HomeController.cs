using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcShoppingApp.Data;
using MvcShoppingApp.Models;

namespace MvcShoppingApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _db.Products.ToListAsync();
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}