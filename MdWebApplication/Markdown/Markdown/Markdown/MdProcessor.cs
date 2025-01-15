namespace Markdown.Classes;

public class MdProcessor
{
    public string GetHtmlFromMarkdown(string text)
    {
        var allTagsFromText = new Parser().Parse(text);
        return new Renderer().Render(allTagsFromText, text);
    }

    public static void Main()
    {
        
    }
}
