using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StaticSiteGenerator.Engine
{
    internal class Lexer<T>
    {
        List<KeyValuePair<Regex, T>> Lexes = new List<KeyValuePair<Regex, T>>();
        public Lexer() { }
        public void AddRule(Regex pattern, T value)
        {
            Lexes.Add(new KeyValuePair<Regex, T>(pattern, value));
        }

    }
}
