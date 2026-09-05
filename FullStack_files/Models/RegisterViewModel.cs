using System.ComponentModel.DataAnnotations;

namespace CoolCompanyEstore.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "الرجاء إدخال الاسم الكامل.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "الرجاء إدخال البريد الإلكتروني.")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "الرجاء إدخال رقم الهاتف.")]
        [Phone(ErrorMessage = "صيغة رقم الهاتف غير صحيحة.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "الرجاء إدخال كلمة المرور.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "الرجاء تأكيد كلمة المرور.")]
        [Compare("Password", ErrorMessage = "كلمتا المرور غير متطابقتين.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "الرجاء اختيار الدور.")]
        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty; // مثل: SuperAdmin أو ContentManager

        public List<string> Roles { get; set; } = new List<string>();
    }
}
