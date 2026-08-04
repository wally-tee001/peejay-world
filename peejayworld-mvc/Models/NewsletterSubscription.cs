using System.ComponentModel.DataAnnotations;

namespace peejayworld_mvc.Models;

public class NewsletterSubscription
{
    public int Id { get; set; }

    [Required, EmailAddress, MaxLength(320)]
    public string Email { get; set; } = string.Empty;

    public DateTime SubscribedAtUtc { get; set; }
}

