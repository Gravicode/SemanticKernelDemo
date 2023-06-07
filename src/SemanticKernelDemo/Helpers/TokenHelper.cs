using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelDemo.Helpers
{
    public class TokenHelper
    {
        public static void CheckMaxToken(int MaxToken, string Content)
        {
            var count = CountWords(Content);
            var token = 4 * count / 3;
            if (token > MaxToken) throw new Exception("Please reduce text, already exceeded the limit.");
        }
        public static unsafe int CountWords(string s)
        {
            int count = 0;
            fixed (char* ps = s)
            {
                int len = s.Length;
                bool inWord = false;
                char* pc = ps;
                while (len-- > 0)
                {
                    if (char.IsWhiteSpace(*pc++))
                    {
                        if (!inWord)
                        {
                            inWord = true;
                        }
                    }
                    else
                    {
                        if (inWord)
                        {
                            inWord = false;
                            count++;
                        }
                    }
                    if (len == 0)
                    {
                        if (inWord)
                        {
                            count++;
                        }
                    }
                }
            }
            return count;
        }
    }
}
