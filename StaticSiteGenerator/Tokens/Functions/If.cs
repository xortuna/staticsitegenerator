using System.Text;
using StaticSiteGenerator.Engine;

namespace StaticSiteGenerator.Tokens.Functions
{
    internal class If : FunctionToken
    {
        public If(List<Token> args) : base(args) {
            if (args.Count < 2 || args.Count > 3) throw new ArgumentException("Invalid arguments for if, expected 2-3 (conditon, if true, {if false})");
        }
        public override string Execute(DictionaryStack stack)
        {
            var condition = args[0].Execute(stack);
            if (condition == "true" || condition == "1")
                return args[1].Execute(stack);
            else if(args.Count == 3)
                return args[2].Execute(stack);
            return "";
        }
    }
}
