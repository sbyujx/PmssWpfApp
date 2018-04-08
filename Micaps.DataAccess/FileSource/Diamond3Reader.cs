using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pmss.Micaps.DataEntities.FileSource;
using System.IO;
using System.Text.RegularExpressions;

namespace Pmss.Micaps.DataAccess.FileSource
{
    public class Diamond3Reader : BaseReader
    {
        public Diamond3Reader(string path) : base(path)
        {
        }

        public Diamond3Entity RetrieveEntity()
        {
            var result = new Diamond3Entity();
            result.Items = new List<Diamond3EntityItem>();

            using (var reader = new StreamReader(this.FilePath, Encoding.Default))
            {
                string line = string.Empty;
                string[] array = null;
                string pattern = @"\s+";

                // line1
                line = reader.ReadLine();
                array = Regex.Split(line?.Trim(), pattern);
                if (array == null || array.Length <= 2)
                {
                    throw new InvalidDataException(this.FilePath);
                }
                result.DiamondType = Convert.ToInt32(array[1]);
                result.Description = array[2];
                for (int i = 3; i < array.Length; i++)
                {
                    result.Description += " " + array[i];
                }

                // header may need more than 1 line
                // Get 6 string, the last one is count
                string[] header = new string[6];
                for (int i = 0; i < 6; i++)
                {
                    header[i] = GetOneString(reader);
                }
                result.Year = Convert.ToInt32(header[0]);
                result.Month = Convert.ToInt32(header[1]);
                result.Day = Convert.ToInt32(header[2]);
                result.Level = Convert.ToInt32(header[3]);

                int lineCount = Convert.ToInt32(header[5]);
                for (int i = 0; i < lineCount + 2; i++)
                    GetOneString(reader);

                int pointCount = Convert.ToInt32(GetOneString(reader));
                for (int i = 0; i < pointCount * 2 + 1; i++)
                    GetOneString(reader);
                result.StationAmount = Convert.ToInt32(GetOneString(reader));
                if (!string.IsNullOrWhiteSpace(reader.ReadLine()?.Trim()))
                {
                    throw new InvalidDataException(this.FilePath);
                }

                // find line items
                //while ((line = reader.ReadLine()) != null)
                //{
                //    array = Regex.Split(line?.Trim(), pattern);
                //    if (array != null && array.Length == 2 && 1 == Convert.ToInt32(array[0]))
                //    {
                //        result.StationAmount = Convert.ToInt32(array[1]);
                //        break;
                //    }
                //}

                while ((line = reader.ReadLine()) != null)
                {
                    array = Regex.Split(line?.Trim(), pattern);
                    if (array == null || array.Length < 5)
                    {
                        throw new InvalidDataException(this.FilePath);
                    }

                    var item = new Diamond3EntityItem
                    {
                        StationNumber = Convert.ToInt32(array[0]),
                        Longitude = Convert.ToSingle(array[1]),
                        Latitude = Convert.ToSingle(array[2]),
                        Elevation = Convert.ToSingle(array[3]),
                        StationValue = array[4]
                    };
                    result.Items.Add(item);
                }
            }

            return result;
        }

        private string GetOneString(StreamReader reader)
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

        private bool IsValidChar(int c)
        {
            if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
                return false;
            return true;
        }
    }
}
