using StaticSiteGenerator.Engine;

namespace StaticSiteTests
{
    [TestClass]
    public class MarkdownTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var result = MarkdownParser.CompileText("## Test");
            Assert.AreEqual("<h2>Test</h2>", result);
        }
    }
}