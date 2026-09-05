using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CoolCompanyEstore.ViewModels;
using CoolCompanyEstore.Models;
using Microsoft.AspNetCore.Mvc;
using CoolCompanyEstore.Data;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;


namespace CoolCompanyEstore.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _environment;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _environment = environment;
        }

        // =============== صفحات عامة ===============

        public IActionResult Dashboard() => View();

        public IActionResult ViewComplaints() =>
            View(_context.Contacts.ToList());

        public async Task<IActionResult> ManageUsers()
        {
            var allUsers = await _userManager.Users
                                 .Where(u => !u.IsDeleted)
                                 .ToListAsync();

            var normalUsers = new List<ApplicationUser>();
            var otherUsers = new List<ApplicationUser>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.Roles = roles.ToList();

                if (roles.Contains("NormalUser"))
                    normalUsers.Add(user);
                else
                    otherUsers.Add(user);
            }

            var model = new UsersViewModel
            {
                NormalUsers = normalUsers,
                OtherUsers = otherUsers
            };

            return View(model);
        }

        public async Task<IActionResult> DeletedUsers()
        {
            var deletedUsers = await _userManager.Users
                                 .Where(u => u.IsDeleted)
                                 .ToListAsync();

            foreach (var user in deletedUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.Roles = roles.ToList();
            }

            return View(deletedUsers);
        }

        public IActionResult SuperDashboard() =>
            View(_context.Orders.ToList());

        // =============== تسجيل الدخول ===============
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Account/Login.cshtml", model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || user.IsDeleted)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View("~/Views/Account/Login.cshtml", model);
            }

            var roles = await _userManager.GetRolesAsync(user);

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View("~/Views/Account/Login.cshtml", model);
            }

            // التوجيه حسب الدور
            if (roles.Contains("SuperAdmin"))
            {
                return RedirectToAction("SuperDashboard", "Admin");
            }
            else if (roles.Contains("ContentManager"))
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            else if (roles.Contains("Accountant"))
            {
                return RedirectToAction("Dashboard", "Content");
            }
            else if (roles.Contains("NormalUser"))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "You do not have access to admin panel.");
                await _signInManager.SignOutAsync();
                return View("~/Views/Account/Login.cshtml", model);
            }
        }

        // =============== إدارة المنتجات ===============
        [Authorize(Roles = "SuperAdmin,ContentManager")]
        public IActionResult ManageProducts()
        {
            // تم التعديل هنا: استخدام Include لجلب الصور مع المنتج
            var products = _context.Products.Include(p => p.Category).Include(p => p.Images).ToList();
            return View(products);
        }

        [Authorize(Roles = "SuperAdmin,ContentManager")]
        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            var viewModel = new ProductFormViewModel
            {
                Categories = await _context.Categories.ToListAsync(),
                AvailableColors = new List<string> { "Red", "Blue", "Green", "Black", "White" },
                AvailableSizes = new List<string> { "S", "M", "L", "XL" },
                SelectedColors = new List<string>(),
                SelectedSizes = new List<string>()
            };

            return View(viewModel);
        }

        [Authorize(Roles = "SuperAdmin,ContentManager")]
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
                Description = viewModel.Description,
                Price = viewModel.Price,
                SKU = viewModel.SKU,
                CategoryId = viewModel.CategoryId,
                AvailableColors = string.Join(",", allColors),
                AvailableSizes = string.Join(",", allSizes),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsFeatured = false,
                Images = new List<ProductImage>()
            };

            // ** التعديل الأساسي: معالجة قائمة الصور الجديدة **
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

            TempData["Success"] = "Product added successfully!";
            return RedirectToAction("ManageProducts");
        }


        [Authorize(Roles = "SuperAdmin,ContentManager")]
        [HttpGet]
        public async Task<IActionResult> EditProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // تم التعديل هنا: جلب الصور مع المنتج
            var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new EditProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                Price = product.Price,
                Description = product.Description,
                CategoryId = product.CategoryId,
                // تم التعديل هنا: تعبئة قائمة مسارات الصور الحالية
                ExistingImagePaths = product.Images?.Select(i => i.ImagePath).ToList() ?? new List<string>(),
                Categories = await _context.Categories.ToListAsync(),
                SelectedColors = product.AvailableColors?.Split(',').ToList() ?? new List<string>(),
                SelectedSizes = product.AvailableSizes?.Split(',').ToList() ?? new List<string>()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(EditProductViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Categories = await _context.Categories.ToListAsync();
                return View(viewModel);
            }

            var existing = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == viewModel.Id);
            if (existing == null)
                return NotFound();

            existing.Name = viewModel.Name;
            existing.SKU = viewModel.SKU;
            existing.Price = viewModel.Price;
            existing.Description = viewModel.Description;
            existing.CategoryId = viewModel.CategoryId;

            var colorsList = viewModel.SelectedColors ?? new List<string>();
            if (!string.IsNullOrWhiteSpace(viewModel.CustomColor))
                colorsList.Add(viewModel.CustomColor.Trim());
            existing.AvailableColors = string.Join(',', colorsList);

            var sizesList = viewModel.SelectedSizes ?? new List<string>();
            if (!string.IsNullOrWhiteSpace(viewModel.CustomSize))
                sizesList.AddRange(viewModel.CustomSize.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            existing.AvailableSizes = string.Join(',', sizesList);


            // ** التعديل الأساسي: معالجة الصور الجديدة **
            if (viewModel.ImageFiles != null && viewModel.ImageFiles.Count > 0)
            {
                // حذف الصور القديمة من مجلد wwwroot
                foreach (var oldImage in existing.Images)
                {
                    var oldFilePath = Path.Combine(_environment.WebRootPath, oldImage.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // حذف الصور القديمة من قاعدة البيانات
                _context.ProductImages.RemoveRange(existing.Images);
                existing.Images = new List<ProductImage>(); // إعداد قائمة جديدة

                // إضافة الصور الجديدة
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

                    existing.Images.Add(new ProductImage
                    {
                        ImagePath = "/img/products/" + uniqueFileName
                    });
                }
            }


            _context.Products.Update(existing);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Product updated successfully!";
            return RedirectToAction("ManageProducts");
        }

        [Authorize(Roles = "SuperAdmin,ContentManager")]
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Product deleted successfully!";
            return RedirectToAction("ManageProducts");
        }

        // =============== إدارة الطلبات ===============
        public IActionResult OrderDetails(int id)
        {
            var order = _context.Orders
              .Include(o => o.Items)
              .ThenInclude(i => i.Product)
              .Include(o => o.User)
              .FirstOrDefault(o => o.Id == id);

            return order == null ? NotFound() : View(order);
        }

        [HttpGet]
        public IActionResult EditOrder(int id)
        {
            var order = _context.Orders.Find(id);
            return order == null ? NotFound() : View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrder(Order model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingOrder = await _context.Orders.FindAsync(model.Id);
            if (existingOrder == null)
                return NotFound();

            existingOrder.Status = model.Status;
            _context.Orders.Update(existingOrder);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Order updated successfully.";
            return RedirectToAction("Orders");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Order deleted successfully.";
            return RedirectToAction("Orders");
        }

        [HttpGet]
        public IActionResult ManageStatus(int id)
        {
            var order = _context.Orders
              .Include(o => o.Items)
              .ThenInclude(i => i.Product)
              .Include(o => o.User)
              .FirstOrDefault(o => o.Id == id);

            return order == null ? NotFound() : View("ManageStatusList", order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageStatus(Order model)
        {
            var order = await _context.Orders.FindAsync(model.Id);
            if (order == null)
                return NotFound();

            if (order.Status == "Delivered")
            {
                TempData["ErrorMessage"] = "Cannot change status after it's marked as Delivered.";
                return RedirectToAction("Orders");
            }

            order.Status = model.Status;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            TempData["InfoMessage"] = "Order status updated successfully.";
            return RedirectToAction("Orders");
        }

        // =============== إدارة الصلاحيات ===============
        [HttpGet]
        public async Task<IActionResult> AssignRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            ViewBag.UserId = user.Id;
            ViewBag.UserName = user.FullName;
            ViewBag.Roles = _roleManager.Roles.ToList();
            ViewBag.UserRoles = await _userManager.GetRolesAsync(user);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string selectedRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!string.IsNullOrEmpty(selectedRole))
                await _userManager.AddToRoleAsync(user, selectedRole);

            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.IsDeleted = true;
            await _userManager.UpdateAsync(user);
            return RedirectToAction("ManageUsers");
        }

        // =============== إدارة الصلاحيات ===============

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                TempData["ErrorMessage"] = "Role not found.";
                return RedirectToAction("ManageRoles");
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Role deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete role.";
            }

            return RedirectToAction("ManageRoles");
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                TempData["ErrorMessage"] = "Role name cannot be empty.";
                return RedirectToAction("ManageRoles");
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (roleExists)
            {
                TempData["ErrorMessage"] = "This role already exists.";
                return RedirectToAction("ManageRoles");
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Role created successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to create role.";
            }

            return RedirectToAction("ManageRoles");
        }

        [HttpGet]
        public IActionResult OrderProducts()
        {
            var orders = _context.Orders
              .Include(o => o.Items)
              .ThenInclude(i => i.Product)
              .ThenInclude(p => p.Images) // أضف هذا السطر
              .ToList();

            return View(orders);
        }

        [HttpGet]
        public IActionResult Orders()
        {
            // جلب كل الطلبات مع البيانات المرتبطة بها
            var orders = _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Images) // جلب الصور
                .ToList();

            return View(orders);
        }


        [HttpGet]
        public IActionResult ManagePagePermissions()
        {
            var model = new List<PagePermission>
            {
                new PagePermission { Page = "/Admin/CreateProduct", Roles = new List<string>{ "SuperAdmin", "ContentManager" } },
                new PagePermission { Page = "/Admin/EditProduct", Roles = new List<string>{ "SuperAdmin" } },
                new PagePermission { Page = "/Orders/MyOrders", Roles = new List<string>{ "NormalUser" } }
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult UpdatePermissions(List<PagePermission> updatedPermissions)
        {
            TempData["Success"] = "Permissions updated successfully!";
            return RedirectToAction("ManagePagePermissions");
        }
    }
}
