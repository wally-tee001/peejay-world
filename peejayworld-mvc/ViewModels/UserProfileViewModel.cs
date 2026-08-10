using System.ComponentModel.DataAnnotations;

namespace peejayworld_mvc.ViewModels;

public class UserProfileViewModel
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Display(Name = "Full Name")]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Email")]
    [Required, EmailAddress, MaxLength(320)]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Phone")]
    [Phone, MaxLength(50)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Display(Name = "Shipping Address")]
    [MaxLength(1000)]
    public string ShippingAddress { get; set; } = string.Empty;

    [Display(Name = "Billing Address")]
    [MaxLength(1000)]
    public string BillingAddress { get; set; } = string.Empty;

    [Display(Name = "Card Brand")]
    [MaxLength(50)]
    public string CardBrand { get; set; } = string.Empty;

    [Display(Name = "Card Last 4")]
    [MaxLength(4), MinLength(4)]
    public string CardLast4 { get; set; } = string.Empty;

    [Display(Name = "Card Expiry")]
    [MaxLength(7)]
    public string CardExpiry { get; set; } = string.Empty;
}
