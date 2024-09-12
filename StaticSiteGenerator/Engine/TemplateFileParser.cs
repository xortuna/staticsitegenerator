using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticSiteGenerator.Engine
{
    enum TemplateType
    {
        Content,
        Token,
        Metadata
    }
    struct TemplateElement
    {
        public TemplateType Type;
        public string Content;
        public int Line;
    }


    internal static class TemplateFileParser
    {

        enum ReadState
        {
            Content,
            PreEscape,
            MetaData,
            PostMetaData,
            Escape,
            PostEscape,
        }

        public static IEnumerable<TemplateElement> ProcessFile(FileInfo fi)
        {
            StringBuilder output = new StringBuilder();
            ReadState mode = ReadState.Content;
            int lineCount = 0;
            using (StreamReader sr = new StreamReader(fi.FullName))
            {
                string? line = sr.ReadToEnd();
                foreach (var c in line)
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
                                mode = c == '{' ? ReadState.Escape : ReadState.MetaData;
                                yield return new TemplateElement { Type = TemplateType.Content, Line = lineCount, Content = output.ToString() };
                                output.Clear();
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
                                mode = ReadState.Content;
                                yield return new TemplateElement { Type = TemplateType.Metadata, Line = lineCount, Content = output.ToString() };
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
                                mode = ReadState.Content;
                                yield return new TemplateElement { Type = TemplateType.Token, Line = lineCount, Content = output.ToString() };
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
                    throw new Exception($"{fi.Name}: Expected }} charater at end of file");

                yield return new TemplateElement { Type = TemplateType.Content, Line = lineCount, Content = output.ToString() };
            }
        }
    }
}
