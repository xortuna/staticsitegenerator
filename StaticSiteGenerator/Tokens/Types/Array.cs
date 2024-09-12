using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using StaticSiteGenerator.Engine;

namespace StaticSiteGenerator.Tokens.Types
{
    public interface IStringArray
    {
        IEnumerable<string> ExecuteList(DictionaryStack stack);
    }

    internal class TokenArray : Token, IStringArray
    {
        List<Token> _items;

        public TokenArray(List<Token> items)
        {
            _items = items;
        }

        public override string Execute(DictionaryStack stack)
        {
            return  String.Join(",", _items.Select(t=>t.Execute(stack)));
        }

        public IEnumerable<string> ExecuteList(DictionaryStack stack)
        {
            return _items.Select(t => t.Execute(stack));
        }
    }
}