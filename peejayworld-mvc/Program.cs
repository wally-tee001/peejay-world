using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using peejayworld_mvc.Data;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog (structured logging)
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// HSTS (HTTP Strict Transport Security) - only sent over HTTPS in non-Development.
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubDomains = true;
    options.Preload = true;
});

// Trust the X-Forwarded-* headers set by reverse proxies (e.g. Render, Nginx, Azure).
// Needed so HTTPS redirection, HSTS, and logging see the real client scheme/host.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Only trust proxies on the loopback / localhost by default. In production where the
    // proxy is external (Render), open up KnownNetworks/KnownProxies as appropriate.
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(GetConnectionString(builder.Configuration)));

// Build the PostgreSQL connection string from config + a runtime-supplied password
// (kept out of source control). Set the password via env var: ConnectionStrings__DefaultPassword
// or PGPASSWORD / DB_PASSWORD.
static string GetConnectionString(IConfiguration configuration)
{
    var cs = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is missing.");

    var password = configuration["ConnectionStrings:DefaultPassword"]
        ?? configuration["PGPASSWORD"]
        ?? configuration["DB_PASSWORD"];

    if (!string.IsNullOrWhiteSpace(password) && !cs.Contains("Password=", StringComparison.OrdinalIgnoreCase))
    {
        cs += $"Password={password};";
    }

    return cs;
}

var app = builder.Build();

// Apply any pending EF Core migrations on startup (safe for Render/containers).
// Wrap in try/catch so a DB that is briefly unavailable doesn't crash the web host.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        await db.Database.MigrateAsync();
        await IdentityDataSeeder.SeedAsync(scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        // Log the failure but keep the app running; migrations can be retried via `dotnet ef database update`.
        app.Logger.LogError(ex, "Failed to apply database migrations on startup.");
    }
}

// Configure the HTTP request pipeline.
app.UseForwardedHeaders();
app.UseStaticFiles();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Security headers (CSP, nosniff, clickjacking, referrer, permissions).
app.Use(async (context, next) =>
{
    var headers = context.Response.Headers;

    headers["X-Content-Type-Options"] = "nosniff";
    headers["X-Frame-Options"] = "DENY";
    headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";

    // CSP: allow self + Google Fonts + Unsplash images + selected remote product assets.
    headers["Content-Security-Policy"] =
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline'; " +
        "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
        "font-src 'self' https://fonts.gstatic.com; " +
        "img-src 'self' data: https://images.unsplash.com https://rukmini1.flixcart.com https://ng.jumia.is https://encrypted-tbn0.gstatic.com https://i5.walmartimages.com; " +
        "connect-src 'self'; " +
        "base-uri 'self'; " +
        "form-action 'self'; " +
        "frame-ancestors 'none'";

    await next();
});

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

