using Markdown.Enums;

namespace Markdown;

public class Tag
{
    public TagStyle TagStyle;
    public int Length;
    public bool IsPaired;
    public int Index;

    public Tag(TagStyle tagStyle, int length, bool isPaired, int index)
    {
        TagStyle = tagStyle;
        Length = length;
        IsPaired = isPaired;
        Index = index;
    }
}