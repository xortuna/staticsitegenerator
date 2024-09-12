﻿using System.Text;
using StaticSiteGenerator.Engine;

namespace StaticSiteGenerator.Tokens.Functions
{
    internal class Var : FunctionToken
    {
        public Var(List<Token> args) : base(args) {
            if (args.Count != 1) throw new ArgumentException("Invalid arguments for var, expected 1 (variable name)");
        }
        public override string Execute(DictionaryStack stack)
        {

            var arg = args[0].Execute(stack);

            if (!stack.ContainsKey(arg))
                throw new Exception($"No such paramater {arg}");
            return stack.Get(arg);
        }
    }
}
