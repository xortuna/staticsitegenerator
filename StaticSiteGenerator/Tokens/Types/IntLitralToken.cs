using StaticSiteGenerator.Engine;

namespace StaticSiteGenerator.Tokens.Types
{
    public interface IInt
    {
        int ExecuteInt(DictionaryStack stack);
    }
    class IntLiteralToken : Token, IInt
    {
        int _value;
        public IntLiteralToken(int Value) { this._value = Value; }
        public override string Execute(DictionaryStack stack)
        {
            return _value.ToString();
        }
        public int ExecuteInt(DictionaryStack stack)
        {
            return _value;
        }
    }
}
