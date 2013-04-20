using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Weaver
{
    static class SpiderController
    {
        private static XmlDocument xmlDoc;
        public static readonly int maxDepth { get; private set; }
        public static readonly bool UseLogging { get; private set; }

        public static readonly List<string> ExcludedFileTypes { get; private set; }
        public static readonly List<string> ExcludedDomains { get; private set; }

        static SpiderController()
        {
            xmlDoc = new XmlDocument();
            xmlDoc.Load("SpiderConfig.xml");

            maxDepth = Int32.Parse(xmlDoc.GetElementById("MaximumDepth").InnerText);
            ExcludedFileTypes = xmlDoc.GetElementById("ExcludedFileTypes").InnerText.Split('|').ToList<string>();
            ExcludedDomains = xmlDoc.GetElementById("ExcludedDomains").InnerText.Split(';').ToList<string>();
        }

        public static bool ShouldContinue(int currentDepth)
        {
            return currentDepth < maxDepth;
        }
    }
}
