using Markdown.Classes;
using Markdown.Enums;

namespace MarkdownTests;

[TestFixture]
public class ExtractTagsPairsMethodTest
{
    private Parser _parser;

    [SetUp]
    public void SetUp()
    {
        _parser = new Parser();
    }

    [Test]
    public void ExtractTagsPairs_ShouldReturnSingleBoldPair_WhenDoubleUnderscoreIsUsed()
    {
        // Arrange
        var input = "__bold__";

        // Act
        var tags = _parser.ExtractTags(input);
        var result = _parser.ExtractTagsPairs(tags);

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Item1.TagStyle, Is.EqualTo(TagStyle.Bold));
        Assert.That(result[0].Item2.TagStyle, Is.EqualTo(TagStyle.Bold));
        
        // Assert.That(result[0].Item1.Index == 0);
        // Assert.That(result[0].Item2.Index == input.Length - result[0].Item2.Length);
    }

    [Test]
    public void ExtractTagsPairs_ShouldReturnSingleItalicPair_WhenSingleUnderscoreIsUsed()
    {
        // Arrange
        var input = "_italic __bold__ italic_";

        // Act
        var tags = _parser.ExtractTags(input);
        var result = _parser.ExtractTagsPairs(tags);

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        // Assert.That(result[0].Item1.TagStyle, Is.EqualTo(TagStyle.Italic));
        // Assert.That(result[0].Item2.TagStyle, Is.EqualTo(TagStyle.Italic));
        
        // Assert.That(result[0].Item1.Index == 0);
        // Assert.That(result[0].Item2.Index == input.Length - result[0].Item2.Length);
    }

    [Test]
    public void ExtractTagsPairs_ShouldReturnMultipleTags_WhenMultipleTagsArePresent()
    {
        // Arrange
        var input = "__bold__ and _italic_";

        // Act
        var tags = _parser.ExtractTags(input);
        var result = _parser.ExtractTagsPairs(tags);

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].Item1.TagStyle, Is.EqualTo(TagStyle.Bold));
        Assert.That(result[0].Item2.TagStyle, Is.EqualTo(TagStyle.Bold));
        Assert.That(result[1].Item1.TagStyle, Is.EqualTo(TagStyle.Italic));
        Assert.That(result[1].Item2.TagStyle, Is.EqualTo(TagStyle.Italic));

        Assert.That(result[0].Item1.Index == 0);
        Assert.That(result[0].Item2.Index == "__bold__".Length - result[0].Item2.Length);
        Assert.That(result[1].Item1.Index == 13);
        Assert.That(result[1].Item2.Index == 20);
    }
    
    [Test]
    public void ExtractTagsPairs_ShouldReturnInsertedTags_WhenInsertedTagsArePresent()
    {
        // Arrange
        var input = "__bold with _italic_ inside__";

        // Act
        var tags = _parser.ExtractTags(input);
        var result = _parser.ExtractTagsPairs(tags);

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].Item1.TagStyle, Is.EqualTo(TagStyle.Italic));
        Assert.That(result[0].Item2.TagStyle, Is.EqualTo(TagStyle.Italic));
        Assert.That(result[1].Item1.TagStyle, Is.EqualTo(TagStyle.Bold));
        Assert.That(result[1].Item2.TagStyle, Is.EqualTo(TagStyle.Bold));
        
        Assert.That(result[0].Item1.Index == 12); 
        Assert.That(result[0].Item2.Index == 19);
        Assert.That(result[1].Item1.Index == 0);   
        Assert.That(result[1].Item2.Index == 27);
    }
}