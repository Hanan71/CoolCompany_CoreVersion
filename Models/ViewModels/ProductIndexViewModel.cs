using CoolCompanyEstore.Models;

namespace CoolCompanyEstore.ViewModels
{
    public class ProductIndexViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string? Description { get; set; }

        public int CategoryId { get; set; }

        public List<Category> Categories { get; set; } = new();

        public List<string> SelectedColors { get; set; } = new();

        public List<string> SelectedSizes { get; set; } = new();

        public string? CustomColor { get; set; }

        public string? CustomSize { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
