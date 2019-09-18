using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine
{
    public class QueueEntry
    {
        public List<Uri> SubURLs = new List<Uri>();
        public int CrawlDelay;
        public DateTime LastVisited;

        public QueueEntry(List<Uri> inputlist, int delay)
        {
            SubURLs = inputlist;
            CrawlDelay = delay;
        }
    }
}
