using StaticSiteGenerator.Engine;

namespace StaticSiteTests
{
    [TestClass]
    public class TokenParserTests
    {

        [TestMethod]
        public void LinqOnString()
        {
            string text = "foreach('1,2,3,4,5'.take(3),var('foreach.key'))";
            var result = TokenParser.Compile(text);

            var stack = new StaticSiteGenerator.DictionaryStack() { };
            stack.Push();
            Assert.AreEqual("123", result.Execute(stack));
            stack.Pop();
        }

        [TestMethod]
        public void CodeblockParsing()
        {
            string text = "if(equal('a','a'),'hello');print('world')";
            var result = TokenParser.CompileCodeBlock(text);
            Assert.AreEqual("hello world", string.Join(" ", result.Select(r => r.Execute(new StaticSiteGenerator.DictionaryStack()))));
        }


        [TestMethod]
        public void CodeblockParsingNewLine()
        {
            string text = "if(equal('a','a'),'hello');\nprint('world')";
            var result = TokenParser.CompileCodeBlock(text);
            Assert.AreEqual("hello world", string.Join(" ", result.Select(r => r.Execute(new StaticSiteGenerator.DictionaryStack()))));
        }

        [TestMethod]
        public void CodeblockParsingSpaces()
        {
            string text = "   if(equal('a','a'),'hello');\n\r  print('world')";
            var result = TokenParser.CompileCodeBlock(text);
            Assert.AreEqual("hello world", string.Join(" ", result.Select(r => r.Execute(new StaticSiteGenerator.DictionaryStack()))));
        }

        [TestMethod]
        public void CodeblockParsingEndingFailures()
        {
            string text = "   if(equal('a','a'),'hello');\n\r  print('world') \r\n";
            var result = TokenParser.CompileCodeBlock(text);
            Assert.AreEqual("hello world", string.Join(" ", result.Select(r => r.Execute(new StaticSiteGenerator.DictionaryStack()))));
        }
        [TestMethod]
        public void CodeblockParsingEndingFailures2()
        {
            string text = "   if(equal('a','a'),'hello');\n\r  print('world'); \r\n";
            var result = TokenParser.CompileCodeBlock(text);
            Assert.AreEqual("hello world", string.Join(" ", result.Select(r => r.Execute(new StaticSiteGenerator.DictionaryStack()))));
        }


