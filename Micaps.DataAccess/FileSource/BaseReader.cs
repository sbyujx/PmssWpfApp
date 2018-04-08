using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pmss.Micaps.Core.Enums;
using System.IO;
using System.Text.RegularExpressions;

namespace Pmss.Micaps.DataAccess.FileSource
{
    public class BaseReader
    {
        public static DiamondType GetDiamondType(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            using (var reader = new StreamReader(path, Encoding.Default))
            {
                string pattern = @"\s+";
                int firstLineLength = 3;

                string firstline = reader.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(firstline))
                {
                    var stringArray = Regex.Split(firstline, pattern);
                    if (stringArray.Length < firstLineLength)
                    {
                        throw new InvalidDataException(path);
                    }
                    try
                    {
                        return (DiamondType)Convert.ToInt32(stringArray[1]);
                    }
                    catch
                    {
                        throw new Exception("不支持的文件格式.");
                    }
                }
            }

            return DiamondType.NotExist;
        }

        public BaseReader(string path)
        {
            this.FilePath = path;
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }
        }
        public string FilePath { get; set; }

    }
}
