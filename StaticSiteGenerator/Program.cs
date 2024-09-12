using StaticSiteGenerator;
using StaticSiteGenerator.Engine;
using StaticSiteGenerator.Tokens.Functions;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
internal class Program
{
    public static List<string> _assetFileTypes = new List<string>() { ".css", ".png", ".svg", ".js" };
    public static DirectoryInfo _rootDirectory = null;
    public static DirectoryInfo _currentDirectory = null;
    public static Dictionary<string, string> _partial_list;
    private static void Main(string[] args)
    {
        _rootDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        _partial_list = new Dictionary<string, string>();
        
        //Find Special folders
        ProcessSpecialFolders();

        DictionaryStack stack = new DictionaryStack();
        stack.Push();
        stack.Add("root.fullpath", _rootDirectory.FullName);
        var output = _rootDirectory.CreateSubdirectory("_www");
        ProcessDirectory(_rootDirectory, output, stack);
        stack.Pop();
    }

    private static void ProcessSpecialFolders()
    {
        foreach (var item in _rootDirectory.EnumerateDirectories())
        {
            if (item.Name == "_www" || item.Attributes.HasFlag(FileAttributes.Hidden)) continue;

            if (item.Name == "_partial")
            {
                foreach (var partial in item.GetFiles())
                {
                    _partial_list.Add(partial.Name, partial.FullName);
                }
                Console.WriteLine($"Found {_partial_list.Count} partial");
                continue;
            }
        }
        if (Directory.Exists(Path.Join(_rootDirectory.FullName, "_www")))
        {
            Directory.Delete(Path.Join(_rootDirectory.FullName, "_www"), true);
        }
    }
    private static void ProcessDirectory(DirectoryInfo directory, DirectoryInfo output, DictionaryStack stack)
    {
        _currentDirectory = directory;
        stack.Add("directory.fullname", directory.FullName);
        stack.Add("directory.path", directory.FullName.Substring(_rootDirectory.FullName.Length).Replace('\\', '/'));
        stack.Add("directory.name", directory.Name);

        Console.WriteLine($"-- {directory.FullName}");
        //Special files
        if (Path.Exists(Path.Join(directory.FullName, "__template.html")))
        {
            //Load template
            var template = TemplateFileParser.ProcessFile(new FileInfo(Path.Join(directory.FullName, "__template.html")));

            //Process MD files using template
            foreach (var item in directory.EnumerateFiles("*.md"))
            {
                ProcessMarkdownFile(template, item, output, stack);
            }
        }

        //HTML Files
        foreach (var item in directory.EnumerateFiles("*.html"))
        {
            if (item.Name.StartsWith("__")) continue;
            ProcessHtmlFile(item, output, stack);
        }

        //CONTENT
        foreach (var item in directory.EnumerateFiles().Where(r=> _assetFileTypes.Contains(r.Extension)))
        {
            ProcessAsset(item, output);
        }


        //SUB FOLDERS
        foreach (var childDir in directory.EnumerateDirectories())
        {
            if(childDir.Name.StartsWith("_") || childDir.Attributes.HasFlag(FileAttributes.Hidden)) continue;

            var subOutput = new DirectoryInfo(Path.Join(output.FullName, childDir.Name));
            if (!subOutput.Exists)
            {
                subOutput.Create();
            }
            stack.Push();
                ProcessDirectory(childDir, subOutput, stack);
            stack.Pop();
        }
    }

    private static void ProcessAsset(FileInfo item, DirectoryInfo output)
    {
        Console.WriteLine($"\tCopying {item.Name}");
        item.CopyTo(Path.Join(output.FullName, item.Name));
    }
    private static void ProcessMarkdownFile(IEnumerable<TemplateElement> template, FileInfo fileInfo, DirectoryInfo outputDir, DictionaryStack stack)
    {
        stack.Push();
        stack.Add("page.fullname", fileInfo.FullName);
        stack.Add("page.name", fileInfo.Name);
        stack.Add("page.path", fileInfo.FullName.Substring(_rootDirectory.FullName.Length).Replace('\\', '/'));

        var templateOutputDir = new DirectoryInfo(Path.Join(outputDir.FullName, GetUrl.Decamel(fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length))));
        if (!templateOutputDir.Exists)
        {
            templateOutputDir.Create();
        }
        

        var target = new FileInfo(Path.Combine(templateOutputDir.FullName, "index.html"));
        stack.Add("output.fullname", target.FullName);
        
        Console.WriteLine($"\tGenerating {stack.Get("page.path")}");

        StringBuilder sb = new StringBuilder();
        foreach (var element in TemplateFileParser.ProcessFile(fileInfo))
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


    private static void ProcessHtmlFile(FileInfo fileInfo, DirectoryInfo outputDir, DictionaryStack stack)
    {
        stack.Push();
        stack.Add("page.fullname", fileInfo.FullName);
        stack.Add("page.name", fileInfo.Name);
        stack.Add("page.path", fileInfo.FullName.Substring(_rootDirectory.FullName.Length).Replace('\\','/'));

        var target = new FileInfo(Path.Combine(outputDir.FullName, fileInfo.Name));
        stack.Add("output.fullname", target.FullName);

        Console.WriteLine($"\tGenerating {stack.Get("page.path")}");
        using (var sw = new StreamWriter(target.FullName))
        {
            foreach (var element in TemplateFileParser.ProcessFile(fileInfo))
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
                        catch (Exception ex) { 
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
