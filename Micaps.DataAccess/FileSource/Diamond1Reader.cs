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
    public class Diamond1Reader : BaseReader
    {
        public Diamond1Reader(string path)
            : base(path)
        {

        }

        public Diamond1Entity RetrieveEntity()
        {
            var result = new Diamond1Entity();
            result.Items = new List<Diamond1EntityItem>();

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
                string[] header = new string[5];
                int index = 0;
                while (index < 5)
                {
                    line = reader.ReadLine();
                    array = Regex.Split(line?.Trim(), pattern);
                    if (array == null)
                    {
                        throw new InvalidDataException(this.FilePath);
                    }

                    foreach(var item in array)
                    {
                        header[index++] = item;
                    }
                }
                result.Year = Convert.ToInt32(header[0]);
                result.Month = Convert.ToInt32(header[1]);
                result.Day = Convert.ToInt32(header[2]);
                result.Hour = Convert.ToInt32(header[3]);
                result.StationAmount = Convert.ToInt32(header[4]);

                // Items
                while ((line = reader.ReadLine()) != null)
                {
                    array = Regex.Split(line?.Trim(), pattern);
                    if (array == null || array.Length < 24)
                    {
                        throw new InvalidDataException(this.FilePath);
                    }

                    var item = new Diamond1EntityItem()
                    {
                        StationNumber = Convert.ToInt64(array[0]),
                        Longitude = Convert.ToSingle(array[1]),
                        Latitude = Convert.ToSingle(array[2]),
                        Elevation = Convert.ToSingle(array[3]),
                        StationLevel = Convert.ToInt32(array[4]),
                        CloudAmount = Convert.ToInt32(array[5]),
                        WindAngle = Convert.ToInt32(array[6]),
                        WindSpeed = Convert.ToInt32(array[7]),
                        AirPressure = Convert.ToInt32(array[8]),
                        ThreehoursAP = Convert.ToInt32(array[9]),
                        LastWeather1 = Convert.ToInt32(array[10]),
                        LastWeather2 = Convert.ToInt32(array[11]),
                        SixhoursRain = Convert.ToSingle(array[12]),
                        DiYunZhuang = Convert.ToInt32(array[13]),
                        DiYunLiang = Convert.ToInt32(array[14]),
                        DiYunGao = Convert.ToInt32(array[15]),
                        DewPoint = Convert.ToInt32(array[16]),
                        Visibility = Convert.ToSingle(array[17]),
                        CurrentWeather = Convert.ToInt32(array[18]),
                        Temperature = Convert.ToSingle(array[19]),
                        ZhongYunZhuang = Convert.ToInt32(array[20]),
                        GaoYunZhuang = Convert.ToInt32(array[21]),
                        Flag1 = Convert.ToInt32(array[22]),
                        Flag2 = Convert.ToInt32(array[23])
                    };
                    if (item.Flag1 == 1 && item.Flag2 == 2)
                    {
                        if (array.Length != 26)
                        {
                            throw new InvalidDataException(this.FilePath);
                        }
                        item.TemperatureDiff24 = Convert.ToInt32(array[24]);
                        item.PressureDiff24 = Convert.ToInt32(array[25]);
                    }
                    result.Items.Add(item);
                }
            }

            return result;
        }

    }
}
