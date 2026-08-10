using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using peejayworld_mvc.Data;
using peejayworld_mvc.Models;
using peejayworld_mvc.ViewModels;

namespace peejayworld_mvc.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly AppDbContext _db;

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, AppDbContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _db = db;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email,
            EmailConfirmed = true,
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        await _signInManager.SignInAsync(user, isPersistent: false);

        var profile = new UserProfile
        {
            UserId = user.Id,
            FullName = model.FullName,
            UpdatedAtUtc = DateTime.UtcNow
        };

        _db.UserProfiles.Add(profile);
        await _db.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        var profile = await _db.UserProfiles.FindAsync(user.Id);
        if (profile == null)
        {
            profile = new UserProfile
            {
                UserId = user.Id,
                UpdatedAtUtc = DateTime.UtcNow
            };
            _db.UserProfiles.Add(profile);
            await _db.SaveChangesAsync();
        }

        var vm = new UserProfileViewModel
        {
            UserId = profile.UserId,
            Email = user.Email ?? string.Empty,
            FullName = profile.FullName,
            PhoneNumber = profile.PhoneNumber,
            ShippingAddress = profile.ShippingAddress,
            BillingAddress = profile.BillingAddress,
            CardBrand = profile.CardBrand,
            CardLast4 = profile.CardLast4,
            CardExpiry = profile.CardExpiry
        };

        return View(vm);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(UserProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailResult = await _userManager.SetEmailAsync(user, model.Email);
            var userNameResult = await _userManager.SetUserNameAsync(user, model.Email);
            if (!emailResult.Succeeded || !userNameResult.Succeeded)
            {
                foreach (var error in emailResult.Errors.Concat(userNameResult.Errors))
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
        }

        var profile = await _db.UserProfiles.FindAsync(user.Id);
        if (profile == null)
        {
            profile = new UserProfile { UserId = user.Id };
            _db.UserProfiles.Add(profile);
        }

        profile.FullName = model.FullName;
        profile.PhoneNumber = model.PhoneNumber;
        profile.ShippingAddress = model.ShippingAddress;
        profile.BillingAddress = model.BillingAddress;
        profile.CardBrand = model.CardBrand;
        profile.CardLast4 = model.CardLast4;
        profile.CardExpiry = model.CardExpiry;
        profile.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        await _signInManager.RefreshSignInAsync(user);

        ViewBag.ProfileSaved = true;
        return View(model);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Orders()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        var orders = await _db.Orders
            .Where(x => x.UserId == user.Id)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();

        return View(orders);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> OrderDetails(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        var order = await _db.Orders
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == user.Id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
