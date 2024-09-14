using StaticSiteGenerator.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticSiteGenerator.Processor
{
    public static class HtmlFile
    {
        internal static void Process(FileInfo fileInfo, DirectoryInfo outputDir, DictionaryStack stack)
        {
            stack.Push();
            stack.Add("page.fullname", fileInfo.FullName);
            stack.Add("page.name", fileInfo.Name);
            stack.Add("page.path", fileInfo.FullName.Substring(Program._rootDirectory.FullName.Length).Replace('\\', '/'));

            var target = new FileInfo(Path.Combine(outputDir.FullName, fileInfo.Name));
            stack.Add("output.fullname", target.FullName);

            Console.WriteLine($"\tGenerating {stack.Get("page.path")}");
            using (var sw = new StreamWriter(target.FullName))
            {
                foreach (var element in TemplateTokenizer.ProcessFile(fileInfo))
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
