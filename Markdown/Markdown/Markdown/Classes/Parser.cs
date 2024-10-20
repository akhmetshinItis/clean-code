using System.Net.Mime;
using System.Text;
using Markdown.Enums;
using Markdown.Interfaces;

namespace Markdown.Classes;

public class Parser : IParser
{
    private string Text;

    public (List<(Tag, Tag)>, List<Tag>) Parse(string text)
    {
        Text = text;
        List<Token> parsedTokens = new List<Token>();
        var allTagsList = ExtractTags();
        var singleTags = new List<Tag>();
        var tagsWithoutEscapeChars = ProcessEscapeChars(allTagsList, singleTags);
        var tagPairsList = ExtractTagPairs(tagsWithoutEscapeChars);
        var digitProcessedTagPairs = SkipDigitTags(tagPairsList);
        return (digitProcessedTagPairs, singleTags);
    }

    public List<Tag> ExtractTags()
    {
        var extractedTags = new List<Tag>();
        for (int i = 0; i < Text.Length; i++)
        {
            var tag = ExtractTag(Text[i], i);
            if (tag is null)
            {
                continue;
            }

            i += tag.Length - 1;
            extractedTags.Add(tag);
        }

        return extractedTags;
    }

    private Tag ExtractTag(char symbol, int index)
    {
        if (symbol == '_')
        {
            if (index + 1 < Text.Length && Text[index + 1] == '_')
            {
                return new Tag(TagStyle.Bold, 2, true, index);
            }

            return new Tag(TagStyle.Italic, 1, true, index);
        }

        if (symbol == '#' && (index == 0 || Text[index - 1] == '\n'))
        {
            return new Tag(TagStyle.Header, 1, false, index);
        }

        if (symbol == '\\')
        {
            return new Tag(TagStyle.EscapeCharacter, 1, false, index);
        }

        return null;

    }

    public List<(Tag, Tag)> ExtractTagPairs(List<Tag> tags)
    {
        var tagStackDict = new Dictionary<TagStyle, Stack<Tag>>();
        var tagPairsList = new List<(Tag, Tag)>();
        foreach (var tag in tags)
        {
            if (!tag.IsPaired) continue;
            if (!tagStackDict.ContainsKey(tag.TagStyle)) tagStackDict[tag.TagStyle] = new Stack<Tag>();
            var tagStack = tagStackDict[tag.TagStyle];
            if (tagStack.Count > 0)
            {
                if (tag.Index <= tagStack.Peek().Index + tagStack.Peek().Length) //проверка для случая когда между тегами нет символов
                {
                    tagStack.Pop();
                    continue;
                }
                
                if (tagPairsList.Count != 0 && AreTagsIntersecting(tagPairsList.Last(), (tagStack.Peek(), tag)))
                {
                    tagStackDict[tag.TagStyle].Pop();
                    if (Text.Length > tag.Index + tag.Length && (Text[tag.Index + tag.Length] != ' '
                                                                 && Text[tag.Index + tag.Length] != '\n'))
                    {
                        tagStackDict[tag.TagStyle].Push(tag);
                    }
                        
                }
                else if (tagPairsList.Count != 0 && IsBoldInItalic(tagPairsList, tag, tagStack.Peek()))
                {
                    if(Text[tag.Index - 1] == ' ' 
                       || Text[tag.Index - 1] == '\n') continue;
                    tagPairsList[^1] = (tagStack.Pop(), tag);
                }
                else
                {
                    if (Text[tag.Index - 1] != ' ' && Text[tag.Index - 1] != '\n')
                    { 
                        tagPairsList.Add((tagStack.Pop(), tag));
                    }
                }
            }
    
            else
            {
                if(Text.Length > tag.Index + tag.Length && (Text[tag.Index + tag.Length] != ' ' 
                                                            && Text[tag.Index + tag.Length] != '\n')) tagStack.Push(tag);
            }
        }
        return tagPairsList;
    }



    private List<Tag> ProcessEscapeChars(List<Tag> tags, List<Tag> singleTags)
    {
        var result = new List<Tag>();
        for (int i = 0; i < tags.Count; i++)
        {
            if (tags[i].TagStyle == TagStyle.EscapeCharacter
                && i + 1 < tags.Count
                && tags[i + 1].Index == tags[i].Index + 1)
            {
                singleTags.Add(tags[i]);
                i++;
                continue;
            }

            if (tags[i].TagStyle == TagStyle.EscapeCharacter //проверка случая "\\\\" экранирование экранирования
                && Text.Length > tags[i].Index + 1
                && Text[tags[i].Index + 1] == '\\')
            {
                singleTags.Add(tags[i]);
                continue;
            }

            if (!tags[i].IsPaired && tags[i].TagStyle != TagStyle.EscapeCharacter)
            {
                singleTags.Add(tags[i]);
            }

            result.Add(tags[i]);
        }

        return result;
    }

    private List<(Tag, Tag)> SkipDigitTags(List<(Tag, Tag)> tags)
    {
        var tagsListWithValidInDigitTag = new List<(Tag, Tag)>();
        foreach (var tagPair in tags)
        {
            if (
                tagPair.Item2.Index + tagPair.Item2.Length < Text.Length
                && Char.IsDigit(Text[tagPair.Item1.Index + 1])
                && Char.IsDigit(Text[tagPair.Item2.Index + tagPair.Item2.Length]))
            {
                continue;
            }

            tagsListWithValidInDigitTag.Add(tagPair);
        }

        return tagsListWithValidInDigitTag;
    }

    private bool AreTagsIntersecting((Tag, Tag) correctPair, (Tag, Tag) intersectingPair)
    {
        return correctPair.Item2.Index < intersectingPair.Item2.Index
               && correctPair.Item1.Index < intersectingPair.Item1.Index
               && correctPair.Item2.Index > intersectingPair.Item1.Index;
    }

    private bool IsBoldInItalic(List<(Tag, Tag)> tagPairsList, Tag tag, Tag openItalic)
        => tagPairsList[^1].Item1.TagStyle == TagStyle.Bold
           && tag.TagStyle == TagStyle.Italic
           && openItalic.Index < tagPairsList[^1].Item1.Index
           && tag.Index > tagPairsList[^1].Item2.Index;
}