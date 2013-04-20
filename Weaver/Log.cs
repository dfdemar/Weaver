using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Weaver
{
    public static class Log
    {
        private static string file { get; set; }
        private static string path = "C:\\temp\\";

        static Log()
        {
            file = "SpiderLog.txt";
        }

        private static void WriteToLog(string entry, string url = "")
        {
            Console.WriteLine("[{0}]: {1}: {2}", DateTime.Now, entry, url);
            string line = String.Format("{0,-25}{1,-40}{2}", "["+DateTime.Now+"]", entry, url);

            if (SpiderController.UseLogging)
            {
                lock(file)
                    File.AppendAllText(path + file, line + Environment.NewLine);
            }
        }

        public static void FoundURL(string url)
        {
            WriteToLog("Grabbed URL from page", url);
        }

        public static void LoadingNewPage(string url)
        {
            WriteToLog("Loading new page", url);
        }

        public static void LoadSuccess(string url)
        {
            WriteToLog("Page load successful", url);
        }

        public static void SkippedThisQueuedURL(string url)
        {
            WriteToLog("Skipping...URL already queued", url);
        }

        public static void SkippedThisExcludedURL(string url)
        {
            WriteToLog("Skipping...URL domain is excluded");
        }

        public static void EngueuedURL(string url)
        {
            WriteToLog("Enqueuing", url);
        }

        public static void ThreadCount(int count)
        {
            WriteToLog(String.Format("Number of current threads: {0}", count));
        }
    }
}
