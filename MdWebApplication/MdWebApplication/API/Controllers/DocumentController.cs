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
    public async Task<IActionResult> UploadDocumentAsync([FromForm] string file, [FromForm] string fileName, [FromQuery] Guid userId)
    {
        if (string.IsNullOrEmpty(file))
        {
            return BadRequest("File content cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(fileName))
        {
            return BadRequest("File name cannot be null or empty.");
        }

        try
        {
            var user = await _usersRepository.GetById(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var userName = user.UserName;
            var filename = $"{userName}/{fileName}";
            await _documentService.UploadDocumentAsync(filename, file, user.Id);
            return Ok("File uploaded successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error occurred: {ex.Message}");
        }
    }


    [HttpGet("download")]
    public async Task<IActionResult> DownloadDocumentAsync([FromQuery]Guid userId, [FromQuery]string fileName)
    {
        var user = await _usersRepository.GetById(userId);
        if (user == null)
        {
            return Unauthorized(new { error = "User not found" });
        }

        var filename = $"{user.UserName}/{fileName}";
        var document = await _documentService.DownloadDocument(filename);
        if (document == null)
        {
            return NotFound(new { error = "Document not found" });
        }

        return Ok(document);
    }

    
    // [HttpGet("all")]
    // public async Task<IActionResult> GetAllDocuments(Guid us)
}