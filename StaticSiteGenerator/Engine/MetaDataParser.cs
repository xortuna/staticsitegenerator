using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticSiteGenerator.Engine
{
    internal class MetaDataParser
    {
        public struct MetaData
        {
            public string Key;
            public string Value;
        }
        public static MetaData Parse(TemplateElement element)
        {
            var spltiPos = element.Content.IndexOf(":");
            if (spltiPos == -1) throw new Exception($"Invalid meta data format at {element.Line}: {element.Content}");
            return new MetaData
            {
                Key =
                element.Content.Substring(0, spltiPos),
                Value = element.Content.Substring(spltiPos + 1)
            };

        }
    }
}
