using StaticSiteGenerator.Engine;

namespace StaticSiteGenerator.Tokens.Types
{
    public interface IDouble
    {
        double ExecuteDouble (DictionaryStack stack);
    }
    class DoubleLiteralToken : Token, IDouble
    {
        double _value;
        public DoubleLiteralToken(double Value) { this._value = Value; }
        public override string Execute(DictionaryStack stack)
        {
            return _value.ToString();
        }
        public double ExecuteDouble(DictionaryStack stack)
        {
            return _value;
        }
    }
}
