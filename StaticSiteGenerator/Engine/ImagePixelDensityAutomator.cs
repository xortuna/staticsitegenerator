using StaticSiteGenerator.Tools;
using System.Text.RegularExpressions;

namespace StaticSiteGenerator.Engine
{
    public class ImagePixelDensityAutomator
    {
        private DirectoryInfo _rootPath;
        private bool _upgradeToWebP;

        //[Classes to append] [AltText] (MainImgSrc)
        private Regex _2ParamImg = new Regex(@"(?:\!)\[([^\]]+)\] \(([^\)]+)\)",
  RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);
        private Regex _3ParamImg = new Regex(@"(?:\!)\[([^\]]+)\] \[([^\]]+)\] \(([^\)]+)\)",
        RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

        public ImagePixelDensityAutomator(DirectoryInfo rootPath, bool upgradeToWebP)
        {
            _rootPath = rootPath;
            _upgradeToWebP = upgradeToWebP;
        }

        public string CompileText(string markdownContents)
        {
            List<KeyValuePair<string, string>> replacementQueue = new List<KeyValuePair<string, string>>();
            foreach (Match m in _2ParamImg.Matches(markdownContents))
            {
                if (replacementQueue.Any(r => r.Key == m.Value))
                    continue;
                var imageSrc = m.Groups[2].Value;
                ProcessImage(imageSrc, m.Value, replacementQueue);
            }

            foreach (Match m in _3ParamImg.Matches(markdownContents))
            {
                if (replacementQueue.Any(r => r.Key == m.Value))
                    continue;
                var imageSrc = m.Groups[3].Value;
                ProcessImage(imageSrc, m.Value, replacementQueue);
            }

            foreach(var replacement in replacementQueue)
            {
                markdownContents = markdownContents.Replace(replacement.Key, replacement.Value);
            }
            return markdownContents;
        }

        private void ProcessImage(string imageSrc,  string inputLine, List<KeyValuePair<string, string>> replacementQueue)
        {
            if (imageSrc.StartsWith("/"))
            {
                var sourceFilePath = Path.Join(_rootPath.FullName, imageSrc.Replace("/", "\\"));
                var fi = new FileInfo(sourceFilePath);
                if (fi.Exists)
                {
                    FileInfo oneXImage = fi;
                    FileInfo? twoXImage = null;
                    //Look around for 2x's
                    var extension = fi.Extension;
                    var webM2xImage = new FileInfo(Path.Combine(fi.Directory.FullName, fi.Name.Substring(0, fi.Name.Length - extension.Length) + "-2x" + ".webp"));
                    if (webM2xImage.Exists) {
                        twoXImage = webM2xImage;
                    }
                    else
                    {
                        var multiRez = new FileInfo(Path.Combine(fi.Directory.FullName, fi.Name.Substring(0, fi.Name.Length - extension.Length) + "-2x" + extension));
                        if (multiRez.Exists)
                        {
                            twoXImage = multiRez;
                        }
                    }
                    if(_upgradeToWebP && fi.Extension != ".webp")
                    {
                        var webM = new FileInfo(Path.Combine(fi.Directory.FullName, fi.Name.Substring(0, fi.Name.Length - extension.Length) + ".webp"));
                        if (webM.Exists)
                        {
                            oneXImage = webM;
                        }
                    }

                    if(twoXImage != null)
                    {
                        var x1 = Tools.PathTools.GetRelativePath(_rootPath, oneXImage).ToUrlPath();
                        var x2 = Tools.PathTools.GetRelativePath(_rootPath, twoXImage).ToUrlPath();
                        //Replace string in markdown
                        replacementQueue.Add(new KeyValuePair<string, string>(inputLine, inputLine + $"({x1} 1x, {x2} 2x)"));
                    }
                    if (oneXImage != fi)
                    {
                        var x1 = Tools.PathTools.GetRelativePath(_rootPath, oneXImage).ToUrlPath();
                        //Replace string in markdown
                        replacementQueue.Add(new KeyValuePair<string, string>(inputLine, inputLine + $"({x1} 1x)"));
                    }
                    //TODO WEBM Fallback
                }
            }
        }
    }
}
