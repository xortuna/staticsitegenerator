using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticSiteGenerator.Tools
{
    public static class PathTools
    {
        public static string ToUrlPath(this string relativePath)
        {
            return relativePath.Replace("\\", "/");
        }

        public static string GetRelativePath(DirectoryInfo root, FileInfo path)
        {
            return path.FullName.Substring(root.FullName.Length);
        }

        public static string GetRelativePath(DirectoryInfo root, DirectoryInfo path)
        {
            return path.FullName.Substring(root.FullName.Length);
        }


        public static string MinifyUrl(string url)
        {
            if (url.EndsWith("index.html"))
                url = url.Substring(0, url.Length - "index.html".Length);
            return url;
        }

        public static string Decamel(string camel)
        {
            var sb = new StringBuilder();
            foreach (var c in camel)
            {
                if (c >= 65 && c <= 90)
                {
                    if (sb.Length != 0)
                        sb.Append("_");
                    sb.Append((char)(c + 32));
                }
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

    }
}
