using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using peejayworld_mvc.Data;
using peejayworld_mvc.Models;
using peejayworld_mvc.ViewModels;

namespace peejayworld_mvc.Controllers;

public class CartController : Controller
{
    private const string CartSessionKey = "CartItems";
    private readonly AppDbContext _db;

    public CartController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var cart = GetCart();
        var products = await _db.Products
            .Where(x => cart.Select(i => i.ProductId).Contains(x.Id))
            .ToListAsync();

        var items = products.Select(product => new CartItemViewModel
        {
            Product = product,
            Quantity = cart.First(i => i.ProductId == product.Id).Quantity
        }).ToList();

        return View(items);
    }

    [HttpPost]
    public IActionResult Add(int productId)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(x => x.ProductId == productId);
        if (item == null)
        {
            cart.Add(new CartItem { ProductId = productId, Quantity = 1 });
        }
        else
        {
            item.Quantity++;
        }

        SaveCart(cart);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Remove(int productId)
    {
        var cart = GetCart();
        cart.RemoveAll(x => x.ProductId == productId);
        SaveCart(cart);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Update(int productId, int quantity)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(x => x.ProductId == productId);
        if (item != null)
        {
            item.Quantity = Math.Max(quantity, 1);
        }

        SaveCart(cart);
        return RedirectToAction("Index");
    }

    private List<CartItem> GetCart()
    {
        var json = HttpContext.Session.GetString(CartSessionKey);
        if (string.IsNullOrEmpty(json))
        {
            return new List<CartItem>();
        }

        return System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
    }

    private void SaveCart(List<CartItem> cart)
    {
        HttpContext.Session.SetString(CartSessionKey, System.Text.Json.JsonSerializer.Serialize(cart));
    }
}
