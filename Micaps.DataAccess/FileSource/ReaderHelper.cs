using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Pmss.Micaps.DataAccess.FileSource
{
    public static class ReaderHelper
    {
        public static string GetOneString(StreamReader reader)
        {
            List<Char> list = new List<char>();

            int tmp = -1;
            while ((tmp = reader.Read()) != -1 && !IsValidChar(tmp))
            {

            }
            list.Add((char)tmp);
            while ((tmp = reader.Peek()) != -1 && IsValidChar(tmp))
            {
                tmp = reader.Read();
                list.Add((char)tmp);
            }

            return new string(list.ToArray());
        }

        public static bool IsValidChar(int c)
        {
            if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
                return false;
            return true;
        }
    }
}
