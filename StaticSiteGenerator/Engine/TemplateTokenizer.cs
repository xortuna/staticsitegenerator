using System.Text;

namespace StaticSiteGenerator.Engine
{
    public enum TemplateType
    {
        Content,
        Token,
        Metadata
    }
    public struct TemplateToken
    {
        public TemplateType Type;
        public string Content;
        public int Line;
    }

    public static class TemplateTokenizer
    {
        public static IEnumerable<TemplateToken> ProcessFile(FileInfo fi)
        {
            return ProcessStream(fi.OpenRead());   
        }
  
        public static IEnumerable<TemplateToken> ProcessStream(Stream stream)
        {
            string text = string.Empty;
            using (StreamReader sr = new StreamReader(stream))
            {
                text = sr.ReadToEnd();
            }
            return ProcessText(text);
        }

        enum ReadState
        {
            Content,
            PreEscape,
            MetaData,
            PostMetaData,
            Escape,
            PostEscape,
        }
        public static IEnumerable<TemplateToken> ProcessText(string text)
        {
            StringBuilder output = new StringBuilder();
            ReadState mode = ReadState.Content;
            int lineCount = 0;
            int tokenStartLine = lineCount;
            foreach (var c in text)
            {
                if (c == '\n')
                    lineCount++;
                switch (mode)
                {
                    case ReadState.Content:
                        if (c == '{')
                        {
                            mode = ReadState.PreEscape;
                        }
                        else
                        {
                            output.Append(c);
                        }
                        break;
                    case ReadState.PreEscape:
                        if (c == '{' || c == '#')
                        {
                            if (output.Length > 0)
                            {
                                yield return new TemplateToken { Type = TemplateType.Content, Line = tokenStartLine, Content = output.ToString() };
                                output.Clear();
                            }
                            mode = c == '{' ? ReadState.Escape : ReadState.MetaData;
                            tokenStartLine = lineCount;
                        }
                        else
                        {
                            output.Append('{');
                            output.Append(c);
                            mode = ReadState.Content;
                        }
                        break;
                    case ReadState.MetaData:
                        if (c == '#')
                            mode = ReadState.PostMetaData;
                        else
                            output.Append(c);
                        break;
                    case ReadState.PostMetaData:
                        if (c == '}')
                        {
                            yield return new TemplateToken { Type = TemplateType.Metadata, Line = tokenStartLine, Content = output.ToString() };
                            mode = ReadState.Content;
                            tokenStartLine = lineCount;
                            output.Clear();
                        }
                        else
                        {
                            output.Append('}');
                            output.Append(c);
                            mode = ReadState.MetaData;
                        }
                        break;
                    case ReadState.Escape:
                        if (c == '}')
                        {
                            mode = ReadState.PostEscape;
                        }
                        else
                        {
                            output.Append(c);
                        }
                        break;
                    case ReadState.PostEscape:
                        if (c == '}')
                        {
                            yield return new TemplateToken { Type = TemplateType.Token, Line = tokenStartLine, Content = output.ToString() };
                            mode = ReadState.Content;
                            tokenStartLine = lineCount;
                            output.Clear();
                        }
                        else
                        {
                            output.Append('}');
                            output.Append(c);
                            mode = ReadState.Escape;
                        }
                        break;
                }
            }
            if (mode != ReadState.Content)
                throw new Exception($"Expected }} charater at end of file");

            yield return new TemplateToken { Type = TemplateType.Content, Line = lineCount, Content = output.ToString() };
        }
    }
}
