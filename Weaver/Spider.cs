using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Weaver
{
    public class Spider
    {
        private Queue<Url> queuedURLs { get; set; }
        private List<String> visitedURLs { get; set; }
        private Log log { get; set; }

        public Spider()
        {
            this.queuedURLs = new Queue<Url>();
            this.visitedURLs = new List<string>();
            this.log = new Log();
        }

        public void Go(string link)
        {
            Url url = new Url { name = link, depth = 0 };
            FetchNewPage(url);
        }

        private void FetchNewPage(Url url)
        {
            NetworkConnection connection = new NetworkConnection();
            string html = connection.Go(url.name, log);

            if (!String.IsNullOrEmpty(html))
            {
                log.LoadSuccess(url.name);
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

                log.FoundURL(url);
                AddToQueue(url, depth + 1);
            }
            return url;
        }

        private void LoadNextURL()
        {
            while (this.queuedURLs.Count > 0)
            {
                Url url = this.queuedURLs.Dequeue();
                if (SpiderController.ShouldContinue(url.depth))
                    FetchNewPage(new Url { name = url.name, depth = url.depth });
            }
        }

        private void AddToQueue(string link, int newDepth)
        {
            if (this.visitedURLs.Contains(link))
                log.SkippedThisURL(link);
            else
            {
                this.queuedURLs.Enqueue(new Url { name = link, depth = newDepth });
                log.EngueuedURL(link);
            }
        }
    }
}
