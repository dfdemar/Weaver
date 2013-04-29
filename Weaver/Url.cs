using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Weaver
{
    public class Url
    {
        public Uri uri { get; set; }
        public int depth { get; set; }

        public Url()
        {
        }

        public Url(string path, int depth)
        {
            this.uri = new Uri(path);
            this.depth = depth;
        }

        public Url(Uri uri, int depth)
        {
            this.uri = uri;
            this.depth = depth;
        }

        public void Download()
        {
            Thread.Sleep(SpiderController.IdleTime());

            string filename = Path.GetFileName(this.uri.LocalPath);

            UriBuilder uri = new UriBuilder(this.uri.AbsoluteUri);
            string path = SpiderController.DownloadFolder + Regex.Replace(Path.GetDirectoryName(uri.Path), "/", @"\");

            if(!path.EndsWith(@"\"))
                path += @"\";

            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);

            using (WebClient client = new WebClient())
            {
                client.DownloadFileAsync(this.uri, path + filename);
                Log.DownloadedFile(this.uri.AbsoluteUri);
            }
        }
    }
}
