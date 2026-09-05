using System;
using System.ComponentModel.DataAnnotations;

namespace CoolCompanyEstore.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Shipping Address")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Processing";

        public decimal TotalAmount { get; set; } // ← تحسبه من مجموع cart items

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        public ApplicationUser User { get; set; } = null!;


    }
}
