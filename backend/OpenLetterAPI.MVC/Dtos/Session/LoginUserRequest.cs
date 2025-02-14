using System.ComponentModel.DataAnnotations;

namespace OpenLetterAPI.MVC.Dtos.Session;

public class LoginUserRequest
{
    [Required]
    public string? Username { get; set; }

    [Required]
    public string? Password { get; set; }
}
