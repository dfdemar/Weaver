using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Weaver
{
    public class Log
    {
        private string file { get; set; }
        private string path = "C:\\temp\\";

        public Log()
        {
            this.file = "SpiderLog.txt";
        }

        private void WriteToLog(string entry, string url)
        {
            Console.WriteLine("[{0}]: {1}: {2}", DateTime.Now, entry, url);
            string line = String.Format("{0,-25}{1,-40}{2}", "["+DateTime.Now+"]", entry, url);

            if(SpiderController.UseLogging)
                File.AppendAllText(this.path + this.file, line + Environment.NewLine);
        }

        public void FoundURL(string url)
        {
            WriteToLog("Grabbed URL from page", url);
        }

        internal void LoadingNewPage(string url)
        {
            WriteToLog("Loading new page", url);
        }

        public void LoadSuccess(string url)
        {
            WriteToLog("Page load successful", url);
        }

        public void SkippedThisURL(string url)
        {
            WriteToLog("URL already visited...skip queuing", url);
        }

        internal void EngueuedURL(string url)
        {
            WriteToLog("Enqueuing", url);
        }
    }
}
