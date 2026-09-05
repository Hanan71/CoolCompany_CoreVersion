using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CoolCompanyEstore.Models
{
    public class EditProfileViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Profile Image")]
        public IFormFile? ProfileImage { get; set; }  // 

        public string ExistingImageUrl { get; set; } = string.Empty;
    }
}
