using StaticSiteGenerator.Engine;
using StaticSiteGenerator.Tokens.Types;

namespace StaticSiteGenerator.Tokens.Functions
{
    [FunctionTokenAttribute("where_metadata", 2, 2)]
    internal class WhereMetaData : FunctionToken, IStringArray
    {

        protected override string Identifier => "where_metadata";
        /// <summary>
        /// Reads the metadata into memory
        /// </summary>
        /// <param name="args">
        /// 1 Filepath
        /// 2 Then()
        /// </param>
        public WhereMetaData(List<Token> args) : base(args) { 
            if (args.Count != 2) throw new ArgumentException("Invalid arguments for readmetadata, expected 3 (array, shouldFilter {return true/false})");
        }

        public override string Execute(DictionaryStack stack)
        {
            return string.Join("", ExecuteList(stack));
        }

        public IEnumerable<string> ExecuteList(DictionaryStack stack)
        {

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

            return strings.Where((r) => {
                 stack.Push();
                 foreach(var val in GetMetadataValues(r))
                 stack.Add(val.Key, val.Value);
                 var ret = args[1].Execute(stack);
                 stack.Pop();
                 return ret == "true" || ret == "1";
             });
        }

        private IEnumerable<MetaDataParser.MetaData> GetMetadataValues(string path)
        {
           
            FileInfo fi = new FileInfo(path);
            if (!fi.Exists)
                throw new ArgumentNullException("args", $"Where_Metadata key is not a file: {path}");

            foreach (var metadata in TemplateTokenizer.ProcessFile(fi).Where(t => t.Type == TemplateType.Metadata))
            {
                yield return MetaDataParser.Parse(metadata);
            }
        }
   
    }
}
