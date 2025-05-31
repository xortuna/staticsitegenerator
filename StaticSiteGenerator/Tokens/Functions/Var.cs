using System.Text;
using StaticSiteGenerator.Engine;

namespace StaticSiteGenerator.Tokens.Functions
{
    [FunctionTokenAttribute("var", 1, 1)]
    internal class Var : FunctionToken
    {
        protected override string Identifier => "var";

        public Var(List<Token> args) : base(args) {
            if (args.Count != 1) throw new ArgumentException("Invalid arguments for var, expected 1 (variable name)");
        }
        public override string Execute(DictionaryStack stack)
        {

            var arg = args[0].Execute(stack);

            if (!stack.ContainsKey(arg))
                throw new Exception($"ERROR: Meta data paramater {arg} was not set prior to read");
            return stack.Get(arg);
        }
    }
}
