using System.ComponentModel.DataAnnotations;

namespace peejayworld_mvc.ViewModels;

public class LoginViewModel
{
    [Required, EmailAddress, MaxLength(320)]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
