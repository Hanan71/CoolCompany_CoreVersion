using CoolCompanyEstore.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order? Order { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int Quantity { get; set; }

    public string? SelectedColor { get; set; }
    public string? SelectedSize { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    //  خصائص إضافية للعرض (Snapshot)
    public string? ProductName { get; set; }
    public string? ProductSKU { get; set; }
    public string? ProductImagePath { get; set; }
}
