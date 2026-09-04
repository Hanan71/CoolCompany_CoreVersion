namespace CoolCompanyEstore.Data
{
    public static class PermissionStore
    {
        // ممكن تستبدلها لاحقاً بقاعدة بيانات
        public static Dictionary<string, List<string>> PagePermissions { get; set; } = new()
        {
            { "/Dashboard", new List<string> { "SuperAdmin", "ContentManager" } },
            { "/Products", new List<string> { "ContentManager" } },
        };

    }
}
