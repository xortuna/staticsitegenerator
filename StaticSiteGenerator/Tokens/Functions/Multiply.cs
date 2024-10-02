using System;
using System.Reflection.Metadata;
using System.Text;
using StaticSiteGenerator.Engine;
using StaticSiteGenerator.Tokens.Types;

namespace StaticSiteGenerator.Tokens.Functions
{
    [FunctionTokenAttribute("multiply", 2, 2)]
    internal class Multiply : NumericOperator
    {
        public Multiply(List<Token> args) : base(args)
        {

        }
        protected override string Name => "multiply";

        protected override int PerformIntOperation(int a, int b)
        {
            return a * b;
        }

        protected override double PerformDoubleOperation(double a, double b)
        {
            return a * b;
        }
    }
}
