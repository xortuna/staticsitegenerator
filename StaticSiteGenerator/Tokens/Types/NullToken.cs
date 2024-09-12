using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StaticSiteGenerator.Engine;

namespace StaticSiteGenerator.Tokens.Types
{
    internal class NullToken : Token
    {
        public override string Execute(DictionaryStack stack)
        {
            throw new Exception("NULL value requested");
        }
    }
}
