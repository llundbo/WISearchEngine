﻿using System;
using System.Threading;

namespace SearchEngine
{
    class Program
    {
        static void Main()
        {
            Crawler crawler = new Crawler(50, 4);
            crawler.StartCrawl();
            Thread.Sleep(3000);
            Indexer.CalculateIDF();
            Indexer.Calculatetfstar();
            Indexer.CalculateLength();

            PageRanker pageRanker = new PageRanker(crawler);

            var matrix = pageRanker.BuildTransitionProbabilityMatrix();

            for (int i = 0; i < crawler.UrlQueue.Count; i++)
            {
                for (int j = 0; j < crawler.UrlQueue.Count; j++)
                {
                    Console.Write(matrix[i, j]);
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }

            Console.ReadLine();

            while (true)
            {
                Console.WriteLine("Input search term:");
                string input = Console.ReadLine();
                var resultList = QueryRanker.CosineScore(input);

                if (resultList.Count == 0)
                    Console.WriteLine("No search results found..");
                else
                {
                    for (int i = 0; i < Math.Min(10, resultList.Count); i++)
                    {
                        Console.WriteLine((i + 1) + " (" + resultList[i].Item1 + "): " + resultList[i].Item2);
                    }
                }
            }
        }
    }
}
