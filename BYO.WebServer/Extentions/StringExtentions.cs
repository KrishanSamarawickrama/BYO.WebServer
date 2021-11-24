using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BYO.WebServer
{
    public static class StringExtentions
    {
        public static string LeftOf(this string src, string c)
        {
            string ret = src;
            int idx = src.IndexOf(c);
            if (idx != -1)
            {
                ret = src.Substring(0, idx);
            }
            return ret;
        }


        public static string LeftOf(this string src, char c, int n)
        {
            string ret = src;
            int idx = -1;
            while (n > 0)
            {
                idx = src.IndexOf(c, idx + 1);
                if (idx == -1)
                {
                    break;
                }
            }
            if (idx != -1)
            {
                ret = src.Substring(0, idx);
            }
            return ret;
        }

        public static string RightOf(this string src, string c)
        {
            string ret = String.Empty;
            int idx = src.IndexOf(c);
            if (idx != -1)
            {
                ret = src.Substring(idx + c.Length);
            }
            return ret;
        }

        public static string RightOf(this string src, char c, int n)
        {
            string ret = String.Empty;
            int idx = -1;
            while (n > 0)
            {
                idx = src.IndexOf(c, idx + 1);
                if (idx == -1)
                {
                    break;
                }
                --n;
            }

            if (idx != -1)
            {
                ret = src.Substring(idx + 1);
            }

            return ret;
        }

        public static string LeftOfRightmostOf(this string src, char c)
        {
            string ret = src;
            int idx = src.LastIndexOf(c);
            if (idx != -1)
            {
                ret = src.Substring(0, idx);
            }
            return ret;
        }

        public static string RightOfRightmostOf(this string src, char c)
        {
            string ret = String.Empty;
            int idx = src.LastIndexOf(c);
            if (idx != -1)
            {
                ret = src.Substring(idx + 1);
            }
            return ret;
        }

        public static string Between(this string src, char start, char end)
        {
            string ret = String.Empty;
            int idxStart = src.IndexOf(start);
            if (idxStart != -1)
            {
                ++idxStart;
                int idxEnd = src.IndexOf(end, idxStart);
                if (idxEnd != -1)
                {
                    ret = src.Substring(idxStart, idxEnd - idxStart);
                }
            }
            return ret;
        }

        public static char Rightmost(this string src)
        {
            char c = '\0';
            if (src.Length > 0)
            {
                c = src[src.Length - 1];
            }
            return c;
        }
    }
}
