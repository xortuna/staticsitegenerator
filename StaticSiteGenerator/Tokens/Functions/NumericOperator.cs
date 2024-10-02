using System;
using System.Reflection.Metadata;
using System.Text;
using StaticSiteGenerator.Engine;
using StaticSiteGenerator.Tokens.Types;

namespace StaticSiteGenerator.Tokens.Functions
{
    abstract class NumericOperator : FunctionToken, IInt, IDouble
    {
        protected override string Identifier => Name;
        protected abstract string Name { get; }
        public NumericOperator(List<Token> args) : base(args)
        {
            if (args.Count != 2) throw new ArgumentException($"Invalid arguments for {Name} Expected 2 (Variable Name, Value) got {args.Count}");
        }
        protected abstract int PerformIntOperation(int a, int b);
        protected abstract double PerformDoubleOperation(double a, double b);

        public override string Execute(DictionaryStack stack)
        {
            if (args[0] is IInt)
            {
                return ExecuteInt(stack).ToString();
            }
    
            return ExecuteDouble(stack).ToString();

        }

        public int ExecuteInt(DictionaryStack stack)
        {
            int a = 0;
            int b = 0;
            if (args[0] is IInt ia)
            {
                a = ia.ExecuteInt(stack);
            }
            else
            {
                var avar = args[0].Execute(stack);
                if (avar.Contains('.'))
                {
                    if (double.TryParse(avar, out double ad))
                        a = (int)ad;
                }
                else
                {
                    int.TryParse(avar, out a);
                }
            }

            if (args[1] is IInt ib)
            {
                b = ib.ExecuteInt(stack);
            }
            else
            {
                var bvar = args[1].Execute(stack);
                if (bvar.Contains('.'))
                {
                    if (double.TryParse(bvar, out double bd))
                        b = (int)bd;
                }
                else
                {
                    int.TryParse(bvar, out b);
                }
            }
            return PerformIntOperation(a, b);
        }
        public double ExecuteDouble(DictionaryStack stack)
        {
            double a = 0;
            double b = 0;
            if (args[0] is IDouble ia)
            {
                a = ia.ExecuteDouble(stack);
            }
            else
            {
                var avar = args[0].Execute(stack);
                double.TryParse(avar, out a);
            }

            if (args[1] is IDouble ib)
            {
                b = ib.ExecuteDouble(stack);
            }
            else
            {
                var bvar = args[1].Execute(stack);
                double.TryParse(bvar, out b);
            }
            return PerformDoubleOperation(a, b);
        }
    }
}
