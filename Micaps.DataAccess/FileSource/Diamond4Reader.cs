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
    public class Diamond4Reader : BaseReader
    {
        public Diamond4Reader(string path) : base(path)
        {
        }

        public Diamond4Entity RetrieveEntity()
        {
            var result = new Diamond4Entity();

            string line = string.Empty;
            string[] array = null;
            string pattern = @"\s+";

            using (var reader = new StreamReader(this.FilePath,Encoding.Default))
            {
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
                string[] header = new string[19];
                int index = 0;
                while (index < 19)
                {
                    line = reader.ReadLine();
                    array = Regex.Split(line?.Trim(), pattern);
                    if (array == null)
                    {
                        throw new InvalidDataException(this.FilePath);
                    }

                    foreach (var item in array)
                    {
                        header[index++] = item;
                    }
                }
                result.Year = Convert.ToInt32(header[0]);
                result.Month = Convert.ToInt32(header[1]);
                result.Day = Convert.ToInt32(header[2]);
                result.Hour = Convert.ToInt32(header[3]);
                result.Aging = Convert.ToInt32(header[4]);
                result.Level = Convert.ToInt32(header[5]);
                result.LonGridSize = Convert.ToSingle(header[6]);
                result.LatGridSize = Convert.ToSingle(header[7]);
                result.LonStart = Convert.ToSingle(header[8]);
                result.LonEnd = Convert.ToSingle(header[9]);
                result.LatStart = Convert.ToSingle(header[10]);
                result.LatEnd = Convert.ToSingle(header[11]);
                result.latGridCount = Convert.ToInt32(header[12]);
                result.LonGridCount = Convert.ToInt32(header[13]);
                result.ContourInterval = Convert.ToSingle(header[14]);
                result.ContourStart = Convert.ToSingle(header[15]);
                result.ContourEnd = Convert.ToSingle(header[16]);
                result.SmoothFactor = Convert.ToSingle(header[17]);
                result.BoldFactor = Convert.ToSingle(header[18]);

                // items
                result.Items = new float[result.LonGridCount, result.latGridCount];
                for (int i = 0; i < result.LonGridCount; i++)
                {
                    int j = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        array = Regex.Split(line?.Trim(), pattern);
                        if (array == null)
                        {
                            throw new InvalidDataException(this.FilePath);
                        }

                        foreach (var item in array)
                        {
                            result.Items[i,j++] = Convert.ToSingle(item);
                        }
                        if (j == result.latGridCount)
                            break;
                    }
                }
            }

            return result;
        }
    }
}
