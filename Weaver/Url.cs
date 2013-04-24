using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
