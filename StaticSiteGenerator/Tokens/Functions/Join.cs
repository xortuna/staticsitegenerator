using StaticSiteGenerator.Engine;
using StaticSiteGenerator.Tokens.Types;

namespace StaticSiteGenerator.Tokens.Functions
{
    internal class Join : FunctionToken
    {
        public Join(List<Token> args) : base(args)
        {
            if (args.Count < 1 || args.Count > 2) throw new ArgumentException($"Invalid arguments for join expected 1-2 (array, {{join char}}) got {args.Count}");
        }
        public override string Execute(DictionaryStack stack)
        {
            var joinChar = args.Count == 1 ? "," : args[1].Execute(stack);
            if (args[0] is IStringArray sr)
                return string.Join(joinChar, sr.ExecuteList(stack));
            return String.Join(joinChar, args[0].Execute(stack).Split(","));
        }
    }
}
