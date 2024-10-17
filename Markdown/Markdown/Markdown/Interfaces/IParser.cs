using Markdown.Classes;

namespace Markdown.Interfaces;

public interface IParser
{
    public (List<(Tag, Tag)>, List<Tag>) Parse(string text);
}