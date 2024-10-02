using StaticSiteGenerator.Engine;
using StaticSiteGenerator.Tokens.Types;

namespace StaticSiteGenerator.Tokens.Functions
{
    [FunctionTokenAttribute("where", 2, 3)]
    internal class Where : FunctionToken, IStringArray
    {
        protected override string Identifier => "where";

        public Where(List<Token> args) : base(args)
        {
            if (args.Count < 2 || args.Count > 3) throw new ArgumentException("Invalid arguments for where expected 2-3 (array, evaulator, {array variable}");
        }
        public override string Execute(DictionaryStack stack)
        {
            return string.Join(",", ExecuteList(stack));
        }

        public IEnumerable<string> ExecuteList(DictionaryStack stack)
        {
            string? keyName = (args.Count == 3) ? args[2].Execute(stack) : "where.key";
            if (args[0] is IStringArray sa)
            {
                foreach (var t in sa.ExecuteList(stack))
                {
                    stack.Push();
                    stack.Add(keyName, t);
                    if(args[1].Execute(stack) == "true")
                    {
                        yield return t;
                    }
                    stack.Pop();
                }
            }
            else
            {
                var arg = args[0].Execute(stack);
                foreach (var t in arg.Split(','))
                {
                    stack.Push();
                    stack.Add(keyName, t);
                    if (args[1].Execute(stack) == "true")
                    {
                        yield return t;
                    }
                    stack.Pop();
                }
            }
        }
    }
}
