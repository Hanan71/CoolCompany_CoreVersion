using CoolCompanyEstore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class PermissionAuthorizeAttribute : TypeFilterAttribute
{
    public PermissionAuthorizeAttribute(string permission) : base(typeof(PermissionAuthorizeFilter))
    {
        Arguments = new object[] { permission };
    }
}

public class PermissionAuthorizeFilter : IAsyncAuthorizationFilter
{
    private readonly string _permission;
    private readonly PermissionService _permissionService;

    public PermissionAuthorizeFilter(string permission, PermissionService permissionService)
    {
        _permission = permission;
        _permissionService = permissionService;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var hasPermission = await _permissionService.HasPermissionAsync(context.HttpContext.User, _permission);
        if (!hasPermission)
        {
            context.Result = new ForbidResult();
        }
    }


    public class PermissionAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _page;

        public PermissionAuthorizeAttribute(string page)
        {
            _page = page;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }


        var roles = PermissionStore.PagePermissions.TryGetValue(_page, out var allowedRoles) ? allowedRoles : new List<string>();
            if (!roles.Any(role => user.IsInRole(role)))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}


