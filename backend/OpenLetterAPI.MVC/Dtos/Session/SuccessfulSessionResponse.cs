namespace OpenLetterAPI.MVC.Dtos.Session;

public class SuccessfulSessionResponse
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Token { get; set; }
}
