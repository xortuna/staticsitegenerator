﻿using System.Text;
using StaticSiteGenerator.Engine;

namespace StaticSiteGenerator.Tokens.Functions
{
    internal class DoesNotEqual : FunctionToken
    {
        public DoesNotEqual(List<Token> args) : base(args) {
            if (args.Count != 2) throw new ArgumentException("Invalid arguments for DoesNotEqual, expected 2 (a, b)");
        }
        public override string Execute(DictionaryStack stack)
        {
            var lh = args[0].Execute(stack);
            var rh = args[1].Execute(stack);
            if (lh != rh)
                return "true";
            else
                return "false";
        }
    }
}
