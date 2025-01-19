using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;

namespace MdWebApplication.Services;

public class MinioService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;
    private readonly string _endpoint;
    
    public MinioService(IConfiguration configuration)
    {
        _minioClient = new MinioClient()
            .WithEndpoint(configuration["MinIO:Endpoint"])
            .WithCredentials(configuration["MinIO:AccessKey"], configuration["MinIO:SecretKey"])
            .Build();

        _bucketName = configuration["MinIO:BucketName"];
    }

    public async Task<bool> UploadFileAsync(Stream fileStream, string objectName)
    {
        objectName += ".md";
        try
        {
            // Проверяем, существует ли указанный bucket
            bool found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName));
            if (!found)
            {
                // Создаем bucket, если он отсутствует
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName));
            }


            // Загружаем объект в MinIO из потока
            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName)
                .WithStreamData(fileStream) // Передаем поток с данными
                .WithObjectSize(fileStream.Length) // Указываем размер объекта
                .WithContentType("application/octet-stream"));

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error uploading file: {ex.Message}");
            return false;
        }
    }
    
    public async Task<string> GetFileAsync(string objectName)
    {
        objectName += ".md";
        try
        {
            using (var memoryStream = new MemoryStream())
            {
                // Скачиваем файл в поток.
                await _minioClient.GetObjectAsync(new GetObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(objectName)
                    .WithCallbackStream(stream =>
                    {
                        stream.CopyToAsync(memoryStream);
                    }));

                // Конвертируем поток в строку.
                memoryStream.Seek(0, SeekOrigin.Begin);
                using (var streamReader = new StreamReader(memoryStream))
                {
                    string fileContent = await streamReader.ReadToEndAsync();
                    return fileContent;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving file: {ex.Message}");
            throw;
        }
    }

    // Новый метод для генерации URL файла на MinIO
    public string GetFileUrl(string objectName)
    {
        return $"https://{_endpoint}/{_bucketName}/{objectName}";
    }
    
}

