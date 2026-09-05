using Microsoft.AspNetCore.Mvc;
using CoolCompanyEstore.Data;
using CoolCompanyEstore.Models;
using CoolCompanyEstore.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CoolCompanyEstore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // عرض كل المنتجات مع تصفية حسب الفئة
        public async Task<IActionResult> Index(string category)
        {
            var categories = await _context.Categories.ToListAsync();

            var productsQuery = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images) // تم التعديل هنا: تضمين الصور
                .Where(p => p.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                productsQuery = productsQuery.Where(p => p.Category != null && p.Category.Name == category);
            }

            var products = await productsQuery.ToListAsync();

            var viewModel = new ProductFormViewModel
            {
                Products = products,
                Categories = categories,
                SelectedCategory = category ?? "",
                AvailableColors = new List<string> { "Black", "White", "Blue", "Red", "Yellow" },
                AvailableSizes = new List<string> { "S", "M", "L", "XL", "XXL" },
                SelectedColors = new List<string>(),
                SelectedSizes = new List<string>(),
                CustomColor = string.Empty,
                CustomSize = string.Empty
            };

            return View(viewModel);
        }

        // صفحة تفاصيل المنتج
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images) // تم التعديل هنا: تضمين الصور
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // عرض نموذج إنشاء منتج جديد مع ViewModel
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            var categories = await _context.Categories.ToListAsync();

            var viewModel = new ProductFormViewModel
            {
                Categories = categories,
                AvailableColors = new List<string> { "Black", "White", "Blue", "Red", "Yellow" },
                AvailableSizes = new List<string> { "S", "M", "L", "XL", "XXL" },
                SelectedColors = new List<string>(),
                SelectedSizes = new List<string>()
            };

            return View(viewModel);
        }

        // معالجة نموذج إنشاء منتج جديد مع ViewModel
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(ProductFormViewModel viewModel)
        {
            // تم التعديل هنا: التحقق من وجود صور قبل أي شيء
            if (viewModel.ImageFiles == null || viewModel.ImageFiles.Count == 0)
            {
                ModelState.AddModelError("ImageFiles", "Please upload at least one product image.");
            }

            if (!ModelState.IsValid)
            {
                viewModel.Categories = await _context.Categories.ToListAsync();
                viewModel.AvailableColors = new List<string> { "Black", "White", "Blue", "Red", "Yellow" };
                viewModel.AvailableSizes = new List<string> { "S", "M", "L", "XL", "XXL" };
                return View(viewModel);
            }

            var allColors = new List<string>(viewModel.SelectedColors ?? new List<string>());
            if (!string.IsNullOrWhiteSpace(viewModel.CustomColor))
            {
                allColors.Add(viewModel.CustomColor.Trim());
            }

            var allSizes = new List<string>(viewModel.SelectedSizes ?? new List<string>());
            if (!string.IsNullOrWhiteSpace(viewModel.CustomSize))
            {
                allSizes.Add(viewModel.CustomSize.Trim());
            }

            var product = new Product
            {
                Name = viewModel.Name,
                Price = viewModel.Price,
                SKU = viewModel.SKU,
                Description = viewModel.Description,
                CategoryId = viewModel.CategoryId,
                AvailableColors = string.Join(",", allColors),
                AvailableSizes = string.Join(",", allSizes),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsFeatured = false,
                Images = new List<ProductImage>()
            };

            // ** التعديل الرئيسي: معالجة قائمة الصور الجديدة **
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "img", "products");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            foreach (var file in viewModel.ImageFiles)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                product.Images.Add(new ProductImage
                {
                    ImagePath = "/img/products/" + uniqueFileName
                });
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            TempData["Success"] = "The product has been created successfully.";
            return RedirectToAction("Index");
        }
    }
}
