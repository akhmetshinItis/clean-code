using Core.Models;
using DataAccess.Models;
using MdWebApplication.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly AppDbContext _dbContext;

    public UsersRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<UserEntity>> Get()
    {
        return await _dbContext.Users
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .ToListAsync();
    }

    public async Task<List<UserEntity>> GetWithDocuments()
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Include(c => c.Documents)
            .ToListAsync();
    }

    public async Task<UserEntity> GetById(Guid id)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task Add(Guid id, string userName, string login, string passwordHash, List<DocumentEntity> documents)
    {
        var user = new UserEntity
        {
            Id = id,
            UserName = userName,
            Login = login,
            PasswordHash = passwordHash,
            Documents = documents
        };
        await _dbContext.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(Guid id, string userName, string login, string passwordHash, List<DocumentEntity> documents)
    {
        await _dbContext.Users
            .Where(c => c.Id == id)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(u => u.UserName, userName)
                    .SetProperty(u => u.Login, login)
                    .SetProperty(u => u.PasswordHash, passwordHash));
    }

    public async Task Delete(Guid id)
    {
        await _dbContext.Users
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync();
    }

    public async Task Add(User user)
    {
        var userEntity = new UserEntity
        {
            Id = user.Id,
            UserName = user.UserName,
            Login = user.Login,
            PasswordHash = user.PasswordHash
        };
        await _dbContext.Users.AddAsync(userEntity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User> GetByLogin(string login)
    {
        var userEntity = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Login == login) ?? throw new Exception("No such user");
        return new User
        {
            Id = userEntity.Id,
            Login = userEntity.Login,
            PasswordHash = userEntity.PasswordHash,
            UserName = userEntity.UserName
        };
    }
}