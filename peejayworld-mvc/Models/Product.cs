using System.ComponentModel.DataAnnotations;

namespace peejayworld_mvc.Models;

public class Product
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required, MaxLength(1000)]
    public string ImageUrl { get; set; } = string.Empty;

    [Required]
    public decimal Price { get; set; }

    public bool IsFeatured { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
