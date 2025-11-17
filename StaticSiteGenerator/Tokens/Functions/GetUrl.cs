using StaticSiteGenerator.Engine;
using StaticSiteGenerator.Tools;

namespace StaticSiteGenerator.Tokens.Functions
{

    [FunctionTokenAttribute("get_url", 1, 1)]
    internal class GetUrl : FunctionToken
    {
        protected override string Identifier => "get_url";
        public GetUrl(List<Token> args) : base(args) {
            if (args.Count != 1) throw new ArgumentException("Invalid arguments for GetUrl, expected 1 (url)");
        }

        public override string Execute(DictionaryStack stack)
        {
            string relative = args[0].Execute(stack);
            if (relative.Length > 1 && relative.StartsWith(stack.Get("root.fullpath"))) {
                string rootPath = stack.Get("root.fullpath");
                relative = relative.Substring(rootPath.Length).ToUrlPath();
            }

            if (relative.EndsWith(".md"))
            {
                var ix = relative.LastIndexOf("/");
                if(ix != -1)
                {
                   var filePath = relative.Substring(ix+1, relative.Length-ix-4);
                   var sub = PathTools.Decamel(filePath);
                   relative = relative.Substring(0, ix) + "/" + sub + "/";
                }
            }


            return relative;
        }
    }
}
