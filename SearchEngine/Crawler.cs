using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Threading;
using System.Threading.Tasks;

namespace SearchEngine
{
    public class Crawler
    {
        private List<Uri> SeedURLs = new List<Uri>();
        private Dictionary<string, RobotRules> RulesList = new Dictionary<string, RobotRules>();
        private SyncedUrlQueue UrlQueue = new SyncedUrlQueue();
        //private volatile Dictionary<string, QueueEntry> UrlQueue = new Dictionary<string, QueueEntry>();

        public Crawler()
        {
            AddSeeds();
            InitializeSeedsToQueue();
            HandleQueue();
            Console.ReadKey();
        }

        private void AddSeeds()
        {                   
            SeedURLs.Add(new Uri("https://eb.dk"));
            //SeedURLs.Add(new Uri("https://migogaalborg.dk"));
            //SeedURLs.Add(new Uri("https://nyheder.tv2.dk"));
            //SeedURLs.Add(new Uri("https://bt.dk"));
            //SeedURLs.Add(new Uri("https://moodle.org"));
            //SeedURLs.Add(new Uri("https://dr.dk"));
            //SeedURLs.Add(new Uri("https://ing.dk"));
            //SeedURLs.Add(new Uri("https://nordjyske.dk"));
            //SeedURLs.Add(new Uri("https://aalborgnu.dk"));
        }

        private void InitializeSeedsToQueue()
        {
            foreach(Uri seed in SeedURLs)
            {
                GetRobotRules(seed);

                if (RulesList[seed.Host].DisallowedUrls.Contains("/"))
                    continue;

                UrlQueue.Add(seed.Host, new QueueEntry(new List<Uri> { seed }, RulesList[seed.Host].Delay));
            }
        }

        private void GetRobotRules(Uri url)
        {
            Uri robotURL = new Uri(url.ToString() + (url.ToString().Last() == '/' ? "robots.txt" : "/robots.txt"));
            string result = string.Empty;

            using (WebClient client = new WebClient())
            {
                for (int i = 0; i < 2; i++)
                {
                    try
                    {
                        if (i == 0)
                        {
                            result = client.DownloadString(robotURL).Replace("http://", "https://");
                            if (!string.IsNullOrEmpty(result))
                            {
                                if (!RulesList.ContainsKey(url.Host))
                                    RulesList.Add(url.Host, new RobotRules(result));
                                break;
                            }

                        }
                        else
                        {
                            result = client.DownloadString(robotURL).Replace("https://", "http://");
                            if (!RulesList.ContainsKey(url.Host))
                                RulesList.Add(url.Host, new RobotRules(result));
                        }
                    }
                    catch (WebException)
                    {
                        continue;
                    }
                }
                
                if(string.Empty == result)
                    RulesList.Add(url.Host, new RobotRules(""));
            }    
        }

