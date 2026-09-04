using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoolCompanyEstore.Controllers
{
    [Authorize(Roles = "ContentManager")]
    public class ContentController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
