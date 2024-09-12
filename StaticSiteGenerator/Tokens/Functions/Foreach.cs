using StaticSiteGenerator.Engine;
using StaticSiteGenerator.Tokens.Types;

namespace StaticSiteGenerator.Tokens.Functions
{
    internal class Foreach : FunctionToken, IStringArray
    {
        public Foreach(List<Token> args) : base(args)
        {
            if (args.Count < 2 || args.Count > 3) throw new ArgumentException("Invalid arguments for foreach expected 2-3 (array, print, {array variable}");
        }
        public override string Execute(DictionaryStack stack)
        {
            return string.Join(",", ExecuteList(stack));
        }

        public IEnumerable<string> ExecuteList(DictionaryStack stack)
        {

            string? keyName = (args.Count == 3) ? args[2].Execute(stack) : "foreach.key";
            if (args[0] is IStringArray sa)
            {
                foreach (var t in sa.ExecuteList(stack))
                {
                    stack.Push();
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
                    stack.Push();
                    stack.Add(keyName, t);
                    yield return args[1].Execute(stack);
                    stack.Pop();
                }
            }
        }
    }
}
