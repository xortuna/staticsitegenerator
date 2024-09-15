using System.Text;
using StaticSiteGenerator.Engine;
using StaticSiteGenerator.Tokens.Types;

namespace StaticSiteGenerator.Tokens.Functions
{
    internal class Reverse : FunctionToken, IStringArray
    {
        public Reverse(List<Token> args) : base(args) {
            if (args.Count != 1) throw new ArgumentException("Invalid arguments for reverse, expected 1 (string)");
        }
        public override string Execute(DictionaryStack stack)
        {
            return new string(args[0].Execute(stack).Reverse().ToArray());
        }

        public IEnumerable<string> ExecuteList(DictionaryStack stack)
        {
            if(args[0] is IStringArray sr)
            {
                return sr.ExecuteList(stack).Reverse();
            }
            return args[0].Execute(stack).Split(",").Reverse();
        }
    }
}
