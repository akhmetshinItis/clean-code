using Application.Interfaces.Services;
using Core.Models;
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
    private readonly IUsersService _usersService;

    
    public DocumentController(DocumentService documentService, IUsersService usersService)
    {
        _documentService = documentService;
        _usersService = usersService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadDocumentAsync([FromForm] string file,
        [FromForm] string fileName, [FromForm] bool isSharing)
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
            var userId = await _usersService.GetUserIdFromToken(User);
            if (userId is null)
            {
                return Unauthorized(new { error = "Incorrect token" });
            }
            
            var user = await _usersService.GetUserById((Guid)userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var userName = user.UserName;
            await _documentService.UploadDocumentAsync(fileName, userName, file, isSharing, user.Id);
            return Ok("File uploaded successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error occurred: {ex.Message}");
        }
    }


    [HttpGet("download")]
    public async Task<IActionResult> DownloadDocumentAsync([FromQuery]string fileName)
    {
        var userId = await _usersService.GetUserIdFromToken(User);
        if (userId is null)
        {
            return Unauthorized(new { error = "Incorrect token" });
        }
        var user = await _usersService.GetUserById((Guid)userId);
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
    
    
    [HttpGet("load")]
    public async Task<IActionResult> DownloadDocumentAsync([FromQuery]Guid id)
    {
        var userId = await _usersService.GetUserIdFromToken(User);
        if (userId is null)
        {
            return Unauthorized(new { error = "Incorrect token" });
        }
        var user = await _usersService.GetUserById((Guid)userId);
        if (user == null)
        {
            return Unauthorized(new { error = "User not found" });
        }

        var documentName = await _documentService.GetDocumentById(id);
        var documentContent = await _documentService.DownloadDocument($"{user.UserName}/{documentName}");
        if (documentName == null)
        {
            return NotFound(new { error = "Document not found" });
        }

        return Ok(documentContent);
    }
    
    
    [HttpGet("shared")]
    public async Task<IActionResult> DownloadSharedDocumentAsync([FromQuery]Guid id)
    {
        Console.WriteLine("loading shared document");
        var documentName = await _documentService.GetDocumentById(id);
        var userName = await _documentService.GetOwnersName(id);
        var documentContent = await _documentService.DownloadDocument($"{userName}/{documentName}");
        if (documentName == null)
        {
            return NotFound(new { error = "Document not found" });
        }
        Console.WriteLine(!await _documentService.IsDocumentSharing(id));
        if (!await _documentService.IsDocumentSharing(id))
        {
            return NotFound(new { error = "Document sharing is not available" }); 
        }

        return Ok(documentContent);
    }


    [HttpGet("all")]
    public async Task<IActionResult> GetAllDocuments()
    {
        var userId = await _usersService.GetUserIdFromToken(User);
        if (userId is null)
        {
            return Unauthorized(new { error = "Incorrect token" });
        }
        
        var user = await _usersService.GetUserWithDocuments((Guid)userId);
        if (user == null)
        {
            return Unauthorized(new { error = "User not found" });
        }

        var documents = user.Documents.Select(doc => new DocumentVm
        {
            Id = doc.Id,
            DocumentName = doc.FileName
        });

        return Ok(documents);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteDocument(string documentName)
    {
        await _documentService.DeleteDocument(documentName);
        return Ok();
    }
    
}