        [TestMethod]
        public void BasicSyntax()
        {
            string text = "if(equal('a','a'),'hello')";
            var result = TokenParser.CompileV2(text, out int parsedLength);
            Assert.AreEqual("hello", result.Execute(new StaticSiteGenerator.DictionaryStack()));
        }
        [TestMethod]
        public void LinqOnFunction()
        {
            string text = "var('a').Reverse()";
            var result = TokenParser.CompileV2(text, out int parsedLength);
            var stack = new StaticSiteGenerator.DictionaryStack() { };
            stack.Push();
            stack.Add("a", "hello");
            Assert.AreEqual("olleh", result.Execute(stack));
            stack.Pop();
        }
        [TestMethod]
        public void InlineOperators()
        {
            string text = "1+2";
            var result = TokenParser.CompileV2(text, out int parsedLength);
            var stack = new StaticSiteGenerator.DictionaryStack() { };
            stack.Push();
            Assert.AreEqual("3", result.Execute(stack));
            stack.Pop();
        }
        [TestMethod]
        public void BracketOperator()
        {
            string text = "(1)";
            var result = TokenParser.CompileV2(text, out int parsedLength);
            var stack = new StaticSiteGenerator.DictionaryStack() { };
            stack.Push();
            Assert.AreEqual("1", result.Execute(stack));
            stack.Pop();
        }
        [TestMethod]
        public void Bracket2Operator()
        {
            string text = "(3-(1+1))";
            var result = TokenParser.CompileV2(text, out int parsedLength);
            var stack = new StaticSiteGenerator.DictionaryStack() { };
            stack.Push();
            Assert.AreEqual("1", result.Execute(stack));
            stack.Pop();
        }
        [TestMethod]
        public void MultipleInlineOperators()
        {
            string text = "1+2+3+4";
            var result = TokenParser.CompileV2(text, out int parsedLength);
            var stack = new StaticSiteGenerator.DictionaryStack() { };
            stack.Push();
            Assert.AreEqual("10", result.Execute(stack));
            stack.Pop();
        }
        [TestMethod]
        public void SubtractOperators()
        {
            string text = "2-1";
            var result = TokenParser.CompileV2(text, out int parsedLength);
            var stack = new StaticSiteGenerator.DictionaryStack() { };
            stack.Push();
            Assert.AreEqual("1", result.Execute(stack));
            stack.Pop();
        }
        [TestMethod]
        public void MultiplyOperators()
        {
            string text = "2*2";
            var result = TokenParser.CompileV2(text, out int parsedLength);
            var stack = new StaticSiteGenerator.DictionaryStack() { };
            stack.Push();
            Assert.AreEqual("4", result.Execute(stack));
            stack.Pop();
        }
        [TestMethod]
        public void DivideOperators()
        {
            string text = "6/2";
            var result = TokenParser.CompileV2(text, out int parsedLength);
            var stack = new StaticSiteGenerator.DictionaryStack() { };
            stack.Push();
            Assert.AreEqual("3", result.Execute(stack));
            stack.Pop();
        }
        [TestMethod]
        public void EqualOperators()
        {
            string text = "6=2";
            var result = TokenParser.CompileV2(text, out int parsedLength);
            var stack = new StaticSiteGenerator.DictionaryStack() { };
            stack.Push();
            Assert.AreEqual("false", result.Execute(stack));
            stack.Pop();
        }
        [TestMethod]
        public void Concat()
        {
            string text = "concat('a','b','c')";
            var result = TokenParser.CompileV2(text, out int parsedLength);
            var stack = new StaticSiteGenerator.DictionaryStack() { };
            stack.Push();
            Assert.AreEqual("abc", result.Execute(stack));
            stack.Pop();
        }
        [TestMethod]
        public void ConcatArray()
        {
            string text = "concat('a','b','c','d','e').take(3)";
            var result = TokenParser.CompileV2(text, out int parsedLength);
            var stack = new StaticSiteGenerator.DictionaryStack() { };
            stack.Push();
            Assert.AreEqual("a,b,c", result.Execute(stack));
            stack.Pop();
        }
        [TestMethod]
        public void Join()
        {
            string text = "join('a,b,c','-')";
            var result = TokenParser.CompileV2(text, out int parsedLength);
            var stack = new StaticSiteGenerator.DictionaryStack() { };
            stack.Push();
            Assert.AreEqual("a-b-c", result.Execute(stack));
            stack.Pop();
        }
        [TestMethod]
        public void TestString()
        {
            string text = "concat('1','2')+2";
            var result = TokenParser.CompileV2(text, out int parsedLength);
            var stack = new StaticSiteGenerator.DictionaryStack() { };
            stack.Push();
            Assert.AreEqual("14", result.Execute(stack));
            stack.Pop();
        }
        [TestMethod]
        public void ReverseString()
        {
            string text = "'hello'.reverse()";
            var result = TokenParser.CompileV2(text, out int parsedLength);
            var stack = new StaticSiteGenerator.DictionaryStack() { };
            stack.Push();
            Assert.AreEqual("olleh", result.Execute(stack));
            stack.Pop();
        }
        [TestMethod]
        public void ReverseArray()
        {
            string text = "foreach(to_array('1,2,3').reverse(),var('foreach.key'))";
            var result = TokenParser.CompileV2(text, out int parsedLength);
            var stack = new StaticSiteGenerator.DictionaryStack() { };
            stack.Push();
            Assert.AreEqual("321", result.Execute(stack));
            stack.Pop();
        }
        [TestMethod]
        public void ChainedLinq()
        {
            string text = "join('1,2,3'.to_array().reverse(),'')";
            var result = TokenParser.CompileV2(text, out int parsedLength);
            var stack = new StaticSiteGenerator.DictionaryStack() { };
            stack.Push();
            Assert.AreEqual("321", result.Execute(stack));
            stack.Pop();
        }
        [TestMethod]
        public void ChainedLinq2()
        {
            string text = "foreach('1,2,3'.to_array().reverse(),'fart')";
            var result = TokenParser.CompileV2(text, out int parsedLength);
            var stack = new StaticSiteGenerator.DictionaryStack() { };
            stack.Push();
            Assert.AreEqual("fartfartfart", result.Execute(stack));
            stack.Pop();
        }
    }
}