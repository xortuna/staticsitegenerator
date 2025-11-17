using StaticSiteGenerator.Engine;
using StaticSiteGenerator.Tokens.Types;

namespace StaticSiteGenerator.Tokens.Functions
{
    [FunctionTokenAttribute("orderby_metadata", 2, 3)]
    internal class OrderByMetaData : FunctionToken, IStringArray
    {

        protected override string Identifier => "orderby_metadata";
        /// <summary>
        /// Reads the metadata into memory
        /// </summary>
        /// <param name="args">
        /// 1 Filepath
        /// 2 Then()
        /// </param>
        public OrderByMetaData(List<Token> args) : base(args) { 
            if (args.Count < 2 || args.Count > 3) throw new ArgumentException("Invalid arguments for readmetadata, expected 2-3 (array, key, {desc})");
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

            var ordered = strings.OrderBy((r) => GetMetadataValue(r, keyName));
            if (descending)
                return ordered.Reverse();
            return ordered;
        }

        private string GetMetadataValue(string path, string key)
        {
           
            FileInfo fi = new FileInfo(path);
            if (!fi.Exists)
                throw new ArgumentNullException("args", $"OrderByMetadata key is not a file: {path}");

            foreach (var metadata in TemplateTokenizer.ProcessFile(fi).Where(t => t.Type == TemplateType.Metadata))
            {
                var k = MetaDataParser.Parse(metadata);
                if(k.Key == key)
                    return k.Value;
            }

            throw new ArgumentNullException("args", $"OrderByMetadata metadata {key} is not set in : {path}");
        }
   
    }
}
