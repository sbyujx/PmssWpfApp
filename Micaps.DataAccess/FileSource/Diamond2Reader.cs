using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Pmss.Micaps.DataEntities.FileSource;
using System.Text.RegularExpressions;
using Pmss.Micaps.Core.Enums;

namespace Pmss.Micaps.DataAccess.FileSource
{
    public class Diamond2Reader : BaseReader
    {
        public Diamond2Reader(string path)
            : base(path)
        {

        }

        public Diamond2Entity RetrieveEntity()
        {
            var entity = new Diamond2Entity();
            entity.Items = new List<Diamond2EntityItem>();

            using (var reader = new StreamReader(this.FilePath, Encoding.Default))
            {
                string line1 = string.Empty;
                string line = string.Empty;
                string pattern = @"\s+";
                int line1Length = 3;// type, description
                int lineLength = 10;// Items

                // Get first line
                line1 = reader.ReadLine();
                var array = Regex.Split(line1?.Trim(), pattern);
                if (array == null || array.Length < line1Length)
                {
                    throw new InvalidDataException(this.FilePath + " Line1");
                }
                entity.DiamondType = Convert.ToInt32(array[1]);
                entity.Description = array[2];
                for (int i = 3; i < array.Length; i++)
                {
                    entity.Description += " " + array[i];
                }

                // header may need more than 1 line
                string[] header = new string[6];
                int index = 0;
                while (index < 6)
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
                entity.Year = Convert.ToInt32(header[0]);
                entity.Month = Convert.ToInt32(header[1]);
                entity.Day = Convert.ToInt32(header[2]);
                entity.Hour = Convert.ToInt32(header[3]);
                entity.Level = Convert.ToInt32(header[4]);
                entity.StationAmount = Convert.ToInt32(header[5]);

                // Get Items
                while ((line = reader.ReadLine()) != null)
                {
                    array = Regex.Split(line, pattern);
                    if (array == null || array.Length != lineLength)
                    {
                        throw new InvalidDataException(this.FilePath + " Items");
                    }

                    var item = new Diamond2EntityItem
                    {
                        StationNumber = Convert.ToInt64(array[0]),
                        Longitude = Convert.ToSingle(array[1]),
                        Latitude = Convert.ToSingle(array[2]),
                        Elevation = Convert.ToSingle(array[3]),
                        StationLevel = Convert.ToInt32(array[4]),
                        Height = Convert.ToSingle(array[5]),
                        Temperature = Convert.ToSingle(array[6]),
                        TemperatureDiff = Convert.ToSingle(array[7]),
                        WindAngle = Convert.ToSingle(array[8]),
                        WindSpeed = Convert.ToSingle(array[9])
                    };
                    entity.Items.Add(item);
                }
            }

            return entity;
        }
    }
}
