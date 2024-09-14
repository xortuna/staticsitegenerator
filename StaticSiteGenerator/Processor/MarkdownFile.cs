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
            stack.Add("page.fullname", fileInfo.FullName);
            stack.Add("page.name", fileInfo.Name);
            stack.Add("page.path", fileInfo.FullName.Substring(Program._rootDirectory.FullName.Length).Replace('\\', '/'));

            var templateOutputDir = new DirectoryInfo(Path.Join(outputDir.FullName, PathTools.Decamel(fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length))));
            if (!templateOutputDir.Exists)
            {
                templateOutputDir.Create();
            }


            var target = new FileInfo(Path.Combine(templateOutputDir.FullName, "index.html"));
            stack.Add("output.fullname", target.FullName);

            Console.WriteLine($"\tGenerating {stack.Get("page.path")}");

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
                            var func = TokenParser.Compile(element.Content);
                            try
                            {
                                stack.Push();
                                sw.Write(func.Execute(stack));
                                stack.Pop();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                                Console.WriteLine($"\t\tat {target.FullName}:line {element.Line} {element.Content}");
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
            stack.Pop();

        }
    }
}
