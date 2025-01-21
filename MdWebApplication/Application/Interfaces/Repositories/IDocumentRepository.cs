using Core.Models;

namespace MdWebApplication.Interfaces.Repositories;

public interface IDocumentRepository
{
    Task AddDocumentAsync(Guid userId, string fileName, string fileUrl);
    Task DeleteDocument(string documentName);
    Task<Document> GetDocumentById(Guid id);
}
