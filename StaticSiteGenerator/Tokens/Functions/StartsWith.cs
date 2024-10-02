using System.Text;
using StaticSiteGenerator.Engine;

namespace StaticSiteGenerator.Tokens.Functions
{
    [FunctionTokenAttribute("starts_with", 2, 2)]
    internal class StartsWith : FunctionToken
    {
        protected override string Identifier => "starts_with";
        public StartsWith(List<Token> args) : base(args) {
            if (args.Count != 2) throw new ArgumentException($"Invalid arguments for starts_with, expected 2 (text, startswith) got {args.Count}.");
        }
        public override string Execute(DictionaryStack stack)
        {
            var lh = args[0].Execute(stack);
            var rh = args[1].Execute(stack);
            if (lh.StartsWith(rh))
                return "true";
            else
                return "false";
        }
    }
}
