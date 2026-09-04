using System.ComponentModel.DataAnnotations.Schema;

namespace CoolCompanyEstore.Models
{
    public class PagePermission
    {
        public int Id { get; set; }

        public string Page { get; set; } = string.Empty;

        public string RolesSerialized { get; set; } = string.Empty;

        public string PageName { get; set; } = string.Empty;

        [NotMapped]
        public List<string> AllowedRoles { get; set; } = new List<string>();

        [NotMapped]
        public List<string> Roles
        {
            get => string.IsNullOrEmpty(RolesSerialized)
                   ? new List<string>()
                   : RolesSerialized.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            set => RolesSerialized = string.Join(",", value);
        }
    }
}
