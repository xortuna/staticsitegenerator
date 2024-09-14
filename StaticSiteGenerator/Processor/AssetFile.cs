using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticSiteGenerator.Processor
{
    internal class AssetFile
    {
        internal static void Process(FileInfo item, DirectoryInfo output, DictionaryStack stack)
        {
            Console.WriteLine($"\tCopying {item.Name}");
            item.CopyTo(Path.Join(output.FullName, item.Name));
        }

    }
}
