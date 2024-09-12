using StaticSiteGenerator.Tokens.Functions;
using StaticSiteGenerator.Tokens.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticSiteGenerator.Engine
{

    abstract class Token()
    {
        public abstract string Execute(DictionaryStack stack);
    }

    abstract class FunctionToken : Token
    {
        protected List<Token> args;
        public FunctionToken(List<Token> args) { this.args = args; }
    }



    internal static class TokenParser
    {
        enum ParserState
        {
            FunctionName,
            ParameterAnalisis,
            StringLitral,
            SubFunction,
            EndParamater,
            Operator,
            End
        }
        public static Token Compile(string escape)
        {
            string functionName = "invalid";
            List<Token> parameters = new List<Token>();
            StringBuilder sb = new StringBuilder();
            ParserState state = ParserState.FunctionName;
            double depth = 0;
            bool escaped = false;
            foreach (var c in escape)
            {
                switch (state)
                {
                    case ParserState.FunctionName:
                        if (c == ' ')
                            continue;
                        if (c == '(')
                        {
                            state = ParserState.ParameterAnalisis;
                            functionName = sb.ToString();
                            sb.Clear();
                        }
                        else
                            sb.Append(c);
                        break;
                    case ParserState.ParameterAnalisis:
                        if (c == ' ')
                            continue;
                        if (c == '\'')
                        {
                            if (sb.Length != 0)
                                throw new Exception($"Unexpected charaters {sb} before string litral");
                            state = ParserState.StringLitral;
                            continue;
                        }
                        if (c == ')' || c == ',')
                        {
                            //Litral detection
                            if (sb.ToString() == "true") { }

                            parameters.Add(new NullToken());
                            state = c == ')' ? ParserState.End : ParserState.ParameterAnalisis;
                            sb.Clear();
                            continue;
                        }
                        else if (c == '(')
                        {
                            state = ParserState.SubFunction;
                            depth++;
                        }
                        sb.Append(c);
                        break;
                    case ParserState.StringLitral:
                        if (c == '\'')
                        {
                            parameters.Add(new StringLiteralToken(sb.ToString()));
                            sb.Clear();
                            state = ParserState.EndParamater;
                            break;
                        }
                        sb.Append(c);
                        break;
                    case ParserState.EndParamater:
                        if (c == ')')
                        {
                            state = ParserState.End;
                        }
                        if (c == ',')
                        {
                            state = ParserState.ParameterAnalisis;
                        }
                        break;
                    case ParserState.SubFunction:
                        sb.Append(c);
                        if (c == ')')
                        {
                            depth--;
                            if (depth == 0)
                            {
                                parameters.Add(Compile(sb.ToString()));
                                sb.Clear();
                                state = ParserState.EndParamater;
                            }
                        }
                        else if (c == '(')
                            depth++;

                        break;
                    case ParserState.End:
                        if (c != ' ')
                            throw new Exception($"Unexpected charater {c}");
                        break;
                }
            }

            return ResolveFunction(functionName, parameters);
        }

        private static Token ResolveFunction(string functionName, List<Token> parameters)
        {
            Token function;
            switch (functionName.ToLower())
            {
                case "include":
                    function = new Include(parameters);
                    break;
                case "var":
                    function = new Var(parameters);
                    break;
                case "assign":
                    function = new Assign(parameters);
                    break;
                case "equals":
                    function = new Equals(parameters);
                    break;
                case "foreach":
                    function = new Foreach(parameters);
                    break;
                case "load_metadata":
                    function = new LoadMetaData(parameters);
                    break;
                case "list_files":
                    function = new ListFiles(parameters);
                    break;
                case "get_url":
                    function = new GetUrl(parameters);
                    break;
                case "if":
                    function = new If(parameters);
                    break;
                case "starts_with":
                    function = new StartsWith(parameters);
                    break;
                default: throw new Exception($"Invalid Function {functionName}");
            }

            return function;
        }
    }
}
