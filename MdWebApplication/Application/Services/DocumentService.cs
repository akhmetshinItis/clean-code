using System.Text;
using Core.Models;
using MdWebApplication.Interfaces.Repositories;
using Document = System.Reflection.Metadata.Document;

namespace MdWebApplication.Services;

public class DocumentService
{
    private readonly MinioService _minioService;
    private readonly IDocumentRepository _documentRepository;
    private IUsersRepository _usersRepository;

    public DocumentService(MinioService minioService, IDocumentRepository documentRepository,
        IUsersRepository usersRepository)
    {
        _minioService = minioService;
        _documentRepository = documentRepository;
        _usersRepository = usersRepository;
    }

    public async Task UploadDocumentAsync(string fileName, string userName,string file, bool isSharing, Guid userId)
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
                await _documentRepository.AddDocumentAsync(userId, fileName, fileUrl, isSharing);
            }
        }
    }

    public async Task<string> DownloadDocument(string documentName)
    {
        return await _minioService.GetFileAsync(documentName);
    }

    public Task DeleteDocument(string documentName)
    {
        return _documentRepository.DeleteDocument(documentName);
    }

    public async Task<string> GetDocumentById(Guid id)
    { 
        var document = await _documentRepository.GetDocumentById(id);
        return document.FileName;
    }
    
    public async Task<string> GetOwnersName(Guid id)
    { 
        var document = await _documentRepository.GetDocumentById(id);
        var userName = await _usersRepository.GetById(document.UserId);
        return userName.UserName;
    }

    public async Task<bool> IsDocumentSharing(Guid id)
    {
        var document = await _documentRepository.GetDocumentById(id);
        return document.IsSharing;
    }
}
