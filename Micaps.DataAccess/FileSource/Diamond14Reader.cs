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
    public class Diamond14Reader : BaseReader
    {
        public Diamond14Reader(string path)
            : base(path)
        {

        }

        public Diamond14Entity RetrieveEntity()
        {
            var result = new Diamond14Entity();

            using (var reader = new StreamReader(this.FilePath, Encoding.Default))
            {
                string line1 = string.Empty;
                string line2 = string.Empty;
                string pattern = @"\s+";
                int line1Length = 3;

                // line1
                line1 = reader.ReadLineEx()?.Trim();
                var array = Regex.Split(line1, pattern);
                if (array.Length < line1Length)
                {
                    throw new InvalidDataException(this.FilePath);
                }
                result.DiamondType = Convert.ToInt32(array[1]);
                result.Description = array[2];
                for (int i = 3; i < array.Length; i++)
                {
                    result.Description += "-" + array[i];
                }

                // header may need more than 1 line
                string[] header = new string[5];
                int index = 0;
                while (index < 5)
                {
                    line2 = reader.ReadLineEx();
                    array = Regex.Split(line2?.Trim(), pattern);
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
                // line2
                //line2 = reader.ReadLineEx()?.Trim();
                //array = Regex.Split(line2, pattern);
                //if (array.Length != line2Length)
                //{
                //    throw new InvalidDataException(this.FilePath);
                //}
                //result.Year = Convert.ToInt32(array[0]);
                //result.Month = Convert.ToInt32(array[1]);
                //result.Day = Convert.ToInt32(array[2]);
                //result.Hour = Convert.ToInt32(array[3]);
                //result.Aging = Convert.ToInt32(array[4]);

                // Lines
                var line = reader.ReadLineEx()?.Trim();
                array = Regex.Split(line, pattern);
                if (array == null || array.Length != 2 || array[0].ToLower() != "lines:")
                {
                    throw new InvalidDataException(this.FilePath);
                }
                int lineCount = Convert.ToInt32(array[1]);

                for (int i = 0; i < lineCount; i++)
                {
                    var entityLine = new Diamond14EntityLine();
                    line = reader.ReadLineEx()?.Trim();
                    array = Regex.Split(line, pattern);
                    if (array == null || array.Length != 2)
                    {
                        throw new Exception(this.FilePath);
                    }
                    entityLine.Width = Convert.ToInt32(array[0]);

                    // Points
                    int pointCount = Convert.ToInt32(array[1]);
                    for (int j = 0; j < pointCount; j++)
                    {
                        var lineItem = new Diamond14EntityLineItem();
                        lineItem.Longitude = Convert.ToSingle(ReaderHelper.GetOneString(reader));
                        lineItem.Latitude = Convert.ToSingle(ReaderHelper.GetOneString(reader));
                        lineItem.Z = Convert.ToSingle(ReaderHelper.GetOneString(reader));
                        if (lineItem.IsValid())
                        {
                            entityLine.Items.Add(lineItem);
                        }
                    }
                    //if (!string.IsNullOrWhiteSpace(reader.ReadLineEx()?.Trim()))
                    //{
                    //    throw new InvalidDataException(this.FilePath);
                    //}

                    // Label
                    line = reader.ReadLineEx()?.Trim();
                    array = Regex.Split(line, pattern);
                    if (array == null || array.Length != 2)
                    {
                        throw new InvalidDataException(this.FilePath);
                    }
                    if (array[0].ToLower() == "nolabel")
                        entityLine.LabelValue = null;
                    else
                    {
                        entityLine.LabelValue = Convert.ToInt32(array[0]);
                        // discard other parts of label
                        int count = Convert.ToInt32(array[1]);
                        for (int j = 0; j < count; j++)
                        {
                            reader.ReadLineEx();
                        }
                    }

                    result.Lines.Add(entityLine);
                }

                int contourCount = 0;
                // Remaining
                while ((line = reader.ReadLineEx()) != null)
                {
                    if (line.Trim().Contains("CLOSED_CONTOURS"))
                    {
                        array = Regex.Split(line.Trim(), pattern);
                        contourCount = Convert.ToInt32(array[1]);
                        break;
                    }
                    else
                        result.Remaining.Add(line);
                }

                // Contours
                for (int i = 0; i < contourCount; i++)
                {
                    var entityLine = new Diamond14EntityLine();
                    line = reader.ReadLineEx()?.Trim();
                    array = Regex.Split(line, pattern);
                    if (array == null || array.Length != 2)
                    {
                        throw new Exception(this.FilePath);
                    }
                    entityLine.Width = Convert.ToInt32(array[0]);

                    // Points
                    int pointCount = Convert.ToInt32(array[1]);
                    for (int j = 0; j < pointCount; j++)
                    {
                        var lineItem = new Diamond14EntityLineItem();
                        lineItem.Longitude = Convert.ToSingle(ReaderHelper.GetOneString(reader));
                        lineItem.Latitude = Convert.ToSingle(ReaderHelper.GetOneString(reader));
                        lineItem.Z = Convert.ToSingle(ReaderHelper.GetOneString(reader));
                        if (lineItem.IsValid())
                        {
                            entityLine.Items.Add(lineItem);
                        }
                    }
                    //if (!string.IsNullOrWhiteSpace(reader.ReadLineEx()?.Trim()))
                    //{
                    //    throw new InvalidDataException(this.FilePath);
                    //}

                    // Label
                    line = reader.ReadLineEx()?.Trim();
                    array = Regex.Split(line, pattern);
                    if (array == null || array.Length != 2)
                    {
                        throw new InvalidDataException(this.FilePath);
                    }
                    if (array[0].ToLower() == "nolabel")
                        entityLine.LabelValue = null;
                    else
                    {
                        entityLine.LabelValue = Convert.ToInt32(array[0]);
                        // discard other parts of label
                        int count = Convert.ToInt32(array[1]);
                        for (int j = 0; j < count; j++)
                        {
                            reader.ReadLineEx();
                        }
                    }

                    result.Contours.Add(entityLine);
                }

                // Remaining Post
                while ((line = reader.ReadLineEx()) != null)
                {
                    result.RemainingPost.Add(line);
                }
            }

            return result;
        }
    }
}