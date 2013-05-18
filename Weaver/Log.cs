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
        private static string path { get; set; }

        static Log()
        {
            file = "SpiderLog.txt";
            path = SpiderController.LogFolder;
        }

        public static void WriteToLog(string entry, string url = "")
        {
            Console.WriteLine("[{0}]: {1}: {2}", DateTime.Now, entry, url);

            if (SpiderController.UseLogging)
            {
                lock (file)
                {
                    string line = String.Format("{0,-25}{1,-40}{2}", "[" + DateTime.Now + "]", entry, url);
                    File.AppendAllText(path + file, line + Environment.NewLine);
                }
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
            WriteToLog("Skipping...URL domain is excluded", url);
        }

        public static void SkippedThisExcludedFileType(string url)
        {
            WriteToLog("Skipping...file type is excluded", url);
        }

        public static void EngueuedURL(string url)
        {
            WriteToLog("Queuing", url);
        }

        public static void DownloadedFile(string filename)
        {
            WriteToLog("Downloaded", filename);
        }
    }
}
