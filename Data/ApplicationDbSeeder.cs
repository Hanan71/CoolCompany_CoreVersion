using CoolCompanyEstore.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CoolCompanyEstore.Data
{
    public static class ApplicationDbSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            // تأكد من ترحيل القاعدة
            context.Database.Migrate();

            // حذف الفئات الفارغة
            var invalidCategories = context.Categories
                .Where(c => string.IsNullOrWhiteSpace(c.Name))
                .ToList();

            if (invalidCategories.Any())
            {
                context.Categories.RemoveRange(invalidCategories);
                context.SaveChanges();
            }

            // إضافة الفئات إذا لم تكن موجودة مسبقًا
            var categoryNames = new[] { "Tech Gadgets", "Wearables", "Accessories" };
            foreach (var name in categoryNames)
            {
                if (!context.Categories.Any(c => c.Name == name))
                {
                    context.Categories.Add(new Category { Name = name });
                }
            }
            context.SaveChanges();

            // جلب الفئات المضافة
            var techGadgetsCategory = context.Categories.FirstOrDefault(c => c.Name == "Tech Gadgets");
            var wearablesCategory = context.Categories.FirstOrDefault(c => c.Name == "Wearables");
            var accessoriesCategory = context.Categories.FirstOrDefault(c => c.Name == "Accessories");

            if (techGadgetsCategory == null || wearablesCategory == null || accessoriesCategory == null)
                return;

            // إضافة منتجات إن لم تكن موجودة
            if (!context.Products.Any())
            {
                var products = new List<Product>
                {
                    new Product
                    {
                        Name = "Smart Watch",
                        Description = "A stylish smart watch with health tracking features.",
                        Price = 299.99m,
                        SKU = "SW-001",
                        CategoryId = wearablesCategory.Id,
                        AvailableColors = "Black, Silver",
                        AvailableSizes = "M, L",
                        IsFeatured = true,
                        CreatedAt = System.DateTime.UtcNow,
                        IsActive = true
                    },
                    new Product
                    {
                        Name = "Wireless Earbuds",
                        Description = "Noise-cancelling earbuds with long battery life.",
                        Price = 149.99m,
                        SKU = "EB-002",
                        CategoryId = wearablesCategory.Id,
                        AvailableColors = "White, Black",
                        AvailableSizes = null,
                        CreatedAt = System.DateTime.UtcNow,
                        IsActive = true
                    },
                    new Product
                    {
                        Name = "Portable Charger",
                        Description = "High-capacity power bank for your devices.",
                        Price = 89.00m,
                        SKU = "PC-003",
                        CategoryId = accessoriesCategory.Id,
                        AvailableColors = "Red, Blue, Black",
                        AvailableSizes = null,
                        CreatedAt = System.DateTime.UtcNow,
                        IsActive = true
                    }
                };

                context.Products.AddRange(products);
                context.SaveChanges();

                // ** إضافة الصور للمنتجات بعد حفظها في قاعدة البيانات **
                var smartWatch = context.Products.FirstOrDefault(p => p.SKU == "SW-001");
                if (smartWatch != null)
                {
                    context.ProductImages.Add(new ProductImage { ProductId = smartWatch.Id, ImagePath = "/img/smartwatch.jpg" });
                }

                var earbuds = context.Products.FirstOrDefault(p => p.SKU == "EB-002");
                if (earbuds != null)
                {
                    context.ProductImages.Add(new ProductImage { ProductId = earbuds.Id, ImagePath = "/img/earbuds.jpg" });
                }

                var charger = context.Products.FirstOrDefault(p => p.SKU == "PC-003");
                if (charger != null)
                {
                    context.ProductImages.Add(new ProductImage { ProductId = charger.Id, ImagePath = "/img/charger.jpg" });
                }

                context.SaveChanges();
            }

            // إصلاح المنتجات التي لا تملك فئة صحيحة
            var uncategorizedProducts = context.Products
                .Where(p => !context.Categories.Any(c => c.Id == p.CategoryId) || p.CategoryId == null)
                .ToList();

            if (uncategorizedProducts.Any())
            {
                var uncategorizedCategory = context.Categories.FirstOrDefault(c => c.Name == "Uncategorized");

                if (uncategorizedCategory == null)
                {
                    uncategorizedCategory = new Category { Name = "Uncategorized" };
                    context.Categories.Add(uncategorizedCategory);
                    context.SaveChanges();
                }

                foreach (var product in uncategorizedProducts)
                {
                    product.CategoryId = uncategorizedCategory.Id;
                }

                context.SaveChanges();
            }
        }
    }
}
