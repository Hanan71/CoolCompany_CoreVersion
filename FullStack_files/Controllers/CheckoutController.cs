using Microsoft.AspNetCore.Mvc;
using CoolCompanyEstore.Models;
using System.Text.Json;

namespace CoolCompanyEstore.Controllers
{
    public class CheckoutController : Controller
    {
        // GET: عرض صفحة الدفع
        public IActionResult Index()
        {
            return View();
        }

        // POST: استلام بيانات الدفع ومعالجتها
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // حفظ بيانات النموذج في TempData كـ JSON (لأنه لا يمكن حفظ كائنات مباشرة)
            TempData["CheckoutModel"] = JsonSerializer.Serialize(model);

            // إعادة التوجيه إلى أكشن إنشاء الطلب في OrderController
            return RedirectToAction("CreateOrder", "Order");
        }
    }
}
