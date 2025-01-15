using Markdown.Classes;
using NUnit.Framework;

namespace Markdown.Tests
{
    [TestFixture]
    public class MdProcessorTests
    {
        private MdProcessor mdProcessor;

        [SetUp]
        public void Setup()
        {
            mdProcessor = new MdProcessor();
        }

        [Test]
        public void ItalicParsing_WrapWithSingleUnderscores_ShouldRenderAsEm()
        {
            string markdown = "Текст, _окруженный с двух сторон_ одинарными символами подчерка.";
            string expectedHtml = "Текст, <em>окруженный с двух сторон</em> одинарными символами подчерка.";

            string result = mdProcessor.GetHtmlFromMarkdown(markdown);

            Assert.AreEqual(expectedHtml, result);
        }

        [Test]
        public void BoldParsing_WrapWithDoubleUnderscores_ShouldRenderAsStrong()
        {
            string markdown = "__Выделенный двумя символами текст__ должен становиться полужирным.";
            string expectedHtml = "<strong>Выделенный двумя символами текст</strong> должен становиться полужирным.";

            string result = mdProcessor.GetHtmlFromMarkdown(markdown);

            Assert.AreEqual(expectedHtml, result);
        }

        [Test]
        public void EscapeCharacter_ShouldNotApplyTag()
        {
            string markdown = @"\_Вот это\_ не должно выделиться тегом <em>.";
            string expectedHtml = "_Вот это_ не должно выделиться тегом <em>.";

            string result = mdProcessor.GetHtmlFromMarkdown(markdown);

            Assert.AreEqual(expectedHtml, result);
        }

        [Test]
        public void DoubleEscape_ShouldEscapeTheEscapeCharacter()
        {
            string markdown = "\\\\_вот это будет выделено тегом_";
            string expectedHtml = @"\<em>вот это будет выделено тегом</em>";

            string result = mdProcessor.GetHtmlFromMarkdown(markdown);

            Assert.AreEqual(expectedHtml, result);
        }

        [Test]
        public void BoldInsideItalic_ShouldRenderCorrectly()
        {
            string markdown = "Внутри __двойного выделения _одинарное_ тоже__ работает.";
            string expectedHtml = "Внутри <strong>двойного выделения <em>одинарное</em> тоже</strong> работает.";

            string result = mdProcessor.GetHtmlFromMarkdown(markdown);

            Assert.AreEqual(expectedHtml, result);
        }

        [Test]
        public void ItalicInsideBold_ShouldNotRenderCorrectly()
        {
            string markdown = "Внутри _одинарного __двойное__ не_ работает.";
            string expectedHtml = "Внутри <em>одинарного __двойное__ не</em> работает.";

            string result = mdProcessor.GetHtmlFromMarkdown(markdown);

            Assert.AreEqual(expectedHtml, result);
        }

        [Test]
        public void UnderscoreInsideDigits_ShouldNotBeRenderedAsTag()
        {
            string markdown = "Подчерки внутри текста c цифрами_12_3 не считаются выделением.";
            string expectedHtml = "Подчерки внутри текста c цифрами_12_3 не считаются выделением.";

            string result = mdProcessor.GetHtmlFromMarkdown(markdown);

            Assert.AreEqual(expectedHtml, result);
        }

        [Test]
        public void Headers_ShouldRenderAsH1Tag()
        {
            string markdown = "# Заголовок __с _разными_ символами__";
            string expectedHtml = "<h1> Заголовок <strong>с <em>разными</em> символами</strong></h1>";

            string result = mdProcessor.GetHtmlFromMarkdown(markdown);

            Assert.AreEqual(expectedHtml, result);
        }

        [Test]
        public void MultipleBoldAndItalic_ShouldRenderCorrectly()
        {
            string markdown = "Это __полужирный текст, а это _курсив_ и полужирный__.";
            string expectedHtml = "Это <strong>полужирный текст, а это <em>курсив</em> и полужирный</strong>.";

            string result = mdProcessor.GetHtmlFromMarkdown(markdown);

            Assert.AreEqual(expectedHtml, result);
        }

        [Test]
        public void EmptyTags_ShouldNotBeRendered()
        {
            string markdown = "Если внутри подчерков пустая строка ____, то они остаются символами подчерка.";
            string expectedHtml = "Если внутри подчерков пустая строка ____, то они остаются символами подчерка.";

            string result = mdProcessor.GetHtmlFromMarkdown(markdown);

            Assert.AreEqual(expectedHtml, result);
        }
    }
}
