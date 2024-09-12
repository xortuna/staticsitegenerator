using System.Text;
using StaticSiteGenerator.Engine;

namespace StaticSiteGenerator.Tokens.Functions
{
    internal class GetUrl : FunctionToken
    {
        public static string Decamel(string camel)
        {
            var sb = new StringBuilder();
            foreach (var c in camel)
            {
                if (c >= 65 && c <= 90)
                {
                    if (sb.Length != 0)
                        sb.Append("_");
                    sb.Append((char)(c + 32));
                }
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        public GetUrl(List<Token> args) : base(args) {
            if (args.Count != 1) throw new ArgumentException("Invalid arguments for GetUrl, expected 1 (url)");
        }


        public override string Execute(DictionaryStack stack)
        {

            var arg = args[0].Execute(stack);

            string rootPath = stack.Get("root.fullpath");
            var relative = arg.Substring(rootPath.Length).Replace("\\", "/");
            if (relative.EndsWith(".md"))
            {
                var ix = relative.LastIndexOf("/");
                if(ix != -1)
                {
                   var filePath = relative.Substring(ix+1, relative.Length-ix-4);
                   var sub = Decamel(filePath);
                   relative = relative.Substring(0, ix) + "/" + sub + "/";
                }


            }


            return relative;
        }
    }
}
