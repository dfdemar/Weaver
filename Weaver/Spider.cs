using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Weaver
{
    public class Spider
    {
        private Queue<Url> URLQueue { get; set; }
        private List<String> visitedURLs { get; set; }
        private int threadCount { get; set; }

        public Spider()
        {
            this.URLQueue = new Queue<Url>();
            this.visitedURLs = new List<string>();
            this.threadCount = 0;
        }

        public void Go(string link)
        {
            Url url = new Url { name = link, depth = 0 };
            new Thread(() => FetchNewPage(url)).Start();
        }

        private void FetchNewPage(Url url)
        {
            Log.ThreadCount(++threadCount);

            NetworkConnection connection = new NetworkConnection();
            string html = connection.Go(url.name);

            if (!String.IsNullOrEmpty(html))
            {
                Log.LoadSuccess(url.name);
                this.visitedURLs.Add(url.name);
                Crawl(html, url);
            }
            LoadNextURL();
        }

        private void Crawl(string html, Url url, int currentLocation = 0)
        {
            string link = FindLink(html, ref currentLocation, url.depth);

            if (!String.IsNullOrEmpty(link))
            {
                Crawl(html, url, currentLocation);
            }
            else
                Console.WriteLine("No link found.");
        }

        private string FindLink(string html, ref int startLocation, int depth)
        {
            string url = null;

            int index = html.ToLower().IndexOf("href=\"http", startLocation);
            if (index != -1)
            {
                int start = html.IndexOf('"', index) + 1;
                int end = html.IndexOf('"', start);
                url = html.Substring(start, end - start);
                startLocation = end;

                Log.FoundURL(url);
                HandleURL(url, depth + 1);
            }
            return url;
        }

        private void LoadNextURL()
        {
            while (this.URLQueue.Count > 0)
            {
                Url url = new Url();

                lock (this.URLQueue)
                    url = this.URLQueue.Dequeue();

                if (SpiderController.ShouldContinue(url.depth))
                {
                    Thread.Sleep(SpiderController.ThreadIdleTime);
                    new Thread(() => FetchNewPage(url)).Start();
                }
            }
            Thread.Sleep(SpiderController.ThreadIdleTime);
            --threadCount;
        }

        private void HandleURL(string link, int newDepth)
        {
            if (this.visitedURLs.Contains(link) || this.URLQueue.Any(q => q.name == link))
                Log.SkippedThisQueuedURL(link);
            else if (SpiderController.IsExcludedDomain(link))
                Log.SkippedThisExcludedURL(link);
            else if (SpiderController.IsExcludedFileType(link.ToLower()))
                Log.SkippedThisExcludedFileType(link);
            else if (SpiderController.ShouldDownload(link.ToLower()))
                Download(link);
            else
            {
                lock (this.URLQueue)
                    this.URLQueue.Enqueue(new Url { name = link, depth = newDepth });

                Log.EngueuedURL(link);
            }
        }

        private void Download(string link)
        {
            this.visitedURLs.Add(link);
            Thread.Sleep(SpiderController.ThreadIdleTime);

            Uri uri = new Uri(link);
            string filename = Path.GetFileName(uri.LocalPath);

            using (WebClient client = new WebClient())
            {
                client.DownloadFileAsync(uri, @"C:\Temp\Spider\" + filename);
                Log.DownloadedFile(link);
            }
        }
    }
}
