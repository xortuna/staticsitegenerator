using StaticSiteGenerator;
using StaticSiteGenerator.Engine;
using StaticSiteGenerator.Processor;
using StaticSiteGenerator.Tokens.Functions;
using StaticSiteGenerator.Tools;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

class Config
{
    public bool Watch = false;
    public List<string> AssetFileTypes = new List<string>() { ".css", ".png", ".svg", ".js" };
}

internal class Program
{
    public static Config _config = new Config();
    public static DirectoryInfo _rootDirectory = null;
    public static DirectoryInfo _currentDirectory = null;
    public static Dictionary<string, string> _partial_list;

    private static void Main(string[] args)
    {
        ProcessArgs(args);
        _rootDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        _partial_list = new Dictionary<string, string>();
        
        //Find Special folders
        ProcessSpecialFolders();

        DictionaryStack stack = new DictionaryStack();
        stack.Push();
            stack.Add("root.fullpath", _rootDirectory.FullName);
            var output = _rootDirectory.CreateSubdirectory("_www");
            stack.Push(); 
                ProcessDirectory(_rootDirectory, output, stack);
            stack.Pop();

            if (_config.Watch)
                StartWatch(_rootDirectory, output, stack);
        stack.Pop();

    }


    private static void ProcessArgs(string[] args)
    {
        for (int i = 0; i < args.Length; ++i)
        {
            var arg = args[i];
            switch (arg)
            {
                case "-w":
                    _config.Watch = true;
                    break;
                case "-t":
                    if (arg.Length <= i + 1) 
                        continue;
                    var toAdd = args[i + 1].Split(",");
                    _config.AssetFileTypes.AddRange(toAdd);
                    break;
                default:
                    Console.WriteLine($"Unknown argument {arg}");
                    break;
            }
        }
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

    private static void StartWatch(DirectoryInfo root, DirectoryInfo output, DictionaryStack stack)
    {
        FileSystemWatcher watcher = new FileSystemWatcher(root.FullName);
        watcher.NotifyFilter = NotifyFilters.LastWrite;
        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;
        watcher.Changed += (sender, e) =>
        {
            var currentFile = new FileInfo(e.FullPath);
            
            var directory = currentFile.Directory;
            var relativePath = PathTools.GetRelativePath(root, directory);
            if (currentFile.Extension == "" || relativePath.StartsWith("\\_") || relativePath.StartsWith("\\."))
                return;
            if (!_config.AssetFileTypes.Contains(currentFile.Extension) && currentFile.Extension != ".md" && currentFile.Extension != ".html")
                return;
            var relativeOut = new DirectoryInfo(Path.Join(output.FullName, relativePath));

            WaitForUnlock(currentFile);
            stack.Push();
            stack.Add("directory.fullname", directory.FullName);
            stack.Add("directory.path", relativePath.ToUrlPath());
            stack.Add("directory.name", directory.Name);

            if (currentFile.Name == "__template.html")
            {
                //Special files
                var template = TemplateTokenizer.ProcessFile(new FileInfo(Path.Join(directory.FullName, "__template.html")));

                //Process MD files using template
                foreach (var item in directory.EnumerateFiles("*.md"))
                {
                    MarkdownFile.Process(item, relativeOut, stack, template);
                }
            }
            else if(currentFile.Extension == ".md")
            {
                //Special files
                var template = TemplateTokenizer.ProcessFile(new FileInfo(Path.Join(directory.FullName, "__template.html")));

                //Process MD files using template
                MarkdownFile.Process(currentFile, relativeOut, stack, template);
            }
            else if (currentFile.Extension == ".html") { 
                    if (!relativeOut.Exists)
                        relativeOut.Create();
                    HtmlFile.Process(currentFile, relativeOut, stack);

            }
            else if (_config.AssetFileTypes.Contains(currentFile.Extension))
            {
                if (!relativeOut.Exists)
                    relativeOut.Create();
                AssetFile.Process(currentFile, relativeOut, stack);
            }

            stack.Pop();
        };
        Console.WriteLine("Watching directory... Press enter to exit");
        Console.ReadLine();
    }

    private static void WaitForUnlock(FileInfo currentFile)
    {
        long lastLength = -1;
        while (true)
        {
            if (lastLength == currentFile.Length)
            {
                try
                {
                    using (var r = currentFile.OpenRead())
                    {

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Waiting for {currentFile.Name} to unlock");
                }
                break;
            }
            lastLength = currentFile.Length;
            Thread.Sleep(150);
        }
    }

    private static void ProcessDirectory(DirectoryInfo directory, DirectoryInfo output, DictionaryStack stack)
    {
        _currentDirectory = directory;
        stack.Add("directory.fullname", directory.FullName);
        stack.Add("directory.path", PathTools.GetRelativePath(_rootDirectory, _currentDirectory).ToUrlPath());
        stack.Add("directory.name", directory.Name);

        Console.WriteLine($"-- {directory.FullName}");
        //Special files
        if (Path.Exists(Path.Join(directory.FullName, "__template.html")))
        {
            //Load template
            var template = TemplateTokenizer.ProcessFile(new FileInfo(Path.Join(directory.FullName, "__template.html")));

            //Process MD files using template
            foreach (var item in directory.EnumerateFiles("*.md"))
            {
                MarkdownFile.Process(item, output, stack, template);
            }
        }

        //HTML Files
        foreach (var item in directory.EnumerateFiles("*.html"))
        {
            if (item.Name.StartsWith("__")) continue;
            HtmlFile.Process(item, output, stack);
        }

        //CONTENT
        foreach (var item in directory.EnumerateFiles().Where(r=> _config.AssetFileTypes.Contains(r.Extension)))
        {
            AssetFile.Process(item, output, stack);
        }


        //SUB FOLDERS
        foreach (var childDir in directory.EnumerateDirectories())
        {
            if(childDir.Name.StartsWith("_") || childDir.Name.StartsWith(".")  || childDir.Attributes.HasFlag(FileAttributes.Hidden)) continue;

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

}
