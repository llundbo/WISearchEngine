using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine
{
    public class QueueEntry
    {
        public List<SubURL> SubURLs = new List<SubURL>();
        public int CrawlDelay;
        public DateTime LastVisited;

        public QueueEntry(List<Uri> inputlist, int delay)
        {
            foreach (var url in inputlist)
            {
                SubURLs.Add(new SubURL(url));
            }
            CrawlDelay = delay;
        }
    }
}
