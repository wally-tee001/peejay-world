using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using peejayworld_mvc.Data;
using peejayworld_mvc.Models;

namespace peejayworld_mvc.Controllers;

public class ProductsController : Controller
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _db.Products
            .OrderByDescending(x => x.IsFeatured)
            .ThenBy(x => x.Name)
            .ToListAsync();

        return View(products);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }
}
