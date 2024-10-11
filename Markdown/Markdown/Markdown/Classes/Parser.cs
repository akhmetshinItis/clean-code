using System.Net.Mime;
using System.Text;
using Markdown.Enums;
using Markdown.Interfaces;

namespace Markdown.Classes;

public class Parser : IParser
{
    private Dictionary<string, TagStyle> TagTypes = new Dictionary<string, TagStyle>()
    {
        { "_", TagStyle.Italic },
        { "__", TagStyle.Bold },
        { "#", TagStyle.Header },
        { @"\", TagStyle.EscapeCharacter }

    };

    public List<Token> Parse(string text)
    {
        List<Token> parsedTokens = new List<Token>();
        var allTagsList = ExtractTags(text);
        var tagsWithoutEscapeCharacters = ProcessEscapeCharacters(allTagsList);
        var tagsPairsList = ExtractTagsPairs(tagsWithoutEscapeCharacters);
        //проверяем парность - ExtractTagsPairs()
        //проверяем вложенность - ExtractTagsPairs()
        //проверяем корректность вложенности - ExtractTagsPairs()
        //пофиксить _text__text_ ситуацию получаем <em>text__text</em> для _text_text_text_ <em>text</em>text<em>text</em>
        //пофиксил верхнее
        //проверяем экранирование - ProcessEscapeCharacters()
        
        //проверить внутри слова если да, то пробелы не проверять 
        //проверяем пробелы 
        //проверить __ _ _ __ и _ __ __ _ (второе не должно работать) - готово
        
        //проверяем цифры - SkipTagWhenInDigitSeq()
        //header в список собрать
        
        return parsedTokens;
    }

    public List<Tag> ExtractTags(string text)
    {
        var extractedTags = new List<Tag>();
        for (int i = 0; i < text.Length; i++)
        {
            var tag = ExtractTag(text[i], i, text);
            if (tag is null)
            {
                continue;
            }

            i += tag.Length;
            extractedTags.Add(tag);
        }

        return extractedTags;
    }

    private Tag ExtractTag(char symbol, int index, string text)
    {
        if (symbol == '_')
        {
            if (index + 1 < text.Length && text[index + 1] == '_')
            {
                return new Tag(TagStyle.Bold, 2, true, index);
            }

            return new Tag(TagStyle.Italic, 1, true, index);
        }

        if (symbol == '#' && (index == 0 || text[index - 1] == '\n'))
        {
            return new Tag(TagStyle.Header, 1, false, index);
        }

        if (symbol == '\\')
        {
            return new Tag(TagStyle.EscapeCharacter, 1, false, index);
        }

        return null;

    }

    public List<(Tag, Tag)> ExtractTagsPairs(List<Tag> tags)
    {
        var tagStackDict = new Dictionary<TagStyle, Stack<Tag>>();
        var tagPairsList = new List<(Tag, Tag)>();
        foreach (var tag in tags)
        {
            if (!tag.IsPaired)
            {
                continue;
            }
            if (!tagStackDict.ContainsKey(tag.TagStyle))
            {
                tagStackDict[tag.TagStyle] = new Stack<Tag>();
                
            }
            var tagStack = tagStackDict[tag.TagStyle];
            if (tagStack.Count() > 0)
            {
                if (tagPairsList.Count != 0 && AreTagsIntersecting(tagPairsList.Last(), (tagStack.Peek(), tag)))
                {
                    tagStackDict[tag.TagStyle].Pop();
                    tagStackDict[tag.TagStyle].Push(tag);
                }
                else if (tagPairsList.Count != 0 && IsBoldInItalic(tagPairsList, tag, tagStack.Peek()))
                {
                    tagPairsList[^1] = (tagStack.Pop(), tag);
                }
                else
                {
                    tagPairsList.Add((tagStack.Pop(), tag));
                }
                
            }
            else
            {
                tagStack.Push(tag);
            }
        }

        return tagPairsList;
    }
    
    
    
    private List<Tag> ProcessEscapeCharacters(List<Tag> tags)
    {
        var result = new List<Tag>();
        for (int i = 0; i < tags.Count; i++)
        {
            if (tags[i].TagStyle == TagStyle.EscapeCharacter
                && i + 1 < tags.Count
                && tags[i + 1].Index == i + 1)
            {
                i++;
                continue;
            }
            result.Add(tags[i]);
        }

        return result;
    }

    private List<(Tag, Tag)> SkipTagWhenInDigitSeq(List<(Tag, Tag)> tags, string text) //переделать в прием List<(Tag, Tag)>()
    {
        var tagsListWithValidInDigitTag = new List<(Tag, Tag)>();
        foreach (var tagPair in tags)
        {
            if (tagPair.Item1.Index != 0
                && tagPair.Item2.Index + 1 < text.Length
                && Char.IsDigit(text[tagPair.Item1.Index - 1])
                && Char.IsDigit(text[tagPair.Item2.Index + 1]))
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
    {
        return tagPairsList[^1].Item1.TagStyle == TagStyle.Bold
               && tag.TagStyle == TagStyle.Italic
               && openItalic.Index < tagPairsList[^1].Item1.Index
               && tag.Index > tagPairsList[^1].Item2.Index;
    }
}