using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchEngine
{
    public class PageRanker
    {
        private readonly Crawler _crawler;
        private readonly string[] _keyslist;

        public float[,] BuildTransitionProbabilityMatrix()
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

            return matrix;
        }

        public PageRanker(Crawler crawler)
        {
            _crawler = crawler;
            _keyslist = _crawler.UrlQueue.Keys().ToArray();
        }

        private float CalculateProbability(int i, int j)
        {
            int pointToListCount = _crawler.UrlQueue.Read(_keyslist[i]).PointToList.Count;

            if (_crawler.UrlQueue.Read(_keyslist[i]).PointToList.Contains(_keyslist[j]))
                return pointToListCount == 0 ? 0 : 1 / pointToListCount;
            else return 0;
        }

    }
}
