using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Pmss.Micaps.Render.Config
{
    public class LevelSetting
    {
        public int StartValue { get; set; }
        public int EndValue { get; set; }
        public Color LevelColor { get; set; }
        public string Value
        {
            get
            {
                if (EndValue == int.MaxValue)
                    return string.Empty;
                else
                    return EndValue.ToString();
            }
        }
        public string Description
        {
            get
            {
                if (StartValue == int.MinValue && EndValue == int.MaxValue)
                    return "所有";
                else if (StartValue == int.MinValue)
                    return $"<{EndValue}";
                else if (EndValue == int.MaxValue)
                    return $">={StartValue}";
                else
                    return $">={StartValue},<{EndValue}";
            }
        }
    }
}
