using Markdown.Classes;
using Markdown.Enums;

namespace MarkdownTests;

[TestFixture]
public class TagExtractorTests
{
    private Parser _parser;

    [SetUp]
    public void SetUp()
    {
        _parser = new Parser();
    }

    [Test]
    public void ExtractTags_ShouldReturnBoldTag_WhenDoubleUnderscoreIsFound()
    {
        // Arrange
        var input = "__bold__";

        // Act
        var result = _parser.ExtractTags(input);
        var result2 = _parser.ExtractTagsPairs(result, input);

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].TagStyle, Is.EqualTo(TagStyle.Bold));
        Assert.That(result[0].Length, Is.EqualTo(2));
        
        Assert.That(result2.Count, Is.EqualTo(1));
        
    }

    [Test]
    public void ExtractTags_ShouldReturnItalicTag_WhenSingleUnderscoreIsFound()
    {
        // Arrange
        var input = "_italic_";

        // Act
        var result = _parser.ExtractTags(input);

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].TagStyle, Is.EqualTo(TagStyle.Italic));
        Assert.That(result[0].Length, Is.EqualTo(1));
    }

    [Test]
    public void ExtractTags_ShouldReturnHeaderTag_WhenHashAtStartOfLine()
    {
        // Arrange
        var input = "# Header";

        // Act
        var result = _parser.ExtractTags(input);

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].TagStyle, Is.EqualTo(TagStyle.Header));
        Assert.That(result[0].Length, Is.EqualTo(1));
    }

    [Test]
    public void ExtractTags_ShouldReturnEscapeCharacterTag_WhenBackslashIsFound()
    {
        // Arrange
        var input = "\\ Escape";

        // Act
        var result = _parser.ExtractTags(input);

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].TagStyle, Is.EqualTo(TagStyle.EscapeCharacter));
        Assert.That(result[0].Length, Is.EqualTo(1));
    }

    [Test]
    public void ExtractTags_ShouldReturnMultipleTags_WhenMultipleTagsArePresent()
    {
        // Arrange
        var input = "__bold__ and _italic_ and fake #Header";

        // Act
        var result = _parser.ExtractTags(input);

        // Assert
        Assert.That(result.Count, Is.EqualTo(4));
        Assert.That(result[0].TagStyle, Is.EqualTo(TagStyle.Bold));
        Assert.That(result[2].TagStyle, Is.EqualTo(TagStyle.Italic));
    }
    
    [Test]
    public void ExtractTags_ShouldReturnInsertedTags_WhenInsertedTagsArePresent()
    {
        // Arrange
        var input = "__bold with _italic_ inside__";

        // Act
        var result = _parser.ExtractTags(input);

        // Assert
        Assert.That(result.Count, Is.EqualTo(4));
        Assert.That(result[0].TagStyle, Is.EqualTo(TagStyle.Bold));
        Assert.That(result[1].TagStyle, Is.EqualTo(TagStyle.Italic));
        Assert.That(result[2].TagStyle, Is.EqualTo(TagStyle.Italic));
        Assert.That(result[3].TagStyle, Is.EqualTo(TagStyle.Bold));

    }
    
    
}