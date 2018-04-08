using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Layers;

namespace Pmss.Micaps.DataChecking
{
    public delegate void ShowLayerDelegate(string filePath);
    public class NavMetaData
    {
        public Layer CurrentLayer { get; set; }
        public string KeyName { get; set; }
        public int CurrentIndex { get; set; }
        public string FolderPath { get; set; }
        public List<string> AllFileNames { get; set; } = new List<string>();
        public ShowLayerDelegate ShowLayer { get; set; }

        public bool HasNextFile
        {
            get
            {
                if (CurrentIndex + 1 < AllFileNames.Count())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool HasLastFile
        {
            get
            {
                if (CurrentIndex - 1 >= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public void ShowNextFile()
        {
            if (!HasNextFile)
            {
                return;
            }
            if (this.ShowLayer == null)
            {
                throw new ArgumentNullException("ShowLayer");
            }

            CurrentIndex++;
            ShowLayer(this.FolderPath + '/' + AllFileNames[CurrentIndex]);
        }
        public void ShowLastFile()
        {
            if (!HasLastFile)
            {
                return;
            }
            if (this.ShowLayer == null)
            {
                throw new ArgumentNullException("ShowLayer");
            }

            CurrentIndex--;
            ShowLayer(this.FolderPath + '/' + AllFileNames[CurrentIndex]);
        }
        public void ShowCurrentFile()
        {
            ShowLayer(this.FolderPath + '/' + AllFileNames[CurrentIndex]);
        }
    }
}
