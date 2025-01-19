using Core.Models;
using DataAccess.Models;

namespace DataAccess.Mappers;

public class DocumentMapper
{
    public static Document MapToDocumentModel(DocumentEntity documentEntity)
    {
        if (documentEntity == null)
            throw new ArgumentNullException(nameof(documentEntity));

        return new Document
        {
            Id = documentEntity.Id,
            UserId = documentEntity.UserId,
            FileUrl = documentEntity.FileUrl,
            FileName = documentEntity.FileName
        };
    }
}