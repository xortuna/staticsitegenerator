using HeyRed.MarkdownSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StaticSiteGenerator.Engine.MarkdownExtensions
{
    internal class Achivement : IMarkdownExtension
    {
        private static Regex _AchivementLink = new Regex(@"(\@\@) (?=\S) \[(.+?[*_]*)\] (.+?[*_]*) (?<=\S)\1",
    RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

        static string AchivementText = "<figure class=\"text-end\" title=\"Achivement available in the Piste App\">" +
            "<blockquote class=\"blockquote\">" +
            "<p>" +
            "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"24\" height=\"24\" fill=\"currentColor\" class=\"bi bi-trophy-fill\" style=\"margin-right: 10px;\" viewBox=\"0 0 16 16\">" +
            "<path d=\"M2.5.5A.5.5 0 0 1 3 0h10a.5.5 0 0 1 .5.5q0 .807-.034 1.536a3 3 0 1 1-1.133 5.89c-.79 1.865-1.878 2.777-2.833 3.011v2.173l1.425.356c.194.048.377.135.537.255L13.3 15.1a.5.5 0 0 1-.3.9H3a.5.5 0 0 1-.3-.9l1.838-1.379c.16-.12.343-.207.537-.255L6.5 13.11v-2.173c-.955-.234-2.043-1.146-2.833-3.012a3 3 0 1 1-1.132-5.89A33 33 0 0 1 2.5.5m.099 2.54a2 2 0 0 0 .72 3.935c-.333-1.05-.588-2.346-.72-3.935m10.083 3.935a2 2 0 0 0 .72-3.935c-.133 1.59-.388 2.885-.72 3.935\" />" +
            "</svg>" +
            "$2." +
            "</p>" +
            "</blockquote>" +
            "<figcaption class=\"blockquote-footer\">" +
            "$3<br/>" +
            "<small><em>Achievement available in the Piste app</em></small>" +
            "</figcaption>" +
            "</figure>";

        public string Transform(string text)
        {
            return _AchivementLink.Replace(text, AchivementText);
        }
    }
}
