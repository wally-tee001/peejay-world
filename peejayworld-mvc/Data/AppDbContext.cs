using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using peejayworld_mvc.Models;


namespace peejayworld_mvc.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

    public DbSet<NewsletterSubscription> NewsletterSubscriptions => Set<NewsletterSubscription>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.ToTable("contact_messages");

            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(320).IsRequired();
            entity.Property(x => x.Subject).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Message).HasMaxLength(2000).IsRequired();

            entity.Property(x => x.CreatedAtUtc).IsRequired();

            entity.HasIndex(x => x.Email);
        });

        modelBuilder.Entity<NewsletterSubscription>(entity =>
        {
            entity.ToTable("newsletter_subscriptions");

            entity.Property(x => x.Email).HasMaxLength(320).IsRequired();
            entity.Property(x => x.SubscribedAtUtc).IsRequired();

            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");
            entity.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(320).IsRequired();
            entity.Property(x => x.ShippingAddress).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.Property(x => x.TotalAmount).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.HasIndex(x => x.UserId);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("order_items");
            entity.Property(x => x.Quantity).IsRequired();
            entity.Property(x => x.UnitPrice).IsRequired();
            entity.Property(x => x.TotalPrice).IsRequired();
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("user_profiles");
            entity.HasKey(x => x.UserId);
            entity.Property(x => x.FullName).HasMaxLength(200);
            entity.Property(x => x.PhoneNumber).HasMaxLength(50);
            entity.Property(x => x.ShippingAddress).HasMaxLength(1000);
            entity.Property(x => x.BillingAddress).HasMaxLength(1000);
            entity.Property(x => x.CardBrand).HasMaxLength(50);
            entity.Property(x => x.CardLast4).HasMaxLength(4);
            entity.Property(x => x.CardExpiry).HasMaxLength(7);
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
            entity.HasOne(x => x.User)
                .WithOne()
                .HasForeignKey<UserProfile>(x => x.UserId);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            entity.Property(x => x.Category).HasMaxLength(100).IsRequired();
            entity.Property(x => x.ImageUrl).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.Price).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.HasIndex(x => x.Category);
        });
    }
}

