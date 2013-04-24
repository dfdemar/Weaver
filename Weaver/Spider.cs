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
        private HashSet<String> UrlsSeen { get; set; }
        private int threadCount { get; set; }

        public Spider()
        {
            this.URLQueue = new Queue<Url>();
            this.UrlsSeen = new HashSet<String>();
            this.threadCount = 0;
        }

        public void Go(string link)
        {
            Url url = new Url(link, 0);
            this.UrlsSeen.Add(url.uri.AbsoluteUri);
            ThreadPool.QueueUserWorkItem(obj => FetchNewPage(url));
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
                    Thread.Sleep(SpiderController.ThreadIdleTime);
                    ThreadPool.QueueUserWorkItem(obj => FetchNewPage(url));
                }
            }
            --threadCount;
        }

        private void HandleURL(Url url)
        {
            string link = url.uri.AbsoluteUri;

            if (this.UrlsSeen.Contains(link))
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
                {
                    lock (this.UrlsSeen)
                    {
                        this.UrlsSeen.Add(url.uri.AbsoluteUri);
                        this.URLQueue.Enqueue(url);
                    }
                }

                Log.EngueuedURL(link);
            }
        }

        private void Download(string link)
        {
            this.UrlsSeen.Add(link);
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
