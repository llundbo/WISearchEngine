using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

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
            FetchData(SeedURLs[0].ToString());
        }

        private void AddSeeds()
        {                   
            SeedURLs.Add(new Uri("https://eb.dk"));
            SeedURLs.Add(new Uri("http://migogaalborg.dk"));
            SeedURLs.Add(new Uri("http://nyheder.tv2.dk"));
            SeedURLs.Add(new Uri("https://bt.dk"));
            SeedURLs.Add(new Uri("https://moodle.org"));
            SeedURLs.Add(new Uri("https://dr.dk"));
            SeedURLs.Add(new Uri("https://ing.dk"));
            SeedURLs.Add(new Uri("https://nordjyske.dk"));
            SeedURLs.Add(new Uri("https://aalborgnu.dk"));
        }

        private void InitializeSeedsToQueue()
        {
            foreach(Uri seed in SeedURLs)
            {
                GetRobotRules(seed);

                if (RulesList[seed.Host].DisallowedUrls.Contains("/"))
                    continue;

                UrlQueue.Add(seed.ToString(), new QueueEntry(new List<Uri> { seed }, RulesList[seed.Host].Delay));
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

        private void FetchData(string url)
        {
            List<string> hyperlinks = new List<string>();
            string content = string.Empty;

            HtmlWeb web = new HtmlWeb();
            var doc = web.Load(url);

            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                string href = link.OuterHtml.Split("\"")[1];

                if (href.StartsWith("/"))
                    href = url + href.Substring(1);
                
                if(href.StartsWith("http"))
                    hyperlinks.Add(href);
            }

            foreach (HtmlNode text in doc.DocumentNode.SelectNodes("//body"))
            {
                if(!string.IsNullOrWhiteSpace(text.InnerText))
                    content += text.InnerText.Trim().Replace("&nbsp", "");
            }

            content = Regex.Replace(content, @"\s+", " ");
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            content = rgx.Replace(content, "");

            SortHyperLinks(hyperlinks);
        }

        private void SortHyperLinks(List<string> hyperlinks)
        {
            foreach(string link in hyperlinks)
            {
                Uri url = new Uri(link);

                if(UrlQueue.ContainsKey(url.Host))

            }
        }

        private void HandleQueue()
        {
            int pageCount = 0;

            do
            {
                foreach (string entry in UrlQueue.Keys)
                {
                    if (pageCount >= 1000)
                        break;

                    if (!(DateTime.Now > UrlQueue[entry].LastVisited.AddSeconds(UrlQueue[entry].CrawlDelay)))
                        continue;

                    FetchData(UrlQueue[entry].SubURLs.First().ToString());

                    UrlQueue[entry].SubURLs.RemoveAt(0);
                    pageCount++;
                }
            } while (pageCount < 1000);
            
        }
    }
}
