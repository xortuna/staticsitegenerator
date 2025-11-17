using StaticSiteGenerator.Engine;
using StaticSiteGenerator.Tokens.Types;

namespace StaticSiteGenerator.Tokens.Functions
{
    [FunctionTokenAttribute("orderby", 2, 3)]
    internal class OrderBy: FunctionToken, IStringArray
    {

        protected override string Identifier => "orderby";
        /// <summary>
        /// Reads the metadata into memory
        /// </summary>
        /// <param name="args">
        /// 1 Filepath
        /// 2 Then()
        /// </param>
        public OrderBy(List<Token> args) : base(args) { 
            if (args.Count < 2 || args.Count > 3) throw new ArgumentException("Invalid arguments for readmetadata, expected 2-3 (array, resolver, {desc})");
        }

        public override string Execute(DictionaryStack stack)
        {
            return string.Join("", ExecuteList(stack));
        }

        public IEnumerable<string> ExecuteList(DictionaryStack stack)
        {

            string keyName = args[1].Execute(stack);
            bool descending = args.Count == 3 && args[2].Execute(stack) == "desc";

            IEnumerable<string> strings;
            if (args[0] is IStringArray sa)
            {
                strings = sa.ExecuteList(stack);
            }
            else
            {
                strings = args[0].Execute(stack).Split(',');
            }
            if (strings == null)
                return [""];

            var ordered = strings.OrderBy((r) => {
                stack.Push();
                stack.Add("orderby.key", r);
                var ret = args[1].Execute(stack);
                stack.Pop();
                return ret;
            });
            if (descending)
                return ordered.Reverse();
            return ordered;
        }
    }
}
