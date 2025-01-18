using Core.Models;
using DataAccess.Models;
using MdWebApplication.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly AppDbContext _dbContext;

    public DocumentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task AddDocumentAsync(Guid userId, string fileName, string fileUrl)
    {
        var document = new DocumentEntity
        {
            Id = Guid.NewGuid(),
            FileName = fileName,
            UserId = userId,
            FileUrl = fileUrl
        };

        await _dbContext.AddAsync(document);
        await _dbContext.SaveChangesAsync();
    }
}