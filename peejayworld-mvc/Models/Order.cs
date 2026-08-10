using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace peejayworld_mvc.Models;

public class Order
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public IdentityUser? User { get; set; }

    [Required, MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(320)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(1000)]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required]
    public decimal TotalAmount { get; set; }

    [Required, MaxLength(50)]
    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
