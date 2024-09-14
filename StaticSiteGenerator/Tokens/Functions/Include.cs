using System.Text;
using StaticSiteGenerator.Engine;

namespace StaticSiteGenerator.Tokens.Functions
{
    internal class Include : FunctionToken
    {
        public Include(List<Token> args) : base(args) {
            if (args.Count != 1) throw new ArgumentException("Invalid arguments for include, expected 1 (partial name)");
        }
        public override string Execute(DictionaryStack stack)
        {

            var path = args[0].Execute(stack);

            var target = new FileInfo(Path.Combine(Program._rootDirectory.FullName, "_partial", path));
            if (!target.Exists)
            {
                throw new ArgumentException("Included file does not exist");
            }

            stack.Push();
            stack.Add("partial.name", target.Name);
            StringBuilder sb = new StringBuilder();
            foreach (var element in TemplateTokenizer.ProcessFile(target))
            {
                switch (element.Type)
                {
                    case TemplateType.Content:
                        sb.Append(element.Content);
                        break;
                    case TemplateType.Token:
                        try
                        {
                            var func = TokenParser.Compile(element.Content);
                            stack.Push();
                            sb.Append(func.Execute(stack));
                            stack.Pop();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Error parsing {target.FullName}:line {element.Line} {element.Content}", innerException: ex);
                        }
                        break;
                }
            }

            stack.Pop();
            return sb.ToString();
        }
    }
}
