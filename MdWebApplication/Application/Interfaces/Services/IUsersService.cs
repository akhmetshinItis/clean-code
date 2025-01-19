using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces.Services;

public interface IUsersService
{
    Task<string> Register(string userName, string login, string password);
    Task<string> Login(string login, string password);
    Task<Guid?> GetUserIdFromToken(ClaimsPrincipal user);

}