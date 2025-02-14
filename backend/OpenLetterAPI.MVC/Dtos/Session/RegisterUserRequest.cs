using System.ComponentModel.DataAnnotations;

namespace OpenLetterAPI.MVC.Dtos.Session;

public class RegisterUserRequest
{
    [Required]
    public string? Username { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string? Password { get; set; }
}
