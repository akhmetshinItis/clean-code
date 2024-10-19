using System.Text;
using Markdown.Enums;
using Markdown.Interfaces;

namespace Markdown.Classes;

public class Renderer : IRenderer
{
    private readonly Dictionary<TagStyle, string> tagsInHtml = new Dictionary<TagStyle, string>()
    {
        { TagStyle.Bold,  "strong"},
        { TagStyle.Italic, "em"},

    };
        
    
    public string Render((List<(Tag, Tag)>, List<Tag>)  tags, string text)
    {
        var tagPairs = tags.Item1;
        var singleTags = tags.Item2;
        var lines = text.Split('\n');
        var tagStartIndexes= new Dictionary<int, Tag>();
        var tagFinishIndexes = new Dictionary<int, Tag>();
        var singleTagsDict = new Dictionary<int, Tag>();
        
        foreach (var tag in singleTags)
        {
            singleTagsDict[tag.Index] = tag;
        }
        
        var isHeader = singleTagsDict.TryGetValue(0, out var header) && header.TagStyle == TagStyle.Header;
        var htmlText = isHeader ?  new StringBuilder().Append("<h1>") : new StringBuilder();

        foreach (var tagPair in tagPairs)
        {
            tagStartIndexes[tagPair.Item1.Index] = tagPair.Item1;
            tagFinishIndexes[tagPair.Item2.Index] = tagPair.Item2;
        }

        for (int i = isHeader ? 1 : 0; i < text.Length; i++)
        {
            if (singleTagsDict.TryGetValue(i, out header) && header.TagStyle == TagStyle.Header)
            {
                htmlText.Append("<h1>");
                isHeader = true;
            }
            
            if (tagStartIndexes.TryGetValue(i, out var tag))
            {
                htmlText.Append($"<{tagsInHtml[tag.TagStyle]}>");
                i += tag.Length - 1;
            }
            else if (tagFinishIndexes.TryGetValue(i, out tag))
            {
                htmlText.Append($"</{tagsInHtml[tag.TagStyle]}>");
                i += tag.Length - 1;
            }
            else if (singleTagsDict.ContainsKey(i) && singleTags[i].TagStyle == TagStyle.EscapeCharacter)
            {
                i += singleTags[i].Length - 1;
            }
            else
            {
                htmlText.Append(text[i]);
            }
            
            if ((text[i] == '\n' || i == text.Length - 1) && isHeader)
            {
                htmlText.Append("</h1>");
            }
            
        }

        return htmlText.ToString();
    }
    
}