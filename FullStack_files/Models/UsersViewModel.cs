using System.Collections.Generic;
using CoolCompanyEstore.Models;

namespace CoolCompanyEstore.ViewModels
{
    public class UsersViewModel
    {
        public List<ApplicationUser> NormalUsers { get; set; } = new List<ApplicationUser>();
        public List<ApplicationUser> OtherUsers { get; set; } = new List<ApplicationUser>();
    }
}
