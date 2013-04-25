﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Weaver
{
    public class Spider
    {
        private Queue<Url> URLQueue { get; set; }
        private HashSet<String> UrlsSeen { get; set; }
        private int threadCount { get; set; }

        public Spider()
        {
            this.URLQueue = new Queue<Url>();
            this.UrlsSeen = new HashSet<String>();
            this.threadCount = 0;
        }

        public void Go()
        {
            foreach (string seed in SpiderController.SeedURLs)
            {
                Url url = new Url(seed, 0);
                this.UrlsSeen.Add(seed);
                ThreadPool.QueueUserWorkItem(obj => FetchNewPage(url));
            }

            if (SpiderController.SeedURLs.Count == 0)
                Console.WriteLine("Need at least one seed URL.");
        }

        private void FetchNewPage(Url url)
        {
            Log.ThreadCount(++threadCount);

            NetworkConnection connection = new NetworkConnection();
            Page page = new Page(url, connection.Go(url));

            if (!String.IsNullOrEmpty(page.source))
            {
                Log.LoadSuccess(url.uri.AbsoluteUri);
                Crawl(page);
            }
            LoadNextURL();
        }

        private void Crawl(Page page)
        {
            page.FetchAllUrls(page.url.depth);

            if (page.UrlList.Count > 0)
            {
                foreach (Url url in page.UrlList)
                    HandleURL(url);
            }
            else
                Console.WriteLine("No links found.");

            Console.WriteLine("Finished crawling page.");
        }

        private void LoadNextURL()
        {
            while (this.URLQueue.Count > 0)
            {
                Url url = new Url();

                lock (this.URLQueue)
                {
                    if (this.URLQueue.Count > 0)
                        url = this.URLQueue.Dequeue();
                }

                if (SpiderController.ShouldContinue(url.depth))
                {
                    Thread.Sleep(SpiderController.IdleTime());
                    ThreadPool.QueueUserWorkItem(obj => FetchNewPage(url));
                }
            }
            --threadCount;
        }

        private void HandleURL(Url url)
        {
            string link = url.uri.AbsoluteUri.ToLower();

            if (this.UrlsSeen.Contains(link))
                Log.SkippedThisQueuedURL(link);
            else if (SpiderController.IsExcludedDomain(link))
                Log.SkippedThisExcludedURL(link);
            else if (SpiderController.IsExcludedFileType(link))
                Log.SkippedThisExcludedFileType(link);
            else if (SpiderController.ShouldDownload(link))
            {
                this.UrlsSeen.Add(link);
                url.Download();
            }
            else
            {
                lock (this.URLQueue)
                {
                    this.UrlsSeen.Add(link);
                    this.URLQueue.Enqueue(url);
                }

                Log.EngueuedURL(link);
            }
        }
    }
}
