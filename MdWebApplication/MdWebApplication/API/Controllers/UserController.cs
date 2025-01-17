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
        await _usersService.Register(request.UserName, request.Login, request.Password);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        var token = await _usersService.Login(request.Login, request.Password);
        Response.Cookies.Append("tasty-cookies", token);
        var id = await _usersService.GetUserId(request.Login);
        Response.Cookies.Append("userId", id.ToString());
        return Ok();
    }
}