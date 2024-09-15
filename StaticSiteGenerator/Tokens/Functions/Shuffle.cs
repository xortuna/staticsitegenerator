using StaticSiteGenerator.Engine;
using StaticSiteGenerator.Tokens.Types;

namespace StaticSiteGenerator.Tokens.Functions
{
    internal class Shuffle: FunctionToken, IStringArray
    {
        static Random RND = new Random();
        public Shuffle(List<Token> args) : base(args)
        {
            if (args.Count != 1) throw new ArgumentException("Invalid arguments for shuffle expected 1 (array)");
        }
        public override string Execute(DictionaryStack stack)
        {
            return string.Join(",", ExecuteList(stack));
        }

        public IEnumerable<string> ExecuteList(DictionaryStack stack)
        {
            if(args[0] is IStringArray sr)
            {
                var list = sr.ExecuteList(stack).ToList();
                List<string> output = new List<string>(list.Count());
                while(list.Count > 0)
                {
                    var idx= RND.Next(0, list.Count);
                    output.Add(list[idx]);
                    list.RemoveAt(idx);
                }
                return output;
            }
            else
            {
                var list = args[0].Execute(stack).Split(",").ToList();
                List<string> output = new List<string>(list.Count());
                while (list.Count > 0)
                {
                    var idx = RND.Next(0, list.Count);
                    output.Add(list[idx]);
                    list.RemoveAt(idx);
                }
                return output;
            }
        }
    }
}
