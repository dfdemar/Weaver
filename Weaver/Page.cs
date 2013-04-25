using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Weaver
{
    public class Page
    {
        public Url url { get; private set; }
        public string source { get; private set; }
        public List<Url> UrlList { get; private set; }

        public static Regex URLPATTERN = new Regex(@"(href|src)=""[\d\w\/:#@%;$\(\)~_\?\+\-=\\\.&]*", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public Page(Url url, string source)
        {
            this.url = url;
            this.source = source;
            this.UrlList = new List<Url>();
        }

        public void FetchAllUrls(int depth)
        {
            MatchCollection matches = URLPATTERN.Matches(this.source);

            foreach (Match match in matches)
            {
                string cleanUrl = CleanUrl(match.Value);

                if (!String.IsNullOrEmpty(cleanUrl))
                {
                    Uri uri = new Uri(GetParentUriString(this.url.uri), cleanUrl);
                    Url url = new Url(uri, depth + 1);

                    UrlList.Add(url);
                    Log.FoundURL(url.uri.AbsoluteUri);
                }
            }
        }

        public string CleanUrl(string url)
        {
            string cleanUrl = String.Empty;

            if(!url.Contains("mailto:"))
                cleanUrl = Regex.Replace(url, @"(href|src)=|""", "");

            return cleanUrl;
        }

        public Uri GetParentUriString(Uri uri)
        {
            string path = uri.AbsoluteUri.Remove(uri.AbsoluteUri.Length - uri.Segments.Last().Length);
            return new Uri(path);
        }
    }
}
