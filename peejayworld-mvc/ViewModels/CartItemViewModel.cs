using peejayworld_mvc.Models;

namespace peejayworld_mvc.ViewModels;

public class CartItemViewModel
{
    public Product? Product { get; set; }
    public int Quantity { get; set; }

    public decimal TotalPrice => Product is null ? 0 : Product.Price * Quantity;
}
