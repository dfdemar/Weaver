using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Weaver
{
    public static class SpiderController
    {
        private static XmlDocument xmlDoc;

        public static int MaxDepth { get; private set; }
        public static int MaxThreads { get; private set; }
        public static bool UseLogging { get; private set; }

        public static List<string> ExcludedFileTypes { get; private set; }
        public static List<string> ExcludedDomains { get; private set; }

        static SpiderController()
        {
            xmlDoc = new XmlDocument();
            xmlDoc.Load("SpiderConfig.xml");

            MaxDepth = Int32.Parse(xmlDoc.GetElementById("MaximumDepth").InnerText);
            MaxThreads = Int32.Parse(xmlDoc.GetElementById("MaximumThreads").InnerText);
            ExcludedFileTypes = xmlDoc.GetElementById("ExcludedFileTypes").InnerText.Split('|').ToList<string>();
            ExcludedDomains = xmlDoc.GetElementById("ExcludedDomains").InnerText.Split(new char[]{',',' ', '\r', '\n'}, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            UseLogging = Boolean.Parse(xmlDoc.GetElementById("UseLogging").InnerText);
        }

        public static bool ShouldContinue(int currentDepth)
        {
            return currentDepth < MaxDepth;
        }

        public static bool IsExcludedDomain(string url)
        {
            bool isExcluded = false;

            lock(ExcludedDomains)
            {
                foreach (string domain in ExcludedDomains)
                {
                    if (url.Contains(domain))
                    {
                        isExcluded = true;
                        break;
                    }
                }
            }
            return isExcluded;
        }
    }
}
