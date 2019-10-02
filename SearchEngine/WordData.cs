using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine
{
    public class WordData
    {
        public List<Tuple<int, DocumentStat>> DocumentStatList = new List<Tuple<int, DocumentStat>>();
        public decimal idf = 0M;

        public WordData(int pageIndex, DocumentStat doc)
        {
            DocumentStatList.Add(Tuple.Create(pageIndex, doc));
        }
    }
}
