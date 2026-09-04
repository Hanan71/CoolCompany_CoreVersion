namespace CoolCompanyEstore.Models
{
    public class Wishlist
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public int ProductId { get; set; }

        public Product Product { get; set; } = null!; 

        public ApplicationUser User { get; set; } = null!; 

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

