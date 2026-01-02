using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcShoppingApp.Data;
using MvcShoppingApp.Models;

namespace MvcShoppingApp.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cartItems = await GetCartItemsAsync();
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction("Index", "Home");
            }

            var sessionId = GetOrCreateSessionId();
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.ProductId == productId && c.SessionId == sessionId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    SessionId = sessionId,
                    AddedDate = DateTime.Now
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"{product.Name} added to cart!";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            if (quantity <= 0)
            {
                return await RemoveFromCart(cartItemId);
            }

            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Item removed from cart.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var sessionId = GetOrCreateSessionId();
            var cartItems = await _context.CartItems
                .Where(c => c.SessionId == sessionId)
                .ToListAsync();

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cart cleared successfully.";
            return RedirectToAction("Index");
        }

        private async Task<List<CartItem>> GetCartItemsAsync()
        {
            var sessionId = GetOrCreateSessionId();
            return await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.SessionId == sessionId)
                .ToListAsync();
        }

        private string GetOrCreateSessionId()
        {
            var sessionId = HttpContext.Session.GetString("CartSessionId");
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("CartSessionId", sessionId);
            }
            return sessionId;
        }
    }
}