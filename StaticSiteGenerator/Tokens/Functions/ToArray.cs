using System.Text;
using StaticSiteGenerator.Engine;
using StaticSiteGenerator.Tokens.Types;

namespace StaticSiteGenerator.Tokens.Functions
{
    [FunctionTokenAttribute("to_array", 1, 1)]
    internal class ToArray : FunctionToken, IStringArray
    {
        protected override string Identifier => "to_array";
        public ToArray(List<Token> args) : base(args)
        {
            if (args.Count != 1) throw new ArgumentException($"Invalid arguments for to_array Expected 1 got {args.Count}");
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
