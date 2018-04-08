using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Pmss.Micaps.Render.Config
{
    public class Level<T>
    {
        /// <summary>
        /// Greater or Equal than
        /// </summary>
        public T StartValue { get; set; }
        /// <summary>
        /// Smaller than
        /// </summary>
        public T EndValue { get; set; }
        public Color LevelColor { get; set; }
    }
}
