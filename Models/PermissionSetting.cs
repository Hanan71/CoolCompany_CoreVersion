namespace CoolCompanyEstore.Models
{
    public class PermissionSetting
    {
        public string Page { get; set; } = "";
        public List<string> Roles { get; set; } = new();
    }
}
