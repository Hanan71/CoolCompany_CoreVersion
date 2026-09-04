using Microsoft.AspNetCore.Mvc;

namespace CoolCompanyEstore.Controllers
{
    [PermissionAuthorize("ViewDashboard")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
