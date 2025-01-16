using Core.Models;

namespace MdWebApplication.Interfaces.Repositories;

public interface IUsersRepository
{
    Task Add(User user);
    Task<User> GetByLogin(string login);

    Task<User> GetById(Guid id);
}