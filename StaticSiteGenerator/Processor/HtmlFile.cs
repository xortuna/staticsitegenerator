using StaticSiteGenerator.Engine;

namespace StaticSiteGenerator.Processor
{
    public static class HtmlFile
    {
        internal static void Process(FileInfo fileInfo, DirectoryInfo outputDir, DictionaryStack stack)
        {
            stack.Push();
            stack.Add("input.fullname", fileInfo.FullName);
            stack.Add("input.name", fileInfo.Name);
            stack.Add("input.path", fileInfo.FullName.Substring(Program._rootDirectory.FullName.Length).Replace('\\', '/'));

            var target = new FileInfo(Path.Combine(outputDir.FullName, fileInfo.Name));
            stack.Add("output.fullname", target.FullName);
            stack.Add("output.path", target.FullName.Substring(Program._rootDirectory.FullName.Length).Replace('\\', '/'));
            stack.Add("output.url", target.FullName.Substring(Program._rootOutputDirectory.FullName.Length).Replace('\\', '/'));
            stack.Add("output.fullurl", stack.Get("root.url") + stack.Get("output.url"));

            Console.WriteLine($"\tGenerating {stack.Get("output.path")}");
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
                            var func = TokenParser.CompileCodeBlock(element.Content);
                            try
                            {
                                stack.Push();
                                foreach (var token in func)
                                {
                                    sw.Write(token.Execute(stack));
                                }
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
                                    Console.WriteLine($"\t\t{e.Key}: {e.Value}");
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
