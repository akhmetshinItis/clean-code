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

        var html = Markdig.Markdown.ToHtml(request.Markdown);
        // var mdProcessor = new MdProcessor();
        // var html = mdProcessor.GetHtmlFromMarkdown(request.Markdown);
        return Ok(new { html });
    }
}