using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace SearchEngine
{
    static class NearDuplicateDetection
    {
        public static string[] MakeShingles(string input)
        {
            string[] tempArray;
            List<string> shingleList = new List<string>();
            tempArray = input.Split(' ');

            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            for (int i = 0; i < tempArray.Length - 1; i++)
            {
                tempArray[i] = rgx.Replace(tempArray[i], "");
            }

            for (int i = 0; i < tempArray.Length - 2; i++)
            {
                shingleList.Add(tempArray[i] + " " + tempArray[i + 1] + " " + tempArray[i + 2]);
            }

            return shingleList.ToArray();
        }

        public static float CalculateJaccard(string[] input1, string[] input2)
        {
            float overlap = 0;
            foreach (string shingle in input1)
            {
                foreach (string shingle2 in input2)
                {
                    if (shingle == shingle2)
                        overlap++;
                }
            }

            float union = input1.Length + input2.Length - overlap;

            return overlap / union;
        }
    }
}

