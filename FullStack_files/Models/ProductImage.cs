using System.ComponentModel.DataAnnotations;

namespace CoolCompanyEstore.Models
{
    public class ProductImage
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string ImagePath { get; set; }

        // هذا هو المفتاح الخارجي (Foreign Key) الذي يربط الصورة بالمنتج
        public int ProductId { get; set; }

        // خاصية الملاحة (Navigation property)
        public Product Product { get; set; }
    }
}