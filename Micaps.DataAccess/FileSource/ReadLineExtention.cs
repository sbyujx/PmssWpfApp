using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Pmss.Micaps.DataAccess.FileSource
{
    public static class ReadLineExtention
    {
        public static string ReadLineEx(this StreamReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            string result = null;

            while ((result = reader.ReadLine()?.Trim()) == string.Empty) ;

            return result;
        }
    }
}
