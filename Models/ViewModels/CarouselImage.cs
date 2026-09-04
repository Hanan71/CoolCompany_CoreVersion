using System.ComponentModel.DataAnnotations;

namespace CoolCompanyEstore.Models
{
    public class CarouselImage
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Subtitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string ButtonText { get; set; } = string.Empty;
        public string ButtonLink { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public byte[]? ImageData { get; set; }
        public string? ImageContentType { get; set; }

        public string ButtonLabel { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public string ShortParagraph { get; set; } = string.Empty;



    }
}
