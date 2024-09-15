using StaticSiteGenerator.Engine;
using StaticSiteGenerator.Tokens.Types;

namespace StaticSiteGenerator.Tokens.Functions
{
    internal class Take: FunctionToken, IStringArray
    {
        public Take(List<Token> args) : base(args)
        {
            if (args.Count != 2) throw new ArgumentException($"Invalid arguments for Take expected 2 (array, count) got {args.Count}.");
        }
        public override string Execute(DictionaryStack stack)
        {
            return string.Join(",", ExecuteList(stack));
        }

        public IEnumerable<string> ExecuteList(DictionaryStack stack)
        {
            int take = 0;
            if(args[1] is IInt itoken)
            {
                take = itoken.ExecuteInt(stack);
            }
            else
            {
                if (!int.TryParse(args[1].Execute(stack), out take))
                    throw new ArgumentException("Take() Expected integer for argument 1");
            }

            if (args[0] is IStringArray atoken)
            {
                return atoken.ExecuteList(stack).Take(take);
            }
            else
            {
                return args[0].Execute(stack).Split(",").Take(take);

            }
        }
    }
}
