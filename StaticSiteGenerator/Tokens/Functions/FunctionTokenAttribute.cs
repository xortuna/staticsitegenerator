
namespace StaticSiteGenerator.Tokens.Functions
{
    internal class FunctionTokenAttribute : Attribute
    {
        public FunctionTokenAttribute(string name, int minArgs = 0, int maxArgs = 0)
        {
            Name = name;
            MinArgs = minArgs;
            MaxArgs = maxArgs;
        }
        public string Name { get; }
        public int MinArgs { get; }
        public int MaxArgs { get; }
    }
}