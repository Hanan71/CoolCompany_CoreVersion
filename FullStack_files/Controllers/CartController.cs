using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CoolCompanyEstore.Data;
using CoolCompanyEstore.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System; // تأكد من إضافة هذا السطر لاستخدام DateTime.UtcNow

namespace CoolCompanyEstore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            ViewBag.UserName = user.FullName ?? user.UserName;
            ViewBag.ProfileImageUrl = string.IsNullOrEmpty(user.ProfileImageUrl)
                ? "/img/default-user.png"
                : user.ProfileImageUrl;

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                    .ThenInclude(p => p.Images) // تم إضافة هذا السطر لحل المشكلة
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            return View(cartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, string? color, string? size, int quantity = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return NotFound();

            var existingItem = await _context.CartItems.FirstOrDefaultAsync(c =>
                c.ProductId == productId &&
                c.UserId == user.Id &&
                c.SelectedColor == color &&
                c.SelectedSize == size);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    ProductId = productId,
                    UserId = user.Id,
                    Quantity = quantity,
                    SelectedColor = color,
                    SelectedSize = size,
                    DateAdded = DateTime.UtcNow
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            TempData["CartMessage"] = "Product added to cart successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCartQuick(int productId, int quantity = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return NotFound();

            var existingItem = await _context.CartItems.FirstOrDefaultAsync(c =>
                c.ProductId == productId &&
                c.UserId == user.Id &&
                string.IsNullOrEmpty(c.SelectedColor) &&
                string.IsNullOrEmpty(c.SelectedSize));

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    ProductId = productId,
                    UserId = user.Id,
                    Quantity = quantity,
                    SelectedColor = null,
                    SelectedSize = null,
                    DateAdded = DateTime.UtcNow
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            TempData["CartMessage"] = "Product added to cart successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var item = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == id && c.UserId == user.Id);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var item = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == id && c.UserId == user.Id);
            if (item != null && quantity > 0)
            {
                item.Quantity = quantity;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}

