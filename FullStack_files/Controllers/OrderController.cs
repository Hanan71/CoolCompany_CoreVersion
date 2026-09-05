using CoolCompanyEstore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CoolCompanyEstore.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace CoolCompanyEstore.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> CreateOrder()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var userId = user.Id;

            if (TempData["CheckoutModel"] is not string checkoutJson)
                return RedirectToAction("Index", "Checkout");

            var model = JsonSerializer.Deserialize<CheckoutViewModel>(checkoutJson);
            if (model == null)
                return RedirectToAction("Index", "Checkout");

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .ThenInclude(p => p.Images) // تم التعديل هنا: تضمين صور المنتج
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any() || cartItems.Any(c => c.Product == null))
            {
                TempData["CartMessage"] = "There was a problem with your cart. Please try again.";
                return RedirectToAction("Index", "Cart");
            }

            var total = cartItems.Sum(item => item.Product!.Price * item.Quantity);

            var order = new Order
            {
                UserId = userId,
                FullName = model.FullName,
                ShippingAddress = model.ShippingAddress,
                PaymentMethod = model.PaymentMethod,
                OrderDate = System.DateTime.Now,
                Status = "Processing",
                TotalAmount = total,
                Items = cartItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    SelectedColor = item.SelectedColor,
                    SelectedSize = item.SelectedSize,
                    Price = item.Product!.Price,
                    ProductName = item.Product.Name,
                    ProductSKU = item.Product.SKU,
                    // التعديل هنا: أخذ مسار أول صورة فقط من قائمة الصور
                    ProductImagePath = item.Product.Images.FirstOrDefault()?.ImagePath ?? "/img/default-product.jpg"
                }).ToList()
            };

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            TempData["OrderSuccessMessage"] = "Thank you! Your order is on the way. Check your email for the order details.";
            return RedirectToAction("Index", "Cart");
        }


        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var userId = user.Id;

            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Images) // تم التعديل هنا: تضمين صور المنتج
                .Where(o => o.UserId == userId)
                .ToListAsync();

            return View(orders);
        }


        [HttpGet]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var userId = user.Id;

            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p!.Images) // تم إضافة ! هنا
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

    }
}
