using System.Text;
using StaticSiteGenerator.Engine;
using StaticSiteGenerator.Tokens.Types;

namespace StaticSiteGenerator.Tokens.Functions
{
    [FunctionTokenAttribute("concat", 1, 99)]
    internal class Concat : FunctionToken, IStringArray
    {
        protected override string Identifier => "concat";
        public Concat(List<Token> args) : base(args)
        {
            if (args.Count == 0) throw new ArgumentException("Invalid arguments for Concat Expected >1");
        }
        public override string Execute(DictionaryStack stack)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var token in args)
            {
                sb.Append(token.Execute(stack));
            }

            return sb.ToString();
        }
        public IEnumerable<String> ExecuteList(DictionaryStack stack)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var token in args)
            {
                if (token is IStringArray sa)
                {
                    foreach (var s in sa.ExecuteList(stack))
                        yield return s;
                }
                else
                    yield return token.Execute(stack);
            }
        }
    }
}
