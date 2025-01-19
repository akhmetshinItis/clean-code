using Core.Models;

namespace MdWebApplication.Interfaces.Repositories;

public interface IUsersRepository
{
    Task Add(User user);
    Task<User> GetByLoginAsync(string login);

    Task<User> GetById(Guid id);

    Task<User> GetUserWithDocuments(Guid id);
}