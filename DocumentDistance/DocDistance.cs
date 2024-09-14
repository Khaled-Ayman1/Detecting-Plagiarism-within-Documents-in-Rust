using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

            // TODO comment the following line THEN fill your code here
            // throw new NotImplementedException();

            /*
             * best result:
             * Average execution time (ms) = 196.2
             * Max execution time (ms) = 440
             */

            //variable initialization
            string docString1 = "";
            string docString2 = "";
            double d0 = 0;
            double d1 = 0;
            double d2 = 0;

            //reading file into a string and converting it to lower case in parallel
            Parallel.Invoke(
                () =>
                {
                    using (StreamReader streamReader = new StreamReader(doc1FilePath, Encoding.UTF8))
                    {
                        docString1 = streamReader.ReadToEnd().ToLower();
                    }
                },
                () =>
                {
                    using (StreamReader streamReader = new StreamReader(doc2FilePath, Encoding.UTF8))
                    {
                        docString2 = streamReader.ReadToEnd().ToLower();
                    }
                }
            );

            //spliting the string into a dictionary in parallel
            Dictionary<long, int> doc1hashMap = new Dictionary<long, int>();
            Dictionary<long, int> doc2hashMap = new Dictionary<long, int>();

            Parallel.Invoke(
                () =>
                {
                    doc1hashMap = SplitStringToDictionary(docString1);
                },
                () =>
                {
                    doc2hashMap = SplitStringToDictionary(docString2);
                }
            );

            //calculate d0, d1, d2 in parallel
            //had to use Math functions because regular operations are not accurate
            Parallel.Invoke(
                () =>
                {
                    //this way is faster than .Intersect()
                    //iterate over the smaller hashMap in size --> less iterations 
                    if (doc1hashMap.Count > doc2hashMap.Count)
                    {
                        foreach (long s in doc2hashMap.Keys)
                        {
                            if (doc1hashMap.ContainsKey(s))
                            {
                                d0 += Math.BigMul(doc1hashMap[s], doc2hashMap[s]);

                            }
                        }
                    }
                    else
                    {
                        foreach (long s in doc1hashMap.Keys)
                        {
                            if (doc2hashMap.ContainsKey(s))
                            {
                                d0 += Math.BigMul(doc1hashMap[s], doc2hashMap[s]);

                            }
                        }
                    }
                },
                () =>
                {
                    foreach (int i in doc1hashMap.Values)
                    {
                        d1 += Math.Pow(i, 2);
                    }
                },
                () =>
                {
                    foreach (int i in doc2hashMap.Values)
                    {
                        d2 += Math.Pow(i, 2);
                    }
                }
            );
            
            //calculate angle
            return Math.Acos(d0 / (Math.Sqrt(d1 * d2))) * (180 / Math.PI);
            
        }

        /// <summary>
        /// a function that does the splitting and forming the dictionary due to split function being slower and it is not working right and not splitting when '?' occurs, and Regex.split being so slow. 
        /// </summary>
        /// <param name="document">document to be splitted</param>
        /// <returns>a dictionary of alphanumerical and the number of occurrences of each alphanumerical value</returns>
        private static Dictionary<long,int> SplitStringToDictionary(string docString)
        {
            Dictionary<long, int> doc = new Dictionary<long, int>();
            char[] characters = docString.ToCharArray();
            int start = 0;
            string s;

            //iterate over the document charachters
            for (int i = 0; i < characters.Length; i++)
            {
                //if non alphanumerical 
                if (!char.IsLetterOrDigit(characters[i]))
                {
                    //found a separator add the alphanumerical to the document, or else it is only a separator so skip 
                    if (start < i)
                    {
                        //add characters from start to the separator 
                        s = new string(characters, start, i - start);
                        if (doc.ContainsKey(s.GetHashCode()))
                        {
                            doc[s.GetHashCode()] += 1;
                        }
                        else
                        {
                            doc.Add(s.GetHashCode(), 1);
                        }
                    }
                    start = i + 1;
                }
            }
            //add last alphanumerical in the document if exists
            if (start < characters.Length)
            {
                s = new string(characters, start, characters.Length - start);
                if (doc.ContainsKey(s.GetHashCode()))
                {
                    doc[s.GetHashCode()] += 1;
                }
                else
                {
                    doc.Add(s.GetHashCode(), 1);
                }
            }

            return doc;
        }
    }
}
