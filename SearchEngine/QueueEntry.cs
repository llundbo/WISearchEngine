using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine
{
    public class QueueEntry
    {
        public int VisitMutex = 0;
        public int SubURLListMutex = 0;
        public volatile List<SubURL> SubURLs = new List<SubURL>();
        public int CrawlDelay;
        public DateTime LastVisited;
        public List<string> RefList = new List<string>();

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