        private void FetchData(string url)
        {
            List<string> hyperlinks = new List<string>();
            string content = string.Empty;

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc;

            try
            {
                doc = web.Load(url);
            }
            catch (Exception)
            {
                return;
            }

            HtmlNodeCollection hyperNodes = doc.DocumentNode.SelectNodes("//a[@href]");

            if(!(hyperNodes == null))
            {
                foreach (HtmlNode link in hyperNodes)
                {
                    string href = string.Empty;

                    try
                    {
                        href = link.OuterHtml.Split("\"")[1];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        continue;
                    }

                    if (href.StartsWith("/"))
                        href = url + href.Substring(1);

                    if (href.StartsWith("http"))
                        hyperlinks.Add(href);
                }
            }

            HtmlNodeCollection contentNodes = doc.DocumentNode.SelectNodes("//body");

            if(!(contentNodes == null))
            {
                foreach (HtmlNode text in doc.DocumentNode.SelectNodes("//body"))
                {
                    if (!string.IsNullOrWhiteSpace(text.InnerText))
                        content += text.InnerText.Trim().Replace("&nbsp", "");
                }
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
                Uri url = new Uri(link.Replace("www.", ""));

                if (UrlQueue.ContainsKey(url.Host))
                {
                    bool allowed = true;
                    foreach (string seg in url.Segments)
                    {
                        if (RulesList[url.Host].DisallowedUrls.Contains(seg))
                        {
                            allowed = false;
                            break;
                        }
                    }

                    bool done = false;
                    while(!done)
                    {
                        if(0 == Interlocked.Exchange(ref UrlQueue.Read(url.Host).SubURLListMutex, 1))
                        {
                            if (allowed)
                            {
                                foreach (var suburl in UrlQueue.Read(url.Host).SubURLs)
                                {
                                    if (suburl.Url == url)
                                        allowed = false;
                                }
                            }

                            if (allowed)
                                UrlQueue.Read(url.Host).SubURLs.Add(new SubURL(url));

                            Interlocked.Exchange(ref UrlQueue.Read(url.Host).SubURLListMutex, 0);
                            done = true;
                        }
                        
                    }
                }
                    
                else
                {
                    try
                    {
                        GetRobotRules(new Uri("https://" + url.Host));
                    }
                    catch (WebException)
                    {
                        GetRobotRules(new Uri("http://" + url.Host));
                    }
                    
                    if (RulesList[url.Host].DisallowedUrls.Contains("/"))
                        continue;

                    // Tjekker kun om /path/ er indeholdt i disallowed og ikke om /path/her/ er disallowed og f.eks. /path/hermådugernegåhen er allowed
                    bool allowed = true;
                    foreach (string seg in url.Segments)
                    {
                        if (RulesList[url.Host].DisallowedUrls.Contains(seg))
                        {
                            allowed = false;
                            break;
                        }
                    }

                    if(allowed && !UrlQueue.ContainsKey(url.Host))
                        UrlQueue.Add(url.Host, new QueueEntry(new List<Uri> { url }, RulesList[url.Host].Delay));
                }
            }
        }

        private void HandleQueue()
        {
            int pageCount = 0;

            int numOfThreads = 4;

            List<Task> tasklist = new List<Task>();

            for (int i = 0; i < numOfThreads; i++)
            {
                tasklist.Add(Task.Run(() =>
                {
                    do
                    {
                        var threadKeys = UrlQueue.Keys().ToArray();

                        foreach (string entry in threadKeys)
                        {
                            if (pageCount >= 1000)
                                break;

                            QueueEntry qEntry = UrlQueue.Read(entry);

                            if (!(DateTime.Now > qEntry.LastVisited.AddSeconds(qEntry.CrawlDelay)))
                            {
                                //Console.WriteLine("Delaying the access to:" + entry);
                                continue;
                            }

                            int idx = FindNextIndex(entry, qEntry.SubURLs);

                            if (idx == -1)
                                continue;

                            Console.WriteLine("Fetching from: " + qEntry.SubURLs[idx].ToString());

                            qEntry.LastVisited = DateTime.Now;

                            FetchData(qEntry.SubURLs[idx].ToString());

                            pageCount++;
                            Console.WriteLine("PageCount: " + pageCount);
                        }

                    } while (pageCount < 1000);

                }));
            }

            Task.WaitAll(tasklist.ToArray());

            Console.WriteLine("Queue Handler Complete...");
        }

        private int FindNextIndex(string host,List<SubURL> suburls)
        {
            foreach (var url in suburls.ToArray())
            {
                if (0 == Interlocked.Exchange(ref UrlQueue.Read(host).VisitMutex, 1))
                {
                    if (url.Visited == false)
                    {
                        url.Visited = true;
                        Interlocked.Exchange(ref UrlQueue.Read(host).VisitMutex, 0);
                        return suburls.IndexOf(url);
                    }
                }
                else
                    return -1;
            }

            return -1;
        }
    }
}
