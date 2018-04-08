using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Symbology;

namespace PmssWpfApp.ViewModel
{
    public class DrawOptionVm : INotifyPropertyChanged
    {
        public DrawOptionVm(DrawShape shape,Symbol symbol)
        {
            Shape = shape;
            this.Symbol = symbol;
        }

        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsChecked)));
                }
            }
        }
        public DrawShape Shape { get; set; }
        public Symbol Symbol { get; set; }
        /// <summary>
        /// Distinguish line and contour
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        private bool isChecked = false;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
