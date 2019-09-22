using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine
{
    public class SubURL
    {
        public Uri Url;
        public volatile bool Visited;

        public SubURL(Uri url)
        {
            Url = url;
        }

        public override string ToString()
        {
            return Url.ToString();
        }
    }
}
