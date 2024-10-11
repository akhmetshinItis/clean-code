using System.Net.Mime;
using System.Text;
using Markdown.Enums;
using Markdown.Interfaces;

namespace Markdown.Classes;

public class Parser : IParser
{
    public List<Token> Parse(string text)
    {
        List<Token> parsedTokens = new List<Token>();
        var allTagsList = ExtractTags(text);
        var tagsWithoutEscapeCharacters = ProcessEscapeCharacters(allTagsList);
        var tagsPairsList = ExtractTagsPairs(tagsWithoutEscapeCharacters, text);
        var singleTags = allTagsList.Select(tag => !tag.IsPaired);
        //проверяем парность - ExtractTagsPairs()
        //проверяем вложенность - ExtractTagsPairs()
        //проверяем корректность вложенности - ExtractTagsPairs()
        //пофиксить _text__text_ ситуацию получаем <em>text__text</em> для _text_text_text_ <em>text</em>text<em>text</em>
        //пофиксил верхнее
        //проверяем экранирование - ProcessEscapeCharacters()
        
        //проверяем пробелы готово нужен рефакторинг
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

    public List<(Tag, Tag)> ExtractTagsPairs(List<Tag> tags, string text)
    {
        var tagStackDict = new Dictionary<TagStyle, Stack<Tag>>();
        var tagPairsList = new List<(Tag, Tag)>();
        foreach (var tag in tags)
        {
            if (!tag.IsPaired) continue;
            if (!tagStackDict.ContainsKey(tag.TagStyle)) tagStackDict[tag.TagStyle] = new Stack<Tag>();
            var tagStack = tagStackDict[tag.TagStyle];
            if (tagStack.Count() > 0)
            {
                if (tagPairsList.Count != 0 && AreTagsIntersecting(tagPairsList.Last(), (tagStack.Peek(), tag)))
                {
                    tagStackDict[tag.TagStyle].Pop();
                    if (text.Length > tag.Index + tag.Length && (text[tag.Index + tag.Length] != ' '
                                                                 && text[tag.Index + tag.Length] != '\n'))
                    {
                        tagStackDict[tag.TagStyle].Push(tag);
                    }
                        
                }
                else if (tagPairsList.Count != 0 && IsBoldInItalic(tagPairsList, tag, tagStack.Peek()))
                {
                    if(text[tag.Index - 1] == ' ' 
                       || text[tag.Index - 1] == '\n') continue;
                    tagPairsList[^1] = (tagStack.Pop(), tag);
                }
                else
                {
                    if (text[tag.Index - 1] != ' ' && text[tag.Index - 1] != '\n')
                    { 
                        tagPairsList.Add((tagStack.Pop(), tag));
                    }
                }
            }

            else
            {
                if(text.Length > tag.Index + tag.Length && (text[tag.Index + tag.Length] != ' ' 
                       && text[tag.Index + tag.Length] != '\n')) tagStack.Push(tag);
            }
        }
        var tagPairsList1 =  SkipTagWhenInDigitSeq(tagPairsList, text); //для проверки, исправлю костыль
        return tagPairsList1;
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
            if (
                tagPair.Item2.Index + tagPair.Item2.Length < text.Length
                && Char.IsDigit(text[tagPair.Item1.Index + 1])
                && Char.IsDigit(text[tagPair.Item2.Index + tagPair.Item2.Length ]))
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


    // private bool IsInWord(Tag openingTag, Tag closingTag, string text)
    //     => openingTag.Index > 0
    //        && closingTag.Index + closingTag.Length < text.Length
    //        && (text[openingTag.Index - 1] != ' '
    //            || text[openingTag.Index - 1] != '\n')
    //        && (text[closingTag.Index + closingTag.Length] != ' '
    //            || text[closingTag.Index + closingTag.Length] != '\n');

}