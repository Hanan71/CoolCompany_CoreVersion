using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using CoolCompanyEstore.Data;
using CoolCompanyEstore.Models;


[Authorize(Roles = "SuperAdmin")]
public class PermissionsController : Controller
{
    private readonly string _permissionsFile = "permissions.json";

    public IActionResult Index()
    {
        var savedPermissions = LoadPermissions();

        // الصفحات اللي نبي نظهرها 
        var allPages = new List<string>
{
    "/Admin/SuperDashboard",
    "/Admin/Orders",
    "/Admin/ManageUsers",
    "/Admin/ManageRoles",
    "/CarouselAdmin/Index",
    "/Admin/ManageProducts",
    "/Admin/ViewComplaints",
    "/ContentManager/Dashboard"
};


        var listOfPagePermission = allPages
            .Select(p => new PagePermission
            {
                Page = p,
                Roles = savedPermissions.ContainsKey(p) ? savedPermissions[p] : new List<string>()
            }).ToList();

        return View("ManagePagePermissions", listOfPagePermission);
    }


    [HttpPost]
    public IActionResult UpdatePermissions(Dictionary<string, List<string>> permissions)
    {
        if (permissions == null)
        {
            TempData["Error"] = "لم يتم استلام الصلاحيات.";
            return RedirectToAction("Index");
        }

        PermissionStore.PagePermissions = permissions;
        SavePermissions(permissions);

        TempData["Success"] = "تم تحديث الصلاحيات بنجاح.";
        return RedirectToAction("Index");
    }

    private Dictionary<string, List<string>> LoadPermissions()
    {
        if (!System.IO.File.Exists(_permissionsFile))
            return new Dictionary<string, List<string>>();

        var json = System.IO.File.ReadAllText(_permissionsFile);
        var permissions = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);

        return permissions ?? new Dictionary<string, List<string>>();
    }


    private void SavePermissions(Dictionary<string, List<string>> permissions)
    {
        var json = JsonConvert.SerializeObject(permissions, Formatting.Indented);
        System.IO.File.WriteAllText(_permissionsFile, json);
    }
}
