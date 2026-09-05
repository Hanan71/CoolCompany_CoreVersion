using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using CoolCompanyEstore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CoolCompanyEstore.ViewModels
{
    public class ProductFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The Name field is required.")]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "The SKU field is required.")]
        [StringLength(50)]
        public string? SKU { get; set; }

        [Required(ErrorMessage = "The Price field is required.")]
        [Range(0.01, 1000000, ErrorMessage = "The field Price must be between 0.01 and 1000000.")]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "The Category field is required.")]
        public int CategoryId { get; set; }

        public string? CustomColor { get; set; } = string.Empty;
        public string? CustomSize { get; set; } = string.Empty;


        [Required(ErrorMessage = "Please select at least one image.")]
        public List<IFormFile>? ImageFiles { get; set; }



        // لقائمة الفئات والألوان والأحجام المتاحة (تعرض في الـ View)
        public List<Category> Categories { get; set; } = new List<Category>();

        public List<string>? AvailableColors { get; set; } = new List<string> { "Red", "Blue", "Green", "Black", "White" };
        public List<string>? AvailableSizes { get; set; } = new List<string> { "S", "M", "L", "XL" };

        public List<string>? SelectedColors { get; set; } = new List<string>();
        public List<string>? SelectedSizes { get; set; } = new List<string>();



        public string DebugMessage { get; set; } = "ViewModel Loaded";


        public string? SelectedCategory { get; set; }

        public List<Product>? Products { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; } = false;


    }
}
