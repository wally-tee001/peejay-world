using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using peejayworld_mvc.Data;
using peejayworld_mvc.Models;

namespace peejayworld_mvc.Controllers;

public class ContactController : Controller
{
    private readonly AppDbContext _db;
    private readonly ILogger<ContactController> _logger;

    public ContactController(AppDbContext db, ILogger<ContactController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(ContactMessage model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        model.CreatedAtUtc = DateTime.UtcNow;

        _db.ContactMessages.Add(model);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Contact message saved: {Email} {Subject}", model.Email, model.Subject);

        ViewBag.Success = true;
        return View("Index");
    }
}

