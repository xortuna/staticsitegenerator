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
        //[Classes to append] [AltText] (MainImgSrc) (SourceSets for Chome12+)
        private static Regex _4ParamImage = new Regex(@"(?:\!)\[([^\]]+)\] \[([^\]]+)\] \(([^\)]+)\) \(([^\)]+)\)", RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

        //[Classes to append] [AltText] (MainImgSrc)
        private static Regex _3ParamImg = new Regex(@"(?:\!)\[([^\]]+)\] \[([^\]]+)\] \(([^\)]+)\)",
        RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

        //[AltText] (MainImgSrc) (SourceSet)
        private static Regex _3ParamImgSourceSet = new Regex(@"(?:\!)\[([^\]]+)\] \(([^\)]+)\) \(([^\)]+)\)",
        RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);


        public string Transform(string text)
        {
            text = _4ParamImage.Replace(text, "<div class=\"$1\"><img src=\"$3\" alt=\"$2\" srcset=\"$4\"></div>");
            text = _3ParamImgSourceSet.Replace(text, "<img src=\"$2\" alt=\"$1\" srcset=\"$3\">");
            return _3ParamImg.Replace(text, "<div class=\"$1\"><img src=\"$3\" alt=\"$2\"></div>");
        }
    }
}
