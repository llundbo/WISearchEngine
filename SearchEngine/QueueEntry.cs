using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine
{
    public class QueueEntry
    {
        public List<string> SubURLs = new List<string>();
        int CrawlDelay;

        public QueueEntry(List<string> inputlist, int delay)
        {
            SubURLs = inputlist;
            CrawlDelay = delay;
        }
    }
}
