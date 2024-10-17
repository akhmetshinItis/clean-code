// namespace MarkdownTests;
// using Markdown;
// public class Tests
// {
//     private MdProcessor MdProcessor;
//     private string? inputText;
//     private string? expextedOutput;
//
//     [SetUp]
//     public void Setup()
//     {
//         MdProcessor = new MdProcessor();
//         inputText = "";
//         expextedOutput = "";
//     }
//
//     [Test]
//     public void InputText_ShouldNotBeNull()
//     {
//         Assert.NotNull(inputText);
//     }
//
// }

using System.Diagnostics;
using System.Text;
using FluentAssertions;
using Markdown;
using MdProcessor = Markdown.Classes.MdProcessor;

namespace MarkdownTests;

public class Tests
{
    [TestCase("# __d__ _dd_", "<h1> <strong>d</strong> <em>dd</em></h1>")]
    [TestCase("__d__", "<strong>d</strong>")]
    [TestCase("_d_", "<em>d</em>")]
    [TestCase("__d  _fa_ f__", "<strong>d  <em>fa</em> f</strong>")]
    [TestCase("_d  __fa__ f_", "<em>d  __fa__ f</em>")]
    //[TestCase("_a__a_aa", "<em>aa</em>aa")]
    [TestCase("aa_aa_", "aa<em>aa</em>")]
    [TestCase("aa_a_a", "aa<em>a</em>a")]
    [TestCase("a_aa _aa", "a_aa _aa")]
    //[TestCase("a_3_aa aa", "a_3_aa aa")]
    //[TestCase("a_1_", "a_1_")]
    //[TestCase("a_ 1_", "a_ 1_")]
    //[TestCase("_1_", "<em>1</em>")]
    //[TestCase(@"\a_ 1_", @"\a_ 1_")]
    [TestCase ("\\_Вот это\\_", "_Вот это_")]
    [TestCase("\\\\_вот это будет выделено тегом_", "\\\\<em>вот это будет выделено тегом</em>")]
    //[TestCase("#_a \n\n a_\n\na", "<h1>_a </h1>\n\n a_\n\na")]
    //[TestCase("_a \n\n\n a_\n\na", "_a \n\n\n a_\n\na")]
    public void RenderIsCorrect(string input, string expectedOutput)
    {
        new MdProcessor().GetHtmlFromMarkdown(input).Should().Be(expectedOutput);
    }
}