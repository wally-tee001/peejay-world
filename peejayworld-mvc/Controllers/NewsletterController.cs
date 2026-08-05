using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using peejayworld_mvc.Data;
using peejayworld_mvc.Models;

namespace peejayworld_mvc.Controllers;

public class NewsletterController : Controller
{
    private readonly AppDbContext _db;
    private readonly ILogger<NewsletterController> _logger;

    public NewsletterController(AppDbContext db, ILogger<NewsletterController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Subscribe(NewsletterSubscription model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Index", "Home");
        }

        // Avoid duplicate subscriptions.
        var exists = await _db.NewsletterSubscriptions
            .AnyAsync(x => x.Email == model.Email);

        if (!exists)
        {
            model.SubscribedAtUtc = DateTime.UtcNow;
            _db.NewsletterSubscriptions.Add(model);
            await _db.SaveChangesAsync();
        }

        _logger.LogInformation("Newsletter subscription saved: {Email}", model.Email);

        // PRG pattern: redirect so refresh does not re-POST.
        return RedirectToAction("Index", "Home", new { subscribed = true });
    }
}

