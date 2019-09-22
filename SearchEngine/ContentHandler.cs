using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine
{
    static class ContentHandler
    {
        static List<UrlContent> pageContent = new List<UrlContent>();
        static private float jaccardThreshold = 0.5f;

        public static void AddContent (string content, string url)
        {
            UrlContent newContent = new UrlContent();
            newContent.url = url;
            newContent.content = content;
            newContent.shingles = NearDuplicateDetection.MakeShingles(content);

            if (pageContent.Count == 0)
                pageContent.Add(newContent);

            foreach (UrlContent page in pageContent.ToArray())
            {
                float jaccard = NearDuplicateDetection.CalculateJaccard(newContent.shingles, page.shingles);
                Console.WriteLine(jaccard + " " + newContent.url + " " + page.url);

                if (jaccard < jaccardThreshold)
                    pageContent.Add(newContent);
                else
                    Console.WriteLine("Duplicate found");
            }    
        }

    }
}
