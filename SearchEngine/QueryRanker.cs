using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine
{
    public class QueryRanker
    {
        public List<string> CosineScore(string query)
        {
            throw new NotImplementedException();
            /*float[] scores = new float[Crawler.NUMBEROFPAGES];
            float[] length = new float[Crawler.NUMBEROFPAGES];

            string[] querySplit = query.Trim().ToLower().Split(' ');

            foreach (string term in querySplit)
            {
                CalculateWtq()
            }

            return*/
        }
    }
}
