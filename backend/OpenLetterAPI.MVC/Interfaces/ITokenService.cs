using OpenLetterAPI.MVC.Models;

namespace OpenLetterAPI.MVC.Interfaces;

public interface ITokenService
{
    string CreateToken(User user);
}
