using System.Text;
using StaticSiteGenerator.Engine;

namespace StaticSiteGenerator.Tokens.Functions
{
    [FunctionTokenAttribute("equal", 2, 2)]
    internal class Equal : FunctionToken
    {
        protected override string Identifier => "equal";
        public Equal(List<Token> args) : base(args) {
            if (args.Count != 2) throw new ArgumentException("Invalid arguments for equal, expected 2 (a, b)");
        }
        public override string Execute(DictionaryStack stack)
        {
            var lh = args[0].Execute(stack);
            var rh = args[1].Execute(stack);
            if (lh == rh)
                return "true";
            else
                return "false";
        }
    }
}
