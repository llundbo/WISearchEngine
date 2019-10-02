using System;

namespace SearchEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            Crawler crawler = new Crawler(200, 8);
            crawler.StartCrawl();
            Indexer.CalculateIDF();
            Indexer.Calculatetfstar();

            Console.ReadKey();
        }
    }
}
