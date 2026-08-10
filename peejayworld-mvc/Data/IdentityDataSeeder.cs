using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace peejayworld_mvc.Data;

public static class IdentityDataSeeder
{
    private const string AdminEmail = "admin@peejayworld.com";
    private const string AdminPassword = "Admin@1234";
    private const string AdminRole = "Admin";

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (!await roleManager.RoleExistsAsync(AdminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(AdminRole));
        }

        var adminUser = await userManager.FindByEmailAsync(AdminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = AdminEmail,
                Email = AdminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, AdminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, AdminRole);
            }
        }
        else if (!await userManager.IsInRoleAsync(adminUser, AdminRole))
        {
            await userManager.AddToRoleAsync(adminUser, AdminRole);
        }

        if (!db.Products.Any())
        {
            db.Products.AddRange(
                new Models.Product
                {
                    Name = "Men's Silver Chronograph",
                    Description = "Sleek office-ready chronograph with stainless steel finish and precision quartz movement.",
                    Category = "Watches",
                    ImageUrl = "https://rukmini1.flixcart.com/image/300/300/xif0q/watch/2/n/w/1-a-01-silver-watchions-men-original-imahmhzx5p89n3wt.jpeg",
                    Price = 129.99m,
                    IsFeatured = true,
                    CreatedAtUtc = DateTime.UtcNow,
                },
                new Models.Product
                {
                    Name = "Classic Leather Dress Watch",
                    Description = "Minimal black dial with genuine leather strap, made for polished office styling.",
                    Category = "Watches",
                    ImageUrl = "https://ng.jumia.is/unsafe/fit-in/300x300/filters:fill(white)/product/80/0145814/1.jpg?5982",
                    Price = 99.95m,
                    IsFeatured = true,
                    CreatedAtUtc = DateTime.UtcNow,
                },
                new Models.Product
                {
                    Name = "Women's Rose Gold Case Watch",
                    Description = "Refined rose gold silhouette with a modern mesh band for a chic workplace look.",
                    Category = "Watches",
                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSIFTGr56XuM-3vCUeLPVatiUritql9rjS1-ZUAkPGRbv9WrUK4wPHthHM&s=10",
                    Price = 139.50m,
                    IsFeatured = true,
                    CreatedAtUtc = DateTime.UtcNow,
                },
                new Models.Product
                {
                    Name = "Luxury Military Leather Watch",
                    Description = "Bold dial, luminous hands, and waterproof construction for every busy professional.",
                    Category = "Watches",
                    ImageUrl = "https://i5.walmartimages.com/seo/POEDAGAR-Fashion-Date-Quartz-Men-Watches-Top-Brand-Luxury-Waterproof-Luminous-Man-Clock-Military-Leather-Sport-Mens-Wrist-Watch_20c71273-e2e2-4e56-a05c-83037540df9e.ae83db4f779f472bd102d60fa6170914.jpeg",
                    Price = 149.00m,
                    IsFeatured = true,
                    CreatedAtUtc = DateTime.UtcNow,
                });

            await db.SaveChangesAsync();
        }
    }
}
