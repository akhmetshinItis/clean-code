using Markdig;
using MdWebApplication.API.Contracts.Md;
using Microsoft.AspNetCore.Mvc;

namespace MdWebApplication.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MdController : ControllerBase
{
    [HttpPost("convert")]
    public IActionResult Convert([FromBody] ConvertRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Markdown))
        {
            return BadRequest(new { error = "Markdown text is required." });
        }

        var html = Markdown.ToHtml(request.Markdown);
        return Ok(new { html });
    }
}