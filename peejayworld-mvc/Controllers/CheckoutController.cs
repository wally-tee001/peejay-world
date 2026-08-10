using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using peejayworld_mvc.Data;
using peejayworld_mvc.Models;
using peejayworld_mvc.ViewModels;

namespace peejayworld_mvc.Controllers;

[Authorize]
public class CheckoutController : Controller
{
    private const string CartSessionKey = "CartItems";
    private readonly AppDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public CheckoutController(AppDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var cart = GetCart();
        if (!cart.Any())
        {
            return RedirectToAction("Index", "Cart");
        }

        var products = await _db.Products
            .Where(x => cart.Select(i => i.ProductId).Contains(x.Id))
            .ToListAsync();

        var items = products.Select(product => new CartItemViewModel
        {
            Product = product,
            Quantity = cart.First(i => i.ProductId == product.Id).Quantity
        }).ToList();

        var vm = new CheckoutViewModel
        {
            Items = items,
            OrderTotal = items.Sum(i => i.TotalPrice)
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(CheckoutViewModel model)
    {
        var cart = GetCart();
        if (!cart.Any())
        {
            return RedirectToAction("Index", "Cart");
        }

        var products = await _db.Products
            .Where(x => cart.Select(i => i.ProductId).Contains(x.Id))
            .ToListAsync();

        model.Items = products.Select(product => new CartItemViewModel
        {
            Product = product,
            Quantity = cart.First(i => i.ProductId == product.Id).Quantity
        }).ToList();
        model.OrderTotal = model.Items.Sum(i => i.TotalPrice);

        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var order = new Order
        {
            UserId = _userManager.GetUserId(User),
            FullName = model.FullName,
            Email = model.Email,
            ShippingAddress = model.ShippingAddress,
            Status = "Pending",
            TotalAmount = model.OrderTotal,
            CreatedAtUtc = DateTime.UtcNow,
        };

        foreach (var product in products)
        {
            var quantity = cart.First(i => i.ProductId == product.Id).Quantity;
            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = quantity,
                UnitPrice = product.Price,
                TotalPrice = product.Price * quantity
            });
        }

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        HttpContext.Session.Remove(CartSessionKey);

        return RedirectToAction("Success");
    }

    public IActionResult Success()
    {
        return View();
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
}
