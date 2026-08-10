using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace peejayworld_mvc.ViewModels;

public class ProductManageViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string ImageUrl { get; set; } = string.Empty;

    public IFormFile? ImageFile { get; set; }

    [Required]
    public decimal Price { get; set; }

    public bool IsFeatured { get; set; }
}
