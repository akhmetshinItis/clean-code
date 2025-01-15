using Markdown.Classes;

namespace Markdown.Interfaces;

public interface IRenderer
{
    public string Render((List<(Tag, Tag)>, List<Tag>)  tags, string text);
}