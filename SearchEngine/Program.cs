using System;

namespace SearchEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            Crawler crawler = new Crawler(200, 3);
            crawler.StartCrawl();
            Indexer.CalculateIDF();
            Indexer.Calculatetfstar();
            Indexer.CalculateLength();
            Console.WriteLine("Input search term:");
            string input = Console.ReadLine();
            var resultList = QueryRanker.CosineScore(input);

            int idx = 0;
            foreach (var result in resultList)
            {
                Console.WriteLine(++idx + " ("+ result.Item1 + "): " + result.Item2);
            }

            Console.ReadKey();
        }
    }
}
