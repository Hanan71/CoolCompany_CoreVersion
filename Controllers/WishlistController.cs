using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CoolCompanyEstore.Data;
using CoolCompanyEstore.Models;
using System.Threading.Tasks;
using System.Linq;

namespace CoolCompanyEstore.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WishlistController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var wishlistItems = await _context.Wishlists
                .Where(w => w.UserId == user.Id)
                .Include(w => w.Product)
                .ThenInclude(p => p.Images) // هذا هو السطر الذي يجب إضافته
                .ToListAsync();

            return View(wishlistItems);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var exists = await _context.Wishlists
                .AnyAsync(w => w.UserId == user.Id && w.ProductId == productId);

            if (!exists)
            {
                _context.Wishlists.Add(new Wishlist
                {
                    UserId = user.Id,
                    ProductId = productId
                });

                await _context.SaveChangesAsync();
                TempData["WishlistMessage"] = " Product added to your wishlist.";
            }
            else
            {
                TempData["WishlistMessage"] = " Product is already in your wishlist.";
            }

            return RedirectToAction("Details", "Products", new { id = productId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var item = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == user.Id);

            if (item != null)
            {
                _context.Wishlists.Remove(item);
                await _context.SaveChangesAsync();
                TempData["WishlistMessage"] = " Product removed from your wishlist.";
            }
            else
            {
                TempData["WishlistMessage"] = " Item not found in your wishlist.";
            }

            return RedirectToAction("Index");
        }
    }
}
