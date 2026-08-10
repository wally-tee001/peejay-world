using System.ComponentModel.DataAnnotations;
using peejayworld_mvc.Models;
using System.Collections.Generic;

namespace peejayworld_mvc.ViewModels;

public class CheckoutViewModel
{
    [Required, MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(320)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(1000)]
    public string ShippingAddress { get; set; } = string.Empty;

    public IEnumerable<CartItemViewModel> Items { get; set; } = Enumerable.Empty<CartItemViewModel>();
    public decimal OrderTotal { get; set; }
}
