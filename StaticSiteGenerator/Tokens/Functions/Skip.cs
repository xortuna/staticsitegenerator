using StaticSiteGenerator.Engine;
using StaticSiteGenerator.Tokens.Types;

namespace StaticSiteGenerator.Tokens.Functions
{
    [FunctionTokenAttribute("skip", 2, 2)]
    internal class Skip: FunctionToken, IStringArray
    {
        protected override string Identifier => "skip";
        public Skip(List<Token> args) : base(args)
        {
            if (args.Count != 2) throw new ArgumentException($"Invalid arguments for Skip expected 2 (array, count) got {args.Count}.");
        }
        public override string Execute(DictionaryStack stack)
        {
            return string.Join(",", ExecuteList(stack));
        }

        public IEnumerable<string> ExecuteList(DictionaryStack stack)
        {
            int take = 0;
            if (args[1] is IInt itoken)
            {
                take = itoken.ExecuteInt(stack);
            }
            else
            {
                if (!int.TryParse(args[1].Execute(stack), out take))
                    throw new ArgumentException("Take() Expected integer for argument 1");
            }

            if (args[0] is IStringArray sr)
            {
                return sr.ExecuteList(stack).Skip(take);
            }
            else
            {
                return args[0].Execute(stack).Split(",").Skip(take);

            }
        }
    }
}
