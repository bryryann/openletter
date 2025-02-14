using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using OpenLetterAPI.MVC.Interfaces;
using OpenLetterAPI.MVC.Models;

namespace OpenLetterAPI.MVC.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _sskey;

    public TokenService(IConfiguration config)
    {
        _config = config;
        _sskey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]!));
    }

    public string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.GivenName, user.UserName!)
        };

        var creds = new SigningCredentials(_sskey, SecurityAlgorithms.HmacSha512Signature);

        var tknDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddHours(5),
            SigningCredentials = creds,
            Issuer = _config["JWT:Issuer"],
            Audience = _config["JWT:Audience"]
        };

        var tknHandler = new JwtSecurityTokenHandler();
        var token = tknHandler.CreateToken(tknDescriptor);

        return tknHandler.WriteToken(token);
    }
}
