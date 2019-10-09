using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine
{
    public class PageRanker
    {
        private readonly Crawler _crawler;

        public void BuildTransitionProbabilityMatrix()
        {
            int count = _crawler.UrlQueue.Count;
            float[,] matrix = new float[count,count];
            
            for(int i = 0; i < count; i++)
            {
                for(int j = 0; j < count; j++)
                {
                    matrix[i,j] = CalculateProbability(i, j);
                }
            }
        }

        public PageRanker(Crawler crawler)
        {
            _crawler = crawler;
        }

        private float CalculateProbability(int i, int j)
        {
            throw new NotImplementedException();
            /*float result = 0f;
            int occurrence = 0;

            List<QueueEntry> queueEntryList = _crawler.UrlQueue.Values();

            foreach (var refr in _crawler.UrlQueue.Values())
            {
                refr.RefList
            }

            return result;*/
        }

    }
}
