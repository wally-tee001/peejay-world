using Microsoft.EntityFrameworkCore;
using peejayworld_mvc.Models;


namespace peejayworld_mvc.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

    public DbSet<NewsletterSubscription> NewsletterSubscriptions => Set<NewsletterSubscription>();

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
    }
}

