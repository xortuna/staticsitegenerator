using HeyRed.MarkdownSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StaticSiteGenerator.Engine.MarkdownExtensions
{
    internal class DivWrappedImage : IMarkdownExtension
    {
        private static Regex _floatingImage = new Regex(@"(?:\!)\[([^\]]+)\] \[([^\]]+)\] \((.+?[*_]*)\)",
RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

        public string Transform(string text)
        {
            return _floatingImage.Replace(text, "<div class=\"$1\"><img src=\"$3\" alt=\"$2\"></div>");
        }
    }
}
