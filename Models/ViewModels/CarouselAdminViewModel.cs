// Models/CarouselAdminViewModel.cs
using System.Collections.Generic;

namespace CoolCompanyEstore.Models
{
    public class CarouselAdminViewModel
    {
        public CarouselImage NewSlide { get; set; } = new CarouselImage();  // السلايد الجديد اللي بنضيفه
        public List<CarouselImage> Slides { get; set; } = new List<CarouselImage>();  // كل السلايدات القديمة
    }
}
