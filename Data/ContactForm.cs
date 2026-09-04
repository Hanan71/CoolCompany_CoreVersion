using System.ComponentModel.DataAnnotations;

namespace CoolCompanyEstore.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = "";

        public DateTime SubmittedAt { get; set; } = DateTime.Now;
    }
}
