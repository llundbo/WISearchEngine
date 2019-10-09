using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SearchEngine
{
    static class Indexer
    {
        private static List<string> StopWords_Danish = GetStopWords("../../../danish_stopwords.txt");
        public static Dictionary<string, WordData> IndexedWords = new Dictionary<string, WordData>();

        public static string RemoveStopWords(string input)
        {
            string[] wordList = input.Split(' ');

            int idx = 0;
            foreach (string word in wordList)
            {
                if (StopWords_Danish.Contains(word))
                    wordList[idx] = "";

                idx++;
            }

            StringBuilder sb = new StringBuilder();
            foreach (string str in wordList)
            {
                if (!string.IsNullOrWhiteSpace(str))
                    sb.Append(str + " ");
            }

            return sb.ToString().Trim();
        }

        private static List<string> GetStopWords(string filepath)
        {
            List<string> result = new List<string>();
            using (StreamReader reader = new StreamReader(filepath))
            {
                while (!reader.EndOfStream)
                    result.Add(reader.ReadLine());
            }

            return result;
        }

        public static void AddWordsToIndexList(string content, int pageIndex)
        {
            string[] contentArray = content.Split(' ');

            foreach (string word in contentArray)
            {
                try
                {
                    if (!IndexedWords.ContainsKey(word))
                    {
                        IndexedWords.Add(word, new WordData(pageIndex, new DocumentStat()));  
                    }
                    else
                    {
                        if (!IndexedWords[word].DocumentStatList.Exists(y => y.Item1 == pageIndex))
                            IndexedWords[word].DocumentStatList.Add(Tuple.Create(pageIndex, new DocumentStat()));
                        else
                        {
                            foreach (var tuple in IndexedWords[word].DocumentStatList)
                            {
                                if (tuple.Item1 == pageIndex)
                                {
                                    tuple.Item2.WordFreq++;
                                    break;
                                }
                            }
                        }
                    }    
                }
                catch(ArgumentException) // in case of race condition
                {
                    if (!IndexedWords[word].DocumentStatList.Exists(y => y.Item1 == pageIndex))
                        IndexedWords[word].DocumentStatList.Add(Tuple.Create(pageIndex, new DocumentStat()));
                    else
                    {
                        foreach (var tuple in IndexedWords[word].DocumentStatList)
                        {
                            if (tuple.Item1 == pageIndex)
                            {
                                tuple.Item2.WordFreq++;
                                break;
                            }
                        }
                    }
                }
            }
        }

        public static void Calculatetfstar()
        {
            foreach (var wordData in IndexedWords.Values)
            {
                foreach (var tuple in wordData.DocumentStatList)
                {
                    tuple.Item2.tfStar = (decimal)(1 + Math.Log10(tuple.Item2.WordFreq));
                    tuple.Item2.tf_idf = tuple.Item2.WordFreq * wordData.idf;
                    tuple.Item2.tfstar_idf = tuple.Item2.tfStar * wordData.idf;
                }
            }
        }

        public static void CalculateIDF()
        {
            foreach (var wordData in IndexedWords.Values)
            {
                wordData.idf = (decimal)(Math.Log10(Crawler.NUMBEROFPAGES / wordData.DocumentStatList.Count));
            }
        }

        public static void CalculateLength()
        {
            decimal sum = 0M;
            foreach (var urlContent in ContentHandler.pageContent)
            {
                foreach (string str in urlContent.Content.Split(' '))
                {
                    try
                    {
                        sum += (decimal)Math.Pow((double)IndexedWords[str].idf, 2f);
                    }
                    catch(KeyNotFoundException)
                    {
                        continue;
                    }
                }
                urlContent.Length = (float)Math.Sqrt((double)sum);
            }
        }
    }
}
