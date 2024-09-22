using StaticSiteGenerator.Engine;
using StaticSiteGenerator.Tokens.Types;

namespace StaticSiteGenerator.Tokens.Functions
{
    internal class ListFiles : FunctionToken, IStringArray
    {
        public ListFiles(List<Token> args) : base(args)
        {
            if (args.Count < 1 || args.Count > 2) throw new ArgumentException("Invalid arguments for listfiles expected 2-3 (path, {filter})");
        }
        public override string Execute(DictionaryStack stack)
        {
            return string.Join(",", ExecuteList(stack));
        }

        public IEnumerable<string> ExecuteList(DictionaryStack stack)
        {
            var root = stack.Get("root.fullpath");
            var di = new DirectoryInfo(Path.Join(root, args[0].Execute(stack)));

            if (!di.Exists)
                throw new Exception($"Path {di.FullName} does not exist");

            string patern = args.Count == 2 ? args[1].Execute(stack) : "*";
            return di.EnumerateFiles(patern).OrderByDescending(r=>r.CreationTime).Select(r => r.FullName);
        }
    }
}
