using StaticSiteGenerator.Engine;
using StaticSiteGenerator.Tokens.Types;

namespace StaticSiteGenerator.Tokens.Functions
{
    [FunctionTokenAttribute("foreach", 2, 3)]
    internal class Foreach : FunctionToken, IStringArray
    {
        protected override string Identifier => "foreach";
        public Foreach(List<Token> args) : base(args)
        {
            if (args.Count < 2 || args.Count > 3) throw new ArgumentException($"Invalid arguments for foreach expected 2-3 (array, print, {{array variable}}) got {args.Count}");
        }
        public override string Execute(DictionaryStack stack)
        {
            return string.Join("", ExecuteList(stack));
        }

        public IEnumerable<string> ExecuteList(DictionaryStack stack)
        {

            string? keyName = ((args.Count == 3) ? args[2].Execute(stack) : "foreach") + ".key";
            string? indexName = ((args.Count == 3) ? args[2].Execute(stack) : "foreach") + ".index";
            int index = 0; 
            if (args[0] is IStringArray sa)
            {
                foreach (var t in sa.ExecuteList(stack))
                {
                    index++;
                    stack.Push();
                    stack.Add(indexName, index.ToString());
                    stack.Add(keyName, t);
                    yield return args[1].Execute(stack);
                    stack.Pop();
                }
            }
            else
            {
                var arg = args[0].Execute(stack);
                foreach (var t in arg.Split(','))
                {
                    index++;
                    stack.Push();
                    stack.Add(indexName, index.ToString());
                    stack.Add(keyName, t);
                    yield return args[1].Execute(stack);
                    stack.Pop();
                }
            }
        }
    }
}
