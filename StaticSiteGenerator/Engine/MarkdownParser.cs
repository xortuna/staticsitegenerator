using StaticSiteGenerator.Tokens.Functions;
using StaticSiteGenerator.Tokens.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeyRed.MarkdownSharp;
using StaticSiteGenerator.Engine.MarkdownExtensions;
namespace StaticSiteGenerator.Engine
{
    public static class MarkdownParser
    {
        public static string CompileText(string markdown)
        {
            Markdown mark = new Markdown();
            mark.AddExtension(new Small());
            mark.AddExtension(new Achivement());
            mark.AddExtension(new DivWrappedImage());
            return mark.Transform(markdown);
        }
    }
}
