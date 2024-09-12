using StaticSiteGenerator.Engine;

namespace StaticSiteGenerator.Tokens.Types
{
    class StringLiteralToken : Token
    {
        string _value;
        public StringLiteralToken(string Value) { this._value = Value; }
        public override string Execute(DictionaryStack stack)
        {
            return _value;
        }
    }
}
