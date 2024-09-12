using StaticSiteGenerator.Tokens.Functions;
using StaticSiteGenerator.Tokens.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeyRed.MarkdownSharp;
namespace StaticSiteGenerator.Engine
{
    public static class MarkdownParser
    {
        public static string CompileText(string markdown)
        {
            Markdown mark = new Markdown();
            return mark.Transform(markdown);
        }
    }
}
