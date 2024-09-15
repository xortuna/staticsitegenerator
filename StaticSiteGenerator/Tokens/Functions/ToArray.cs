using System.Text;
using StaticSiteGenerator.Engine;
using StaticSiteGenerator.Tokens.Types;

namespace StaticSiteGenerator.Tokens.Functions
{
    internal class ToArray : FunctionToken, IStringArray
    {
        public ToArray(List<Token> args) : base(args)
        {
            if (args.Count != 1) throw new ArgumentException($"Invalid arguments for ToArray Expected 1 got {args.Count}");
        }
        public override string Execute(DictionaryStack stack)
        {
            return args[0].Execute(stack);
        }
        public IEnumerable<string> ExecuteList(DictionaryStack stack)
        {
            return args[0].Execute(stack).Split(",");
        }
    }
}
