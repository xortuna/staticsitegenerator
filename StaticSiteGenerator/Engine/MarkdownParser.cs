using HeyRed.MarkdownSharp;
using StaticSiteGenerator.Engine.MarkdownExtensions;
namespace StaticSiteGenerator.Engine
{
    internal static class MarkdownParser
    {
        static Markdown _markdownParser = new Markdown();
        static ImagePixelDensityAutomator? _imageOptimiserProcess;

        public static void Configure(DirectoryInfo root, MarkdownConfig config)
        {
            if(config.AutoDetectMutliResolutionImages)
            {
                _imageOptimiserProcess = new ImagePixelDensityAutomator(root, true);
            }
            if (config.EnablePlugins)
            {
                _markdownParser.AddExtension(new Small());
                _markdownParser.AddExtension(new Achivement());
                _markdownParser.AddExtension(new DivWrappedImage());
            }
        }

        public static string CompileText(string markdown)
        {
            if(_imageOptimiserProcess != null)
            {
                markdown = _imageOptimiserProcess.CompileText(markdown);
            }
            return _markdownParser.Transform(markdown);
        }
    }
}
