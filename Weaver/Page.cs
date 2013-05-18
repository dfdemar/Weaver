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

        private static Regex URLPATTERN = new Regex(@"(href|src)=""[\d\w\/:#@%;$\(\)~_\?\+\-=\\\.&]*", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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
                    Uri uri = new Uri(cleanUrl);
                    Url url = new Url(uri, depth + 1);

                    UrlList.Add(url);
                    Log.FoundURL(url.uri.AbsoluteUri);
                }
            }
        }

        private string CleanUrl(string url)
        {
            StringBuilder cleanUrl = new StringBuilder(String.Empty);

            if (!url.Contains("mailto:"))
            {
                try
                {
                    cleanUrl.Append(Regex.Replace(url, @"(?i)(href|src)=|""", ""));

                    Uri uri;

                    if (!IsAbsoluteUrl(cleanUrl.ToString()))
                    {
                        if (cleanUrl.ToString().StartsWith("/"))
                            uri = new Uri(GetParentUriString(this.url.uri), cleanUrl.ToString());
                        else
                            uri = new Uri(this.url.uri.AbsoluteUri + cleanUrl.ToString());
                    }
                    else
                        uri = new Uri(cleanUrl.ToString());

                    UriBuilder uriBuilder = new UriBuilder(uri);
                    uriBuilder.Fragment = String.Empty;

                    cleanUrl.Clear();
                    cleanUrl.Append(uriBuilder.Uri.AbsoluteUri);
                }
                catch (UriFormatException ex)
                {
                    Console.WriteLine(ex.Message, url);
                }
            }

            return cleanUrl.ToString();
        }

        private bool IsAbsoluteUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result); 
        }

        private Uri GetParentUriString(Uri uri)
        {
            string path = uri.AbsoluteUri.Remove(uri.AbsoluteUri.Length - uri.Segments.Last().Length);
            return new Uri(path);
        }
    }
}
