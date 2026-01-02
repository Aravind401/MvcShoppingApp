using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcShoppingApp.Data;
using MvcShoppingApp.Models;

namespace MvcShoppingApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                // Store user info in session
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserName", user.FullName);
                HttpContext.Session.SetString("UserRole", user.Role);

                TempData["SuccessMessage"] = $"Welcome back, {user.FullName}!";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid email or password");
            return View();
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(User user, string ConfirmPassword)
        {
            if (user.Password != ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match");
                return View(user);
            }

            if (ModelState.IsValid)
            {
                // Check if email already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == user.Email);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email already registered");
                    return View(user);
                }

                // Set default role for new users
                user.Role = "Customer";
                user.CreatedDate = DateTime.Now;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // GET: /Account/Profile
        [HttpGet]
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Please login to view your profile.";
                return RedirectToAction("Login");
            }

            var user = new User
            {
                Email = HttpContext.Session.GetString("UserEmail") ?? "",
                FullName = HttpContext.Session.GetString("UserName") ?? "",
                Role = HttpContext.Session.GetString("UserRole") ?? ""
            };

            return View(user);
        }

        // POST: /Account/Logout
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }
    }
}