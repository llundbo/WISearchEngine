using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine
{
    static class ContentHandler
    {
        public static List<UrlContent> pageContent = new List<UrlContent>();
        static private float jaccardThreshold = 0.5f;

        public static void AddContent (string content, string url)
        {   
            UrlContent newContent = new UrlContent(url, content, NearDuplicateDetection.MakeShingles(content));

            if (pageContent.Count == 0)
            {
                pageContent.Add(newContent);
                Indexer.AddWordsToIndexList(content, pageContent.Count - 1);
                return;
            }

            bool matchFound = false;
            foreach (UrlContent page in pageContent.ToArray())
            {
                float jaccard = NearDuplicateDetection.CalculateJaccard(newContent.Shingles, page.Shingles);
                //Console.WriteLine(jaccard + " " + newContent.Url + " " + page.Url);

                if (jaccard > jaccardThreshold)
                {
                    matchFound = true;
                    Console.WriteLine("Duplicate found");
                    break;
                }
            }
            
            if(!matchFound)
            {
                pageContent.Add(newContent);
                Indexer.AddWordsToIndexList(content, pageContent.Count - 1);
            }
        }
    }
}
