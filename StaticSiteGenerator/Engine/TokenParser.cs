using StaticSiteGenerator.Tokens.Functions;
using StaticSiteGenerator.Tokens.Types;
using System.Reflection;
using System.Runtime;
using System.Text;

namespace StaticSiteGenerator.Engine
{

    public abstract class Token()
    {
        public abstract string Execute(DictionaryStack stack);
    }

    abstract class FunctionToken : Token
    {
        protected abstract string Identifier { get; }

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
        enum ParserState2
        {
            DetectStart, 
            ReadParamaters,
            StringLitral,
            LinqFunction,
            CheckForOperator,
            HandleOperator,
        }
        enum MultiLineState 
        {
            DetectStart,
            Recording,
            StringLitral
        }
        public static Token Compile(string escape)
        {
            return CompileV2(escape, out _);
        }


        public static IEnumerable<Token> CompileCodeBlock(string token)
        {
            //Pull out complete tokens from a token set;
            int i = 0;
            MultiLineState state = MultiLineState.DetectStart;
            StringBuilder sb = new StringBuilder();
            for (; i < token.Length; ++i)
            {
                char c = token[i];
                switch (state)
                {
                    case MultiLineState.DetectStart:
                        if (c == ' ' || c == '\n' || c == '\r')
                            continue; //Whitespace stripping
                        if (c == '\'')
                        {
                            state = MultiLineState.StringLitral;
                        }
                        if (c == ';')
                        {
                            yield return CompileV2(sb.ToString(), out _);
                            sb.Clear();
                            continue;
                        }
                        sb.Append(c);
                        state = MultiLineState.Recording;
                        break;
                    case MultiLineState.Recording:
                        if (c == '\'')
                        {
                            state = MultiLineState.StringLitral;
                        }
                        if (c == ';')
                        {
                            yield return CompileV2(sb.ToString(), out _);
                            sb.Clear();
                            state = MultiLineState.DetectStart;
                            continue;
                        }
                        sb.Append(c);
                        break;
                    case MultiLineState.StringLitral:
                        if (c == '\'')
                        {
                            state = MultiLineState.DetectStart;
                        }
                        sb.Append(c);
                        break;
                }
            }
            if(sb.Length >0)
                yield return CompileV2(sb.ToString(), out _);
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
                case '=': return "equal";
                default:
                    return "null";
            }
        }
        private static Dictionary<string, Type> _tokenTypes = new Dictionary<string, Type>();

        private static void InitaliseTokenTypes()
        {
            foreach(var t in Assembly.GetExecutingAssembly().GetTypes().Where(r=> Attribute.IsDefined(r, typeof(FunctionTokenAttribute)) && typeof(FunctionToken).IsAssignableFrom(r)))
            {
                var ft = t.GetCustomAttribute<FunctionTokenAttribute>();
                _tokenTypes.Add(ft.Name, t);
            }
        }
        private static Token ResolveFunction(string functionName, List<Token> parameters)
        {
            if (_tokenTypes.Count == 0)
                InitaliseTokenTypes();

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

                default:
                    if (!_tokenTypes.ContainsKey(functionName.ToLower()))
                        throw new Exception($"Invalid Function {functionName}");
                    var ft = _tokenTypes[functionName.ToLower()].GetCustomAttribute<FunctionTokenAttribute>();
                    if(parameters.Count < ft.MinArgs || parameters.Count > ft.MaxArgs)
                    {
                        throw new Exception($"Invalid arguments for {functionName} - Expected {(ft.MinArgs != ft.MaxArgs ? ft.MinArgs + " to " + ft.MaxArgs : ft.MinArgs )}] got {parameters.Count}");
                    }
                    return (FunctionToken)Activator.CreateInstance(_tokenTypes[functionName.ToLower()], parameters);
            }
        }
    }
}
