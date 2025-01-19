using Application.Interfaces.Services;
using MdWebApplication.API.Contracts.Users;
using MdWebApplication.Services;
using Microsoft.AspNetCore.Mvc;

namespace MdWebApplication.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUsersService _usersService;

    public UserController(IUsersService usersService)
    {
        _usersService = usersService;
    }
    
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        try
        {
            var token = await _usersService.Register(request.Username, request.Login, request.Password);
            Response.Cookies.Append("tasty-cookies", token);
            return Ok();
        }
        catch (Exception e)
        {
            return Conflict(new { error = e.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        var token = await _usersService.Login(request.Login, request.Password);
        Response.Cookies.Append("tasty-cookies", token);
        return Ok();
    }
    
    [HttpPost("logout")]
    public IActionResult LogOut()
    {
        Response.Cookies.Delete("tasty-cookies");
        return Ok();
    }

}