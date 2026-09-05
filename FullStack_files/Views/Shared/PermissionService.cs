using CoolCompanyEstore.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using CoolCompanyEstore.Models; // لو ApplicationUser هنا

public class PermissionService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager; // بدل IdentityUser

    public PermissionService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<bool> HasPermissionAsync(ClaimsPrincipal user, string permissionName)
    {
        var appUser = await _userManager.GetUserAsync(user);
        var roles = await _userManager.GetRolesAsync(appUser);

        var permissions = await _context.RolePermissions
            .Include(rp => rp.Permission)
            .Include(rp => rp.Role)
            .Where(rp => roles.Contains(rp.Role.Name))
            .Select(rp => rp.Permission.Name)
            .ToListAsync();

        return permissions.Contains(permissionName);
    }
}
