using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SearchEngine
{
    public class Crawler
    {
        private List<Uri> SeedURLs = new List<Uri>();
        private Dictionary<string, RobotRules> RulesList = new Dictionary<string, RobotRules>();
        private Dictionary<string, QueueEntry> UrlQueue = new Dictionary<string, QueueEntry>();

        public Crawler()
        {
            AddSeeds();
            InitializeSeedsToQueue();
        }

        private void AddSeeds()
        {
            //SeedURLs.Add(new Uri("https://eb.dk"));
            //SeedURLs.Add(new Uri("http://migogaalborg.dk"));
            //SeedURLs.Add(new Uri("http://nyheder.tv2.dk"));
            //SeedURLs.Add(new Uri("bt.dk"));
            SeedURLs.Add(new Uri("https://moodle.org"));
            //SeedURLs.Add(new Uri("dr.dk"));
            //SeedURLs.Add(new Uri("ing.dk"));
            //SeedURLs.Add(new Uri("nordjyske.dk"));
            //SeedURLs.Add(new Uri("aalborgnu.dk"));
        }

        private void InitializeSeedsToQueue()
        {
            foreach(Uri seed in SeedURLs)
            {
                GetRobotRules(seed);

                if (RulesList[seed.ToString()].DisallowedUrls.Contains("/"))
                    continue;

                UrlQueue.Add(seed.ToString(), new QueueEntry(new List<string>(), RulesList[seed.ToString()].Delay));

            }
        }

        private void GetRobotRules(Uri url)
        {
            Uri robotURL = new Uri(url.ToString() + "/robots.txt");
            string result;

            using (WebClient client = new WebClient())
            {
                result = client.DownloadString(robotURL);
            }

            if (!RulesList.ContainsKey(url.Host))
                RulesList.Add(url.Host, new RobotRules(result));
        }
    }
}
