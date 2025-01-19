using System.Security.Claims;
using Application.Interfaces.Services;
using Core.Models;
using Infrastructure;
using MdWebApplication.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MdWebApplication.Services;

public class UserService : IUsersService
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUsersRepository _usersRepository;
    private readonly IJwtProvider _jwtProvider;

    public UserService(IUsersRepository usersRepository,IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
    {
        _passwordHasher = passwordHasher;
        _usersRepository = usersRepository;
        _jwtProvider = jwtProvider;
    }
    
    public async Task<string> Register(string userName, string login, string password)
    {
        var hashedPassword = _passwordHasher.Generate(password);

        var user = User.Create(Guid.NewGuid(), userName, login, hashedPassword);
        if (await _usersRepository.GetByLoginAsync(login) != null)
        {
            throw new InvalidOperationException("User with this login already exists.");
        } 
        await _usersRepository.Add(user);
        
        return _jwtProvider.GenerateToken(user);
    }

    public async Task<string> Login(string login, string password)
    {
        var user = await _usersRepository.GetByLoginAsync(login);

        var result = _passwordHasher.Verify(password, user.PasswordHash);

        if (result == false)
        {
            throw new Exception("Failed to login");
        }

        var token = _jwtProvider.GenerateToken(user);
        
        return token;
    }

    public async Task<Guid?>GetUserIdFromToken(ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return null; // Возвращаем null, если нет userId или он некорректен
        }

        return userId; // Возвращаем корректный userId
    }

}