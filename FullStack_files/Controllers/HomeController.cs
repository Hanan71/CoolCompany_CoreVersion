using CoolCompanyEstore.Data;
using CoolCompanyEstore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace CoolCompanyEstore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // الصفحة الرئيسية: عرض الكاروسيل + الفلاتر + المميز + الأحدث
        public async Task<IActionResult> Index(string category, string color, string size, string search, decimal? minPrice, decimal? maxPrice)
        {
            // عرض المنتجات المميزة (تم التعديل هنا)
            var featured = await _context.Products
                .Include(p => p.Images)
                .Where(p => p.IsActive && p.IsFeatured)
                .OrderByDescending(p => p.CreatedAt)
                .Take(8)
                .ToListAsync();

            // عرض المنتجات الأحدث (تم التعديل هنا)
            var latest = await _context.Products
                .Include(p => p.Images)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Take(8)
                .ToListAsync();

            // صور الكاروسيل
            var carouselImages = await _context.CarouselImages
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

            // فلاتر البحث (تم التعديل هنا)
            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images) // يجب تضمين الصور هنا أيضًا
                .Where(p => p.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
                products = products.Where(p => p.Category != null && p.Category.Name == category);

            if (!string.IsNullOrEmpty(color))
                products = products.Where(p => p.AvailableColors != null && p.AvailableColors.Contains(color));

            if (!string.IsNullOrEmpty(size))
                products = products.Where(p => p.AvailableSizes != null && p.AvailableSizes.Contains(size));

            if (!string.IsNullOrEmpty(search))
                products = products.Where(p => p.Name.Contains(search));

            if (minPrice.HasValue)
                products = products.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                products = products.Where(p => p.Price <= maxPrice.Value);

            var filtered = await products.ToListAsync();

            var viewModel = new HomeViewModel
            {
                LatestProducts = latest,
                FeaturedProducts = featured,
                FilteredProducts = filtered,
                CarouselImages = carouselImages
            };

            return View(viewModel);
        }

        // الصفحات الثابتة
        public IActionResult About() => View();
        public IActionResult Contact() => View();
        public IActionResult Services() => View();
        public IActionResult Privacy() => View();
        public IActionResult Faq() => View();
        public IActionResult Error() => View();
    }
}
