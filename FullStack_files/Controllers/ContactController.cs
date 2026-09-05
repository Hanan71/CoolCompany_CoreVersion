using CoolCompanyEstore.Data;
using CoolCompanyEstore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace CoolCompanyEstore.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: صفحة نموذج التواصل
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/Home/Contact.cshtml");
        }


        // POST: استقبال وإرسال النموذج
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitContact(Contact model)
        {
            if (ModelState.IsValid)
            {
                model.SubmittedAt = System.DateTime.Now;
                _context.Contacts.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Your message has been sent successfully!";
                return RedirectToAction("Index");
            }

            return View("Index", model);
        }

        // GET: عرض قائمة الرسائل (للمشرف أو الأدمن فقط مثلاً)
        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult ViewComplaints()
        {
            var messages = _context.Contacts.OrderByDescending(c => c.SubmittedAt).ToList();
            return View("~/Views/Account/Admin/ViewComplaints.cshtml", messages);
        }
    }
}

