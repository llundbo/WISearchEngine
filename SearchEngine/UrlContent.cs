using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine
{
    class UrlContent
    {
        public string Url;
        public string Content;
        public string[] Shingles;

        public UrlContent(string url, string content, string[] shingles)
        {
            this.Url = url;
            this.Content = content;
            this.Shingles = shingles;
        }
    }
}
