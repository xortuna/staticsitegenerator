using System.Text;
using StaticSiteGenerator.Engine;
using StaticSiteGenerator.Tools;

namespace StaticSiteGenerator.Tokens.Functions
{
    [FunctionTokenAttribute("minify_url", 1, 1)]
    internal class MinifyUrl : FunctionToken
    {
        protected override string Identifier => "minify_url";
        public MinifyUrl(List<Token> args) : base(args) {
            if (args.Count != 1) throw new ArgumentException("Invalid arguments for MinifyUrl, expected 1 (path)");
        }
        public override string Execute(DictionaryStack stack)
        {
            return PathTools.MinifyUrl(args[0].Execute(stack));
        }
    }
}
