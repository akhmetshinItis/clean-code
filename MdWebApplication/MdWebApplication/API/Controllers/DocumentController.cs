using DataAccess.Repositories;
using MdWebApplication.Interfaces.Repositories;
using MdWebApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MdWebApplication.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DocumentController : ControllerBase
{
    private readonly DocumentService _documentService;
    private readonly IUsersRepository _usersRepository;

    
    public DocumentController(DocumentService documentService, IUsersRepository usersRepository)
    {
        _documentService = documentService;
        _usersRepository = usersRepository;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadDocumentAsync([FromForm] string file, [FromQuery] Guid userId)
    {
        if (string.IsNullOrEmpty(file))
        {
            return BadRequest("File content cannot be null or empty.");
        }

        try
        {
            var user = await _usersRepository.GetById(userId);
            var userName = user.UserName;
            var fileId = Guid.NewGuid();
            var filename = $"{userName}/{fileId}";
            await _documentService.UploadDocumentAsync(filename, file, user.Id, fileId);
            return Ok("File uploaded successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error occurred: {ex.Message}");
        }
    }

    [HttpGet("download")]
    public async Task<IActionResult> DownloadDocumentAsync(Guid userId, Guid fileId)
    {
        var user = await _usersRepository.GetById(userId);

        var filename = $"{user.UserName}/{fileId}";
        var document = await _documentService.DownloadDocument(filename);
        return Ok(document);
    }
}