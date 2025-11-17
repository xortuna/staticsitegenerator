using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StaticSiteGenerator.Tools;

namespace StaticSiteGenerator.Engine
{
    static class SitemapGenerator
    {
        public static void GenerateMap(string publicUrl, DirectoryInfo output)
        {
            Console.WriteLine("Generating Site Map...");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
            ProcessDirectory(publicUrl, output, output, sb);
            sb.AppendLine("</urlset>");

            using (StreamWriter sr = new StreamWriter(Path.Join(output.FullName, "sitemap.xml")))
            {
                sr.WriteLine(sb.ToString());
            }
            using (StreamWriter sr = new StreamWriter(Path.Join(output.FullName, "robots.txt")))
            {
                sr.WriteLine("User-agent: *");
                sr.WriteLine("Allow: /");
                sr.WriteLine($"Sitemap: {publicUrl}/sitemap.xml");
            }
        }

        internal static void ProcessDirectory(string publicUrl, DirectoryInfo root, DirectoryInfo output, StringBuilder sb)
        {

            foreach (var file in output.EnumerateFiles("*.html"))
            {
                sb.Append("<url><loc>");
                sb.Append(publicUrl);
                sb.Append(PathTools.MinifyUrl(PathTools.GetRelativePath(root, file).ToUrlPath()));
                sb.Append("</loc>");
                sb.Append($"<lastmod>{file.LastWriteTime.ToString("yyyy-MM-dd")}</lastmod>");
                sb.AppendLine("</url>");
            }
            foreach (var dir in output.EnumerateDirectories().Where(d => !d.Name.StartsWith("_") || d.Attributes == FileAttributes.Hidden))
            {
                ProcessDirectory(publicUrl, root, dir, sb);
            }
        }
    }
}
