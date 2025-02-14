using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenLetterAPI.MVC.Dtos.Session;
using OpenLetterAPI.MVC.Interfaces;
using OpenLetterAPI.MVC.Models;

namespace OpenLetterAPI.MVC.Controllers;

[Route("session")]
[ApiController]
public class SessionController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokens;

    public SessionController(UserManager<User> userManager, ITokenService tokens)
    {
        _userManager = userManager;
        _tokens = tokens;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var user = new User
            {
                UserName = req.Username,
                Email = req.Email
            };

            var createUser = await _userManager.CreateAsync(user, req.Password!);
            if (!createUser.Succeeded)
                return StatusCode(500, createUser.Errors);

            var roleAttribution = await _userManager.AddToRoleAsync(user, "User");
            if (!roleAttribution.Succeeded)
                return StatusCode(500, roleAttribution.Errors);

            return Ok(
                new SuccessfulSessionResponse
                {
                    Username = user.UserName!,
                    Email = user.Email!,
                    Token = _tokens.CreateToken(user)
                });
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
    }
}
