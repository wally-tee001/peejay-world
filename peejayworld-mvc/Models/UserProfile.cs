using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace peejayworld_mvc.Models;

public class UserProfile
{
    [Key]
    [MaxLength(450)]
    public string UserId { get; set; } = string.Empty;

    public IdentityUser? User { get; set; }

    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string ShippingAddress { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string BillingAddress { get; set; } = string.Empty;

    [MaxLength(50)]
    public string CardBrand { get; set; } = string.Empty;

    [MaxLength(4)]
    public string CardLast4 { get; set; } = string.Empty;

    [MaxLength(7)]
    public string CardExpiry { get; set; } = string.Empty;

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
