using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MdWebApplication.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DocumentController : ControllerBase
{
    [HttpGet("documents")]
    public IActionResult GetAll()
    {
        return Ok();
    }
}