namespace Application.Interfaces.Services;

public interface IUsersService
{
    Task Register(string userName, string login, string password);
    Task<string> Login(string login, string password);

    Task<Guid> GetUserId(string login);
}