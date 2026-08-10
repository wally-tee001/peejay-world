using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using peejayworld_mvc.Data;
using peejayworld_mvc.Models;
using peejayworld_mvc.ViewModels;

namespace peejayworld_mvc.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _environment;

    public AdminController(AppDbContext db, IWebHostEnvironment environment)
    {
        _db = db;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var orders = await _db.Orders
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();

        var now = DateTime.UtcNow;
        var last7Days = now.AddDays(-7);
        var last30Days = now.AddDays(-30);
        var last365Days = now.AddDays(-365);

        var total7 = orders.Where(x => x.CreatedAtUtc >= last7Days).Sum(x => x.TotalAmount);
        var total30 = orders.Where(x => x.CreatedAtUtc >= last30Days).Sum(x => x.TotalAmount);
        var total365 = orders.Where(x => x.CreatedAtUtc >= last365Days).Sum(x => x.TotalAmount);

        var chartLabels = Enumerable.Range(6, 7)
            .Select(offset => now.Date.AddDays(-offset).ToString("MMM d"))
            .ToList();

        var chartValues = Enumerable.Range(6, 7)
            .Select(offset => orders
                .Where(x => x.CreatedAtUtc.Date == now.Date.AddDays(-offset))
                .Sum(x => x.TotalAmount))
            .ToList();

        var viewModel = new AdminDashboardViewModel
        {
            RecentOrders = orders.Take(10),
            TotalSalesLast7Days = total7,
            TotalSalesLast30Days = total30,
            TotalSalesLast365Days = total365,
            ChartLabels = chartLabels,
            ChartValues = chartValues
        };

        return View(viewModel);
    }

    public async Task<IActionResult> OrderDetails(int id)
    {
        var order = await _db.Orders
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateOrderStatus(int id, string status)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        order.Status = status;
        await _db.SaveChangesAsync();
        return RedirectToAction("OrderDetails", new { id = order.Id });
    }

    public async Task<IActionResult> Products()
    {
        var products = await _db.Products.OrderBy(x => x.Name).ToListAsync();
        return View(products);
    }

    public IActionResult CreateProduct()
    {
        return View(new ProductManageViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProduct(ProductManageViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var imageUrl = await SaveImageFileAsync(model.ImageFile);
        var product = new Product
        {
            Name = model.Name,
            Description = model.Description,
            Category = model.Category,
            Price = model.Price,
            IsFeatured = model.IsFeatured,
            CreatedAtUtc = DateTime.UtcNow,
            ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? model.ImageUrl : imageUrl
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return RedirectToAction("Products");
    }

    public async Task<IActionResult> EditProduct(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        return View(new ProductManageViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            ImageUrl = product.ImageUrl,
            Price = product.Price,
            IsFeatured = product.IsFeatured
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(ProductManageViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var product = await _db.Products.FindAsync(model.Id);
        if (product == null)
        {
            return NotFound();
        }

        var imageUrl = await SaveImageFileAsync(model.ImageFile);
        product.Name = model.Name;
        product.Description = model.Description;
        product.Category = model.Category;
        product.Price = model.Price;
        product.IsFeatured = model.IsFeatured;
        if (!string.IsNullOrWhiteSpace(imageUrl))
        {
            product.ImageUrl = imageUrl;
        }
        else if (!string.IsNullOrWhiteSpace(model.ImageUrl))
        {
            product.ImageUrl = model.ImageUrl;
        }

        _db.Products.Update(product);
        await _db.SaveChangesAsync();
        return RedirectToAction("Products");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        return RedirectToAction("Products");
    }

    private async Task<string?> SaveImageFileAsync(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return null;
        }

        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "products");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using var stream = System.IO.File.Create(filePath);
        await file.CopyToAsync(stream);

        return $"/images/products/{fileName}";
    }
}
