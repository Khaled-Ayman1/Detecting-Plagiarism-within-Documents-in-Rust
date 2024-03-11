using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentDistance
{
    class DocDistance
    {

        // *****************************************
        // DON'T CHANGE CLASS OR FUNCTION NAME
        // YOU CAN ADD FUNCTIONS IF YOU NEED TO
        // *****************************************
        /// <summary>
        /// Write an efficient algorithm to calculate the distance between two documents
        /// </summary>
        /// <param name="doc1FilePath">File path of 1st document</param>
        /// <param name="doc2FilePath">File path of 2nd document</param>
        /// <returns>The angle (in degree) between the 2 documents</returns>
        public static double CalculateDistance(string doc1FilePath, string doc2FilePath)
        {
            Task<Dictionary<string, int>> doc1Task = Task.Run(() => ProcessDoc(doc1FilePath));
            Task<Dictionary<string, int>> doc2Task = Task.Run(() => ProcessDoc(doc2FilePath));

            Task.WaitAll(doc1Task, doc2Task);

            Dictionary<string, int> doc1Strings = doc1Task.Result;
            Dictionary<string, int> doc2Strings = doc2Task.Result;


            double mult = 0.0;
            double powD1 = 0.0;
            double powD2 = 0.0;

            object lockObject = new object();

            Parallel.ForEach(doc1Strings, pair =>
            {
                string key = pair.Key;
                int doc1Value = pair.Value;

                int doc2Value;
                if (doc2Strings.TryGetValue(key, out int value))
                {
                    doc2Value = value;
                }
                else
                {
                    doc2Value = 0;
                }

                lock (lockObject)
                {
                    mult += Math.BigMul(doc1Value, doc2Value);
                    powD1 += Math.Pow(doc1Value, 2);
                    powD2 += Math.Pow(doc2Value, 2);
                }
            });

            Parallel.ForEach(doc2Strings, pair =>
            {
                string key = pair.Key;
                if (!doc1Strings.ContainsKey(key))
                {
                    int doc2Value = pair.Value;

                    lock (lockObject)
                    {
                        powD2 += Math.Pow(doc2Value, 2);
                    }
                }
            });

            return (Math.Acos(mult / Math.Sqrt(powD1 * powD2))) * (180 / Math.PI);            
        }


        private static Dictionary<string, int> ProcessDoc(string docFilePath)
        {

            string docRead = File.ReadAllText(docFilePath).ToLower();

            Dictionary<string, int> docStrings = new Dictionary<string, int>();

            StringBuilder docString = new StringBuilder();

            foreach (char c in docRead)
            {
                if (char.IsLetterOrDigit(c))
                {
                    docString.Append(c);
                }
                else
                {
                    string word = docString.ToString();
                    if (!string.IsNullOrEmpty(word))
                    {
                        if (docStrings.TryGetValue(word, out int docValue))
                        {
                            docStrings[word] = docValue + 1;
                        }
                        else
                        {
                            docStrings[word] = 1;
                        }

                        docString.Clear();
                    }
                }
            }
            string remainWord = docString.ToString();
            if (!string.IsNullOrEmpty(remainWord))
            {
                if (docStrings.TryGetValue(remainWord, out int docValue))
                {
                    docStrings[remainWord] = docValue + 1;
                }
                else
                {
                    docStrings[remainWord] = 1;
                }

                docString.Clear();
            }

            return docStrings;
        }
    }
}
