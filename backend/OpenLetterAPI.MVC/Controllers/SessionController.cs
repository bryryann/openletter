using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenLetterAPI.MVC.Dtos.Session;
using OpenLetterAPI.MVC.Interfaces;
using OpenLetterAPI.MVC.Models;

namespace OpenLetterAPI.MVC.Controllers;

[Route("session")]
[ApiController]
public class SessionController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokens;

    public SessionController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ITokenService tokens)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == req.Username);
        if (user == null)
            return Unauthorized("Invalid username/password.");

        var signin = await _signInManager.CheckPasswordSignInAsync(user, req.Password!, false);
        if (!signin.Succeeded)
            return Unauthorized("Invalid username/password.");

        return Ok(
            new SuccessfulSessionResponse
            {
                Username = user.UserName!,
                Email = user.Email!,
                Token = _tokens.CreateToken(user)
            });
    }
}
