using Core.Models;
using DataAccess.Mappers;
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
    
    public async Task AddDocumentAsync(Guid userId, string fileName, string fileUrl, bool isSharing)
    {
        var document = new DocumentEntity
        {
            Id = Guid.NewGuid(),
            FileName = fileName,
            UserId = userId,
            FileUrl = fileUrl,
            IsSharing = isSharing
        };
        if (await _dbContext.Documents
                .FirstOrDefaultAsync(x => x.FileName == fileName) != null)
        {
            _dbContext.Update(document);
        }
        else
        {
            await _dbContext.AddAsync(document);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteDocument(string documentName)
    {
        var document = await _dbContext.Documents.FirstOrDefaultAsync(d => d.FileName == documentName);
        if (document != null)
        {
            _dbContext.Documents.Remove(document);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<Document> GetDocumentById(Guid id)
    {
        var document = DocumentMapper.MapToDocumentModel(await _dbContext.Documents.FirstOrDefaultAsync(x => x.Id == id));
        return document;
    }
}