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
        public static bool UseWhiteList { get; private set; }
        public static int MinThreadIdleTime { get; private set; }
        public static int MaxThreadIdleTime { get; private set; }
        public static string DownloadFolder { get; private set; }
        public static string LogFolder { get; set; }

        public static List<string> ExcludedFileTypes { get; private set; }
        public static List<string> ExcludedDomains { get; private set; }
        public static List<string> WhiteListedDomains { get; private set; }
        public static List<string> FileTypesToDownload { get; private set; }
        public static List<string> SeedURLs { get; private set; }

        static SpiderController()
        {
            xmlDoc = new XmlDocument();
            xmlDoc.Load("SpiderConfig.xml");

            MaxDepth = Int32.Parse(xmlDoc.GetElementById("MaximumDepth").InnerText);
            MaxThreads = Int32.Parse(xmlDoc.GetElementById("MaximumThreads").InnerText);
            UseLogging = Boolean.Parse(xmlDoc.GetElementById("UseLogging").InnerText);
            UseWhiteList = Boolean.Parse(xmlDoc.GetElementById("UseWhiteList").InnerText);
            MinThreadIdleTime = Int32.Parse(xmlDoc.GetElementById("MinThreadIdleTime").InnerText);
            MaxThreadIdleTime = Int32.Parse(xmlDoc.GetElementById("MaxThreadIdleTime").InnerText);
            DownloadFolder = xmlDoc.GetElementById("DownloadFolder").InnerText;
            LogFolder = xmlDoc.GetElementById("LogFolder").InnerText;

            ExcludedFileTypes = xmlDoc.GetElementById("ExcludedFileTypes").InnerText.Split('|').ToList<string>();
            ExcludedDomains = xmlDoc.GetElementById("ExcludedDomains").InnerText.Split(new char[]{',',' ', '\r', '\n'}, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            WhiteListedDomains = xmlDoc.GetElementById("WhiteListedDomains").InnerText.Split(new char[] { ',', ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            FileTypesToDownload = xmlDoc.GetElementById("FileTypesToDownload").InnerText.Split('|').ToList<string>();
            SeedURLs = xmlDoc.GetElementById("SeedURLs").InnerText.Split(new char[] { ',', ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();   
        }

        public static bool ShouldContinue(int currentDepth)
        {
            return currentDepth < MaxDepth;
        }

        public static bool IsExcludedDomain(string url)
        {
            bool isExcluded = false;

            try
            {
                Uri uri = new Uri(url.ToLower());

                lock (ExcludedDomains)
                {
                    foreach (string domain in ExcludedDomains)
                    {
                        if (uri.Authority.Contains(domain.ToLower()))
                        {
                            isExcluded = true;
                            break;
                        }
                    }
                }
            }
            catch(UriFormatException ex)
            {
                Console.WriteLine(ex.Message);
                isExcluded = true;
            }
            return isExcluded;
        }

        public static bool IsExcludedFileType(string url)
        {
            bool isExcluded = false;

            lock (ExcludedFileTypes)
            {
                foreach (string fileType in ExcludedFileTypes)
                {
                    if (url.ToLower().EndsWith(fileType.ToLower()))
                    {
                        isExcluded = true;
                        break;
                    }
                }
            }
            return isExcluded;
        }

        public static bool ShouldDownload(string url)
        {
            bool downloadMe = false;

            lock (FileTypesToDownload)
            {
                foreach (string fileType in FileTypesToDownload)
                {
                    if (url.ToLower().EndsWith(fileType.ToLower()))
                    {
                        downloadMe = true;
                        break;
                    }
                }
            }
            return downloadMe;
        }

        public static int IdleTime()
        {
            Random random = new Random();
            return random.Next(MinThreadIdleTime, MaxThreadIdleTime + 1);
        }

        public static bool IsWhiteListedDomain(string domain)
        {
            bool isWhiteListed = false;

            lock (WhiteListedDomains)
            {
                foreach (string wlDomain in WhiteListedDomains)
                {
                    if(domain.StartsWith("www."))
                        domain = domain.Remove(0, 4);

                    if (domain == wlDomain)
                    {
                        isWhiteListed = true;
                        break;
                    }
                }
            }
            return isWhiteListed;
        }
    }
}
