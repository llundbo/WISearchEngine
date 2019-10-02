using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine
{
    public class WordData
    {
        List<Tuple<int, DocumentStat>> DocumentList = new List<Tuple<int, DocumentStat>>();
        decimal idf = 0M;
    }
}
