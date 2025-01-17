using Core.Models;

namespace MdWebApplication.Interfaces.Repositories;

public interface IDocumentRepository
{
    Task AddDocumentAsync(Guid userId, Guid fileId, string fileUrl);
}
