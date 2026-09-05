using System.Collections.Generic;

namespace CoolCompanyEstore.Models
{
    public class HomeViewModel
    {
        public List<Product> LatestProducts { get; set; } = new();
        public List<Product> FeaturedProducts { get; set; } = new();
        public List<Product> FilteredProducts { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<CarouselImage> CarouselImages { get; set; } = new();
    }
}
