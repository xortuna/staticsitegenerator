using StaticSiteGenerator.Engine;

namespace StaticSiteGenerator.Tokens.Functions
{
    internal class LoadMetaData : FunctionToken
    {
        /// <summary>
        /// Reads the metadata into memory
        /// </summary>
        /// <param name="args">
        /// 1 Filepath
        /// 2 Then()
        /// </param>
        public LoadMetaData(List<Token> args) : base(args) { 
            if (args.Count != 2) throw new ArgumentException("Invalid arguments for readmetadata, expected 2 (template file, then)");
        }
        public override string Execute(DictionaryStack stack)
        {
            FileInfo fi = new FileInfo(args[0].Execute(stack));
            if (!fi.Exists)
                throw new ArgumentNullException("args", $"File {fi.FullName} does not exist");

            stack.Push();
            foreach (var metadata in TemplateFileParser.ProcessFile(fi).Where(t => t.Type == TemplateType.Metadata))
            {
                var k = MetaDataParser.Parse(metadata);
                stack.Add(k.Key, k.Value);
            }

            var r = args[1].Execute(stack);
            stack.Pop();
            return r;
        }

   
    }
}
