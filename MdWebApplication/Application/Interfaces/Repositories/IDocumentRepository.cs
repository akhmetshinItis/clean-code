using Core.Models;

namespace MdWebApplication.Interfaces.Repositories;

public interface IDocumentRepository
{
    Task AddDocumentAsync(Guid userId, string fileName, string fileUrl, bool isSharing);
    Task DeleteDocument(string documentName);
    Task<Document> GetDocumentById(Guid id);
}
