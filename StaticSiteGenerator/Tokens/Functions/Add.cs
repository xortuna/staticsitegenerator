using System;
using System.Reflection.Metadata;
using System.Text;
using StaticSiteGenerator.Engine;
using StaticSiteGenerator.Tokens.Types;

namespace StaticSiteGenerator.Tokens.Functions
{
    internal class Add : NumericOperator
    {

        public Add(List<Token> args) : base(args)
        {

        }
        protected override string Name => "add";

        protected override int PerformIntOperation(int a, int b)
        {
            return a + b;
        }

        protected override double PerformDoubleOperation(double a, double b)
        {
            return a + b;
        }
    }
}
