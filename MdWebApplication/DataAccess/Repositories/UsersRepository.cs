using Core.Models;
using DataAccess.Mappers;
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

    public async Task<User> GetUserWithDocuments(Guid id)
    {
        var entity = await _dbContext.Users
            .AsNoTracking()
            .Include(c => c.Documents)
            .FirstOrDefaultAsync(x => x.Id == id);
        return new User
        {
            Id = entity.Id,
            Login = entity.Login,
            PasswordHash = entity.PasswordHash,
            UserName = entity.UserName,
            Documents = entity.Documents.Select(x => DocumentMapper.MapToDocumentModel(x)).ToList(),
        };
    }
    
    
    // Исправлю позже
    public async Task<User> GetById(Guid id)
    {
        var userEntity = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
        return new User
        {
            Id = id,
            UserName = userEntity.UserName,
            PasswordHash = userEntity.PasswordHash
        };
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

    // Позже исправлю
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

    // Исправлю позже
    public async Task<User?> GetByLoginAsync(string login)
    {
        if (string.IsNullOrWhiteSpace(login))
        {
            throw new ArgumentException("Логин не может быть пустым или содержать только пробелы.", nameof(login));
        }

        var userEntity = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Login == login);

        if (userEntity == null)
        {
            return null;
        }

        return new User
        {
            Id = userEntity.Id,
            Login = userEntity.Login,
            PasswordHash = userEntity.PasswordHash,
            UserName = userEntity.UserName
        };
    }

}