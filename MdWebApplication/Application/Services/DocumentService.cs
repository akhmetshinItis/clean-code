using System.Text;
using Core.Models;
using MdWebApplication.Interfaces.Repositories;

namespace MdWebApplication.Services;

public class DocumentService
{
    private readonly MinioService _minioService;
    private readonly IDocumentRepository _documentRepository;

    public DocumentService(MinioService minioService, IDocumentRepository documentRepository)
    {
        _minioService = minioService;
        _documentRepository = documentRepository;
    }

    public async Task UploadDocumentAsync(string fileName, string userName,string file, Guid userId)
    {
        if (string.IsNullOrEmpty(file))
        {
            throw new ArgumentException("File content cannot be null or empty", nameof(file));
        }

        var fileBytes = Encoding.UTF8.GetBytes(file);
    
        // Используем MemoryStream для передачи данных напрямую
        using (var memoryStream = new MemoryStream(fileBytes))
        {
            var minioFileName = $"{userName}/{fileName}";
            // Загружаем файл на MinIO
            var success = await _minioService.UploadFileAsync(memoryStream, minioFileName);

            if (success)
            {
                // Генерация URL MinIO для документа
                var fileUrl = _minioService.GetFileUrl(minioFileName);
                // Сохраняем информацию о документе в базе данных через репозиторий
                await _documentRepository.AddDocumentAsync(userId, fileName, fileUrl);
            }
        }
    }

    public Task<string> DownloadDocument(string fileName)
    {
        return _minioService.GetFileAsync(fileName);
    }
    
}
