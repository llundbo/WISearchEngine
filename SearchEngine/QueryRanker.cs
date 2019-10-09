using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchEngine
{
    public static class QueryRanker
    {
        public static List<Tuple<float, string>> CosineScore(string query)
        {
            float[] scores = new float[ContentHandler.pageContent.Count];

            string[] querySplit = query.Trim().ToLower().Split(' ');

            foreach (string term in querySplit)
            {
                foreach (var urlContent in ContentHandler.pageContent)
                {
                    if(urlContent.Content.Contains(term))
                    {
                        int scoreidx = ContentHandler.pageContent.IndexOf(urlContent);
                        try
                        {
                            scores[scoreidx] += (float)Indexer.IndexedWords[term].DocumentStatList.Find(x => x.Item1 == scoreidx).Item2.tfstar_idf * CalculateQueryWeights(term);
                        }
                        catch (NullReferenceException)
                        {
                            continue;
                        }
                        catch (KeyNotFoundException)
                        {
                            continue;
                        }
                    }
                }
            }

            for (int i = 0; i < scores.Length; i++)
            {
                scores[i] = (scores[i] / ContentHandler.pageContent[i].Length);
            }

            List<Tuple<float, string>> resultList = new List<Tuple<float, string>>();

            int idx = 0;
            foreach (var score in scores)
            {
                if (score > 0)
                    resultList.Add(Tuple.Create(score, ContentHandler.pageContent[idx].Url));
                idx++;
            }

            resultList.Sort(ResultSort);
            resultList.Reverse();
            return resultList;
        }

        private static int ResultSort(Tuple<float, string> t1, Tuple<float, string> t2)
        {
            return t1.Item1.CompareTo(t2.Item1);
        }

        private static float CalculateQueryWeights(string subQuery)
        {
            if (Indexer.IndexedWords.ContainsKey(subQuery))
                return (float)Indexer.IndexedWords[subQuery].idf;
            return 0f;
        }
    }
}
