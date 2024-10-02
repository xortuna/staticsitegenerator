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

            target.CreationTime = fileInfo.LastWriteTime;
            stack.Pop();
        }
    }
}
