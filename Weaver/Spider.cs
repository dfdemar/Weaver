using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Weaver
{
    public class Spider
    {
        private Queue<Url> queuedURLs { get; set; }
        private List<String> visitedURLs { get; set; }
        private int threadCount { get; set; }

        public Spider()
        {
            this.queuedURLs = new Queue<Url>();
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
                AddToQueue(url, depth + 1);
            }
            return url;
        }

        private void LoadNextURL()
        {
            while (this.queuedURLs.Count > 0)
            {
                Url url = new Url();

                lock (this.queuedURLs)
                    url = this.queuedURLs.Dequeue();

                if (SpiderController.ShouldContinue(url.depth))
                    new Thread(() => FetchNewPage(url)).Start();
            }
            --threadCount;
            Thread.Sleep(50);
        }

        private void AddToQueue(string link, int newDepth)
        {
            if (this.visitedURLs.Contains(link) || this.queuedURLs.Any(q => q.name == link))
                Log.SkippedThisQueuedURL(link);
            else if (SpiderController.IsExcludedDomain(link))
                Log.SkippedThisExcludedURL(link);
            else
            {
                lock (this.queuedURLs)
                    this.queuedURLs.Enqueue(new Url { name = link, depth = newDepth });

                Log.EngueuedURL(link);
            }
        }
    }
}
