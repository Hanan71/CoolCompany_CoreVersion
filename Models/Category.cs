namespace CoolCompanyEstore.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        // Navigation property: واحدة إلى عدة منتجات
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
