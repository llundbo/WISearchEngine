using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SearchEngine
{
    static class Indexer
    {
        private static List<string> StopWords_Danish = GetStopWords("../../../danish_stopwords.txt");
        private static Dictionary<string, List<int>> IndexedWords = new Dictionary<string, List<int>>();

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
                        IndexedWords.Add(word, new List<int> { pageIndex });
                    else
                    if (!IndexedWords[word].Contains(pageIndex))
                        IndexedWords[word].Add(pageIndex);
                }
                catch(ArgumentException)
                {
                    if (!IndexedWords[word].Contains(pageIndex))
                        IndexedWords[word].Add(pageIndex);
                }
            }
        }
    }
}
