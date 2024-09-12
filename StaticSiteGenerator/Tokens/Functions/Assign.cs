using System.Text;
using StaticSiteGenerator.Engine;

namespace StaticSiteGenerator.Tokens.Functions
{
    internal class Assign : FunctionToken
    {
        public Assign(List<Token> args) : base(args)
        {
            if (args.Count != 2) throw new ArgumentException("Invalid arguments for Assign Expected 2: (Variable Name, Value)");
        }
        public override string Execute(DictionaryStack stack)
        {
            var key = args[0].Execute(stack);
            var value = args[1].Execute(stack);
            stack.Add(key, value);

            return "";
        }
    }
}
