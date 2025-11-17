using StaticSiteGenerator.Engine;

namespace StaticSiteGenerator.Tokens.Functions
{

    [FunctionTokenAttribute("format_date", 2, 2)]
    internal class FormatDate : FunctionToken
    {
        protected override string Identifier => "format_date";
        public FormatDate(List<Token> args) : base(args) {
            if (args.Count != 2) throw new ArgumentException("Invalid arguments for GetUrl, expected 2 (date, output format)");
        }

        public override string Execute(DictionaryStack stack)
        {
            string relative = args[0].Execute(stack);
            DateTime? targetDate = null;
            if (DateTime.TryParseExact(relative, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime date2))
            {
                targetDate = date2;
            }
            else if(DateTime.TryParseExact(relative, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime date))
            {
                targetDate = date;
            }

            if (targetDate != null)
            {
                return targetDate.Value.ToString(args[1].Execute(stack));
            }
            return "Invalid date format";
        }
    }
}
