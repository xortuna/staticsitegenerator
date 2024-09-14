using StaticSiteGenerator.Engine;

namespace StaticSiteTests
{
    [TestClass]
    public class TokenizerTests
    {
        [TestMethod]
        public void TestStream()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var sr = new StreamWriter(ms);
                sr.WriteLine("hello world {{token}} how are you");
                sr.Flush();
                ms.Position = 0;

                var result = TemplateTokenizer.ProcessStream(ms);

                Assert.AreEqual(3, result.Count());
            }
        }
 
        [TestMethod]
        public void TestToken()
        {
            string text = "hello world {{token}} how are you";
            var result = TemplateTokenizer.ProcessText(text).ToList();

            Assert.AreEqual(3, result.Count());
            Assert.AreEqual( 0, result[0].Line);
            Assert.AreEqual( TemplateType.Content, result[0].Type);
            Assert.AreEqual("hello world ", result[0].Content);
            Assert.AreEqual( 0, result[1].Line);
            Assert.AreEqual(TemplateType.Token, result[1].Type);
            Assert.AreEqual("token", result[1].Content);
            Assert.AreEqual(0, result[2].Line);
            Assert.AreEqual(TemplateType.Content, result[2].Type);
            Assert.AreEqual(" how are you", result[2].Content);
        }
        [TestMethod]
        public void TestMetadata()
        {
            string text = "{#metadata:one#}{{token}} how are you";
            var result = TemplateTokenizer.ProcessText(text).ToList();

            Assert.AreEqual(3, result.Count());

            Assert.AreEqual(0, result[0].Line);
            Assert.AreEqual(TemplateType.Metadata, result[0].Type);
            Assert.AreEqual("metadata:one", result[0].Content);
            
            Assert.AreEqual(0, result[1].Line);
            Assert.AreEqual(TemplateType.Token, result[1].Type);
            Assert.AreEqual("token", result[1].Content);
            
            Assert.AreEqual(0, result[2].Line);
            Assert.AreEqual(TemplateType.Content, result[2].Type);
            Assert.AreEqual(" how are you", result[2].Content);
        }
        [TestMethod]
        public void TestMultiLine()
        {
            string text = "Hello World\n\rHow are we all today? {{token}}\n\ryeah";
            var result = TemplateTokenizer.ProcessText(text).ToList();

            Assert.AreEqual(3, result.Count());

            Assert.AreEqual(0, result[0].Line);
            Assert.AreEqual(TemplateType.Content, result[0].Type);
            Assert.AreEqual("Hello World\n\rHow are we all today? ", result[0].Content);

            Assert.AreEqual(1, result[1].Line);
            Assert.AreEqual(TemplateType.Token, result[1].Type);
            Assert.AreEqual("token", result[1].Content);

            Assert.AreEqual(2, result[2].Line);
            Assert.AreEqual(TemplateType.Content, result[2].Type);
            Assert.AreEqual("\n\ryeah", result[2].Content);
        }
    }
}