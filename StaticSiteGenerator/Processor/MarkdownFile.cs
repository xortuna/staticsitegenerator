using StaticSiteGenerator.Engine;
using StaticSiteGenerator.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticSiteGenerator.Processor
{
    internal static class MarkdownFile
    {
        internal static void Process(FileInfo fileInfo, DirectoryInfo outputDir, DictionaryStack stack, IEnumerable<TemplateToken> template)
        {
            stack.Push();
            stack.Add("input.fullname", fileInfo.FullName);
            stack.Add("input.name", fileInfo.Name);
            stack.Add("input.path", fileInfo.FullName.Substring(Program._rootDirectory.FullName.Length).Replace('\\', '/'));
            stack.Add("input.createdon", fileInfo.CreationTimeUtc.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            stack.Add("input.modifiedon", fileInfo.LastWriteTimeUtc.ToString("yyyy-MM-ddTHH:mm:ssZ"));

            var templateOutputDir = new DirectoryInfo(Path.Join(outputDir.FullName, PathTools.Decamel(fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length))));
            if (!templateOutputDir.Exists)
            {
                templateOutputDir.Create();
            }


            var target = new FileInfo(Path.Combine(templateOutputDir.FullName, "index.html"));
            stack.Add("output.fullname", target.FullName);
            stack.Add("output.path", target.FullName.Substring(Program._rootDirectory.FullName.Length).Replace('\\', '/'));
            stack.Add("output.url", target.FullName.Substring(Program._rootOutputDirectory.FullName.Length).Replace('\\', '/'));
            stack.Add("output.fullurl", stack.Get("root.url") + stack.Get("output.url"));

            Console.WriteLine($"\tGenerating {stack.Get("output.path")}");

            StringBuilder sb = new StringBuilder();
            foreach (var element in TemplateTokenizer.ProcessFile(fileInfo))
            {
                switch (element.Type)
                {
                    case TemplateType.Content:
                        sb.Append(element.Content);
                        break;
                    case TemplateType.Metadata:
                        var md = MetaDataParser.Parse(element);
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($"\t\tMetadata set {md.Key} - {md.Value}");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        stack.Add(md.Key, md.Value);
                        break;
                }
            }
            stack.Add("content", MarkdownParser.CompileText(sb.ToString()));

            using (var sw = new StreamWriter(target.FullName))
            {
                foreach (var element in template)
                {
                    switch (element.Type)
                    {
                        case TemplateType.Content:
                            sw.Write(element.Content);
                            break;
                        case TemplateType.Token:
                            var func = TokenParser.CompileCodeBlock(element.Content);
                            try
                            {
                                stack.Push();
                                foreach(var token in func)
                                    sw.Write(token.Execute(stack));
                                stack.Pop();
                            }
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"\t\tTheres an error in {target.FullName}: on line {element.Line} with:\n{element.Content}");
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.WriteLine($"\t\t{ex.Message.ToString()}");
                                if (ex.InnerException != null)
                                    Console.WriteLine(ex.InnerException.Message.ToString());

                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("\t\tCurrent variables");
                                foreach (var e in stack.GetCurrentElements())
                                {
                                    Console.WriteLine($"\t\t{e.Key}: {(e.Value.Length > 100 ? $"{e.Value.Substring(0,50)}..." : e.Value)}");
                                }
                                Console.ForegroundColor = ConsoleColor.Gray;
                                stack.Pop();
                            }
                            break;
                        case TemplateType.Metadata:
                            var md = MetaDataParser.Parse(element);
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"\t\tMetadata set {md.Key} - {md.Value}");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            stack.Add(md.Key, md.Value);
                            break;
                    }
                }
            }
            target.CreationTime = fileInfo.LastWriteTime;
            stack.Pop();

        }
    }
}
