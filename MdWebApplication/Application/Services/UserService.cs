using Application.Interfaces.Services;
using Core.Models;
using Infrastructure;
using MdWebApplication.Interfaces.Repositories;

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
    
    public async Task Register(string userName, string login, string password)
    {
        var hashedPassword = _passwordHasher.Generate(password);

        var user = User.Create(Guid.NewGuid(), userName, login, hashedPassword);

        await _usersRepository.Add(user);
    }

    public async Task<string> Login(string login, string password)
    {
        var user = await _usersRepository.GetByLogin(login);

        var result = _passwordHasher.Verify(password, user.PasswordHash);

        if (result == false)
        {
            throw new Exception("Failed to login");
        }

        var token = _jwtProvider.GenerateToken(user);
        
        return token;
    }
}