using System;
using System.Reflection.Metadata;
using System.Text;
using StaticSiteGenerator.Engine;
using StaticSiteGenerator.Tokens.Types;

namespace StaticSiteGenerator.Tokens.Functions
{
    [FunctionTokenAttribute("divide", 2, 2)]
    internal class Divide : NumericOperator
    {

        public Divide(List<Token> args) : base(args)
        {

        }
        protected override string Name => "divide";

        protected override int PerformIntOperation(int a, int b)
        {
            return a / b;
        }

        protected override double PerformDoubleOperation(double a, double b)
        {
            return a / b;
        }
    }
}
