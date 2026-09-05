using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CoolCompanyEstore.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        [Required]
        public string FullName { get; set; } = string.Empty;

        // قديم - للحفاظ على التوافق مع كود موجود
        public string ProfileImage { get; set; } = string.Empty;

        // جديد - رابط الصورة بشكل أوضح
        public string ProfileImageUrl { get; set; } = string.Empty;


            public bool IsDeleted { get; set; } = false; // مضاف حديثًا
        [NotMapped]
        public List<string> Roles { get; set; } = new List<string>();

    }

}

