using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CoolCompanyEstore.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [Range(0.01, 1000000)]
        public decimal Price { get; set; }

        [Required]
        public string SKU { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        public string? AvailableColors { get; set; }

        public string? AvailableSizes { get; set; }

        // ** التعديل هنا: حذفنا ImagePath واستبدلناها بقائمة صور **
        // public string? ImagePath { get; set; } // هذا السطر أصبح قديماً

        public ICollection<ProductImage>? Images { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsFeatured { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public byte[]? ImageData { get; set; }

        public string? ImageMimeType { get; set; }


    }

}
