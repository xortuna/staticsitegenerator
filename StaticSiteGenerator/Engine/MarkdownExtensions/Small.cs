using HeyRed.MarkdownSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StaticSiteGenerator.Engine.MarkdownExtensions
{
    internal class Small : IMarkdownExtension
    {
        private static Regex _bold = new Regex(@"(\~\~) (?=\S) (.+?[~]*) (?<=\S) \1",
    RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

        public string Transform(string text)
        {
            return _bold.Replace(text, "<small>$2</small>");
        }
    }
}
