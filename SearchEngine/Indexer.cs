using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SearchEngine
{
    static class Indexer
    {
        private static List<string> StopWords_Danish = GetStopWords("../../../danish_stopwords.txt");
        private static Dictionary<string, List<Tuple<int, DocumentStat>>> IndexedWords = new Dictionary<string, List<Tuple<int, DocumentStat>>>();

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
                        IndexedWords.Add(word, new List<Tuple<int, DocumentStat>>{Tuple.Create(pageIndex, new DocumentStat())});  
                    }
                    else
                    {
                        if (!IndexedWords[word].Exists(x => x.Item1 == pageIndex))
                            IndexedWords[word].Add(Tuple.Create(pageIndex, new DocumentStat()));
                        else
                            IndexedWords[word].Find(x => x.Item1 == pageIndex).Item2.WordFreq++;
                    }    
                }
                catch(ArgumentException) // in case of race condition
                {
                    if (!IndexedWords[word].Exists(x => x.Item1 == pageIndex))
                        IndexedWords[word].Add(Tuple.Create(pageIndex, new DocumentStat()));
                    else
                        IndexedWords[word].Find(x => x.Item1 == pageIndex).Item2.WordFreq++;
                }
            }
        }

        public static void CalcIdf()
        {
            foreach(var tupleList in IndexedWords.Values)
            {
                foreach(var tuple in tupleList)
                {
                    //tuple.Item2.IdfValue = Math.Log10(tupleList.Count/)
                }
            }
        }
    }
}
