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
// I will refactor code below later
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
    [TestCase("_a__a_aa", "<em>a__a</em>aa")]
    [TestCase("aa_aa_", "aa<em>aa</em>")]
    [TestCase("aa_a_a", "aa<em>a</em>a")]
    [TestCase("a_aa _aa", "a_aa _aa")]
    [TestCase("_1_", "<em>1</em>")]
    [TestCase("numbers_12_3 ", "numbers_12_3 ")]
    [TestCase("\\a_ 1_", @"\a_ 1_")]
    [TestCase ("\\_This is it\\_", "_This is it_")]
    [TestCase("\\\\_this will be highlighted with a tag_", @"\<em>this will be highlighted with a tag</em>")]
    public void RenderIsCorrect(string input, string expectedOutput)
    {
        new MdProcessor().GetHtmlFromMarkdown(input).Should().Be(expectedOutput);
    }
}