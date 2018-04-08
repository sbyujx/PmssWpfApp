using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace Pmss.Micaps.Render.Config
{
    public class LevelSettingList
    {
        //public LevelSettingList(string name)
        //{
        //    dict = new SortedDictionary<int, Color>();
        //    Name = name;
        //    dict.Add(int.MaxValue, Colors.Black);
        //    Update();
        //}
        public LevelSettingList(string name, string levels = null, string colors = null)
        {
            dict = new SortedDictionary<int, Color>();
            Name = name;

            if (string.IsNullOrEmpty(levels) || string.IsNullOrEmpty(colors))
            {

                dict.Add(int.MaxValue, Colors.Black);
            }
            else
            {
                var levelArray = levels.Split(',');
                var colorArray = colors.Split(',');
                if (levelArray == null || colorArray == null)
                {
                    dict.Add(int.MaxValue, Colors.Black);
                    Update();
                    return;
                }

                if (levelArray.Length != colorArray.Length)
                {
                    throw new FormatException(levels + " " + colors);
                }

                for (int i = 0; i < levelArray.Length; i++)
                {
                    int value;
                    if (levelArray[i] == "max")
                        value = int.MaxValue;
                    else
                        value = Convert.ToInt32(levelArray[i]);
                    Color color = (Color)ColorConverter.ConvertFromString(colorArray[i]);
                    dict.Add(value, color);
                }
            }
            Update();
        }
        public string GetLevelsString()
        {
            UpdateBack();
            string result = string.Empty;
            foreach (var key in dict.Keys)
            {
                if (key != int.MaxValue)
                {
                    result += $"{key},";
                }
                else
                {
                    result += "max";
                }
            }
            return result;
        }
        public string GetColorsString()
        {
            UpdateBack();
            string result = string.Empty;
            foreach (var color in dict.Values)
            {
                result += $"{color},";
            }
            result = result.TrimEnd(',');

            return result;
        }
        public Tuple<string, string> GetSettingString()
        {
            string levels = string.Empty;
            string colors = string.Empty;

            foreach (var item in dict)
            {
                if (levels == string.Empty)
                    levels = item.Key.ToString();
                else
                    levels += "," + item.Key.ToString();

                if (colors == string.Empty)
                    colors = item.Value.ToString();
                else
                    colors += "," + item.Value.ToString();
            }

            return new Tuple<string, string>(levels, colors);
        }
        public void AddOrUpdateLevel(int value)
        {
            AddOrUpdateLevel(value, Colors.Black);
        }
        public void AddOrUpdateLevel(int value, Color color)
        {
            if (value != int.MinValue && value != int.MaxValue)
            {
                UpdateBack();
                dict[value] = color;
                Update();
            }
        }
        public void RemoveLevel(int value)
        {
            if (value != int.MaxValue)
            {
                UpdateBack();
                dict.Remove(value);
                Update();
            }
        }
        public Color GetValueColor(int value)
        {
            Color result = Colors.Black;
            foreach (var item in LevelSettingVmList)
            {
                if (value >= item.StartValue && value < item.EndValue)
                {
                    result = item.LevelColor;
                }
            }
            return result;
        }

        public ObservableCollection<LevelSetting> LevelSettingVmList { get; set; } = new ObservableCollection<LevelSetting>();
        public string Name { get; set; }

        private void Update()
        {
            if (dict.Count() == 0)
            {
                dict.Add(int.MaxValue, Colors.Black);
            }
            this.LevelSettingVmList.Clear();

            int lastValue = int.MinValue;
            foreach (var item in dict)
            {
                var vm = new LevelSetting
                {
                    StartValue = lastValue,
                    EndValue = item.Key,
                    LevelColor = item.Value
                };
                this.LevelSettingVmList.Add(vm);
                lastValue = vm.EndValue;
            }
        }
        // Fix color not sync bug
        // Actually we don't need dict to store color
        private void UpdateBack()
        {
            foreach (var item in LevelSettingVmList)
            {
                dict[item.EndValue] = item.LevelColor;
            }
        }
        private SortedDictionary<int, Color> dict;
    }
}
