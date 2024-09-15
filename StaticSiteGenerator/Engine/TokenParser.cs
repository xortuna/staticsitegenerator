using StaticSiteGenerator.Tokens.Functions;
using StaticSiteGenerator.Tokens.Types;
using System.Text;

namespace StaticSiteGenerator.Engine
{

    public abstract class Token()
    {
        public abstract string Execute(DictionaryStack stack);
    }

    abstract class FunctionToken : Token
    {
        protected List<Token> args;
        public FunctionToken(List<Token> args) { this.args = args; }
    }

    public static class TokenParser
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
            return CompileV2(escape, out int crap);
        }
        enum ParserState2
        {
            DetectStart, 
            ReadParamaters,
            StringLitral,
            LinqFunction,
            CheckForOperator,
            HandleOperator,
        }
        public static Token CompileV2(string token,  out int parsedLength, List<Token>? preInject = null)
        {
            string functionName = "invalid";
            List<Token> parameters = new List<Token>(preInject ?? Enumerable.Empty<Token>());
            StringBuilder sb = new StringBuilder();
            ParserState2 state = ParserState2.DetectStart;
            int i = 0;
            for(; i < token.Length; ++i)
            {
                char c = token[i];
                switch (state)
                {
                    case ParserState2.DetectStart:
                        if (c == ' ' || c == '\n' || c == '\r')
                            continue;
                        if (c == '(')
                        {
                            state = ParserState2.ReadParamaters;
                            functionName = sb.ToString();
                            if(functionName == "")
                            {
                                functionName = "bracket";
                            }
                            sb.Clear();
                        }
                        else if(c == '\'')
                        {
                            state = ParserState2.StringLitral;
                            sb.Clear();
                        }
                        else if (IsOperator(c))
                        {
                            if (sb.ToString().All(char.IsDigit))
                            {
                                var val = int.Parse(sb.ToString());
                                parameters.Add(new IntLiteralToken(val));
                                functionName = GetOperatorFunction(c);
                                state = ParserState2.HandleOperator;
                                continue;
                            }

                        }
                        else if(c == ',' || c == ')')
                        {
                            parsedLength = i -1;
                            //detect litral value
                            if(sb.Length == 0)
                                return new NullToken();
                            if (sb.ToString().All(char.IsDigit))
                            {
                                var val = int.Parse(sb.ToString());
                                return new IntLiteralToken(val);
                            }
                            if(sb.Equals("true"))
                            {
                                return new StringLiteralToken("true");
                            }
                            return new NullToken();
                        }
                        else
                            sb.Append(c);
                        break;
                    case ParserState2.ReadParamaters:
                        if (c == ',')
                            continue;
                        if (c == ')')
                        {
                            state = ParserState2.CheckForOperator;
                            continue;
                        }

                        var param = CompileV2(token.Substring(i), out parsedLength);
                        parameters.Add(param);
                        i = i + parsedLength;
                        break;
                    case ParserState2.StringLitral:
                        if (c == '\'')
                        {
                            functionName = "string";
                            parameters.Add(new StringLiteralToken(sb.ToString()));

                            state = ParserState2.CheckForOperator;
                            continue;
                        }
                        sb.Append(c);
                        break;
                    case ParserState2.CheckForOperator:
                        //Peek
                        if (c == '.')
                        {
                            state = ParserState2.LinqFunction;
                            continue;
                        }
                        if (IsOperator(c))
                        {
                            //Swap the paramater order so the linq function runs first, and this is sent as a paramater
                            var lhs = ResolveFunction(functionName, parameters);
                            parameters = new List<Token>();
                            parameters.Add(lhs);
                            functionName = GetOperatorFunction(c);
                            state = ParserState2.HandleOperator;
                            continue;
                        }
                        else 
                            i--;
                        goto end;
                    case ParserState2.HandleOperator:
                        var rhs = CompileV2(token.Substring(i), out parsedLength);
                        parameters.Add(rhs);
                        goto end;
                    case ParserState2.LinqFunction:
                        {
                            //Swap the paramater order so the linq function runs first, and this is sent as a paramater
                            var previousToken = ResolveFunction(functionName, parameters);
                            var linqToken = CompileV2(token.Substring(i), out parsedLength, new List<Token> { previousToken });
                            parsedLength = parsedLength + i;
                            return linqToken;
                        }
                }
            }
            end:
            parsedLength = i;
            if(functionName == "invalid")
            {
                if (sb.ToString().All(char.IsDigit))
                {
                    var val = int.Parse(sb.ToString());
                    return new IntLiteralToken(val);
                }
                if (sb.Equals("true"))
                {
                    return new StringLiteralToken("true");
                }
                return new NullToken();
            }
            return ResolveFunction(functionName, parameters);
        }

        private static bool IsOperator(char c)
        {
            return GetOperatorFunction(c) != "null";
        }
        private static string GetOperatorFunction(char c)
        {
            switch (c)
            {
                case '+': return "add";
                case '-': return "subtract";
                case '/': return "divide";
                case '*': return "multiply";
                case '=': return "equals";
                default:
                    return "null";
            }
        }
        private static Token ResolveFunction(string functionName, List<Token> parameters)
        {
            switch (functionName.ToLower())
            {
                case "bracket":
                    return parameters[0];
                case "double":
                    return parameters[0];
                case "int":
                    return parameters[0];
                case "string":
                    return parameters[0];

                //Math
                case "add":
                    return new Add(parameters);
                case "subtract":
                    return new Subtract(parameters);
                case "divide":
                    return new Divide(parameters);
                case "multiply":
                    return new Multiply(parameters);

                //File manipulation
                case "include":
                    return new Include(parameters);
                case "get_url":
                    return new GetUrl(parameters);

                //String manipulation
                case "reverse":
                    return new Reverse(parameters);
                case "concat":
                    return new Concat(parameters);

                //Meta Data maipulation
                case "var":
                    return new Var(parameters);
                case "assign":
                    return new Assign(parameters);
                case "load_metadata":
                    return new LoadMetaData(parameters);

                //Conditionals
                case "equals":
                    return new Equals(parameters);
                case "notequal":
                case "notequals":
                    return new DoesNotEqual(parameters);
                case "if":
                    return new If(parameters);
                case "starts_with":
                    return new StartsWith(parameters);

                // Array generator
                case "to_array":
                    return new ToArray(parameters);
                case "list_files":
                    return new ListFiles(parameters);

                //Array functions
                case "join":
                    return new Join(parameters);
                case "foreach":
                    return new Foreach(parameters);
                case "where":
                    return new Where(parameters);
                case "skip":
                    return new Skip(parameters);
                case "take":
                    return new Take(parameters);
                case "shuffle":
                    return new Shuffle(parameters);
                default: throw new Exception($"Invalid Function {functionName}");
            }
        }
    }
}
