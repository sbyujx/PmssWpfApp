using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Layers;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PmssWpfApp.ViewModel
{
    public class LayersTreeViewItem : INotifyPropertyChanged
    {
        public LayersTreeViewItem(Layer layer)
        {
            this.layer = layer;
        }
        public void AddChild(LayersTreeViewItem item)
        {
            if (this.ChildItems == null)
            {
                this.ChildItems = new ObservableCollection<LayersTreeViewItem>();
            }
            ChildItems.Add(item);
        }
        public void RemoveChild(LayersTreeViewItem item)
        {
            // Fix memory leak. http://www.ranorex.com/forum/memory-leak-in-target-silverlight-app-via-automatonpeer-objs-t6963.html
            // The item will be kept in memory up to 3 minutes after being reomved.
            item.RelatedLayer = null;
            ChildItems.Remove(item);
        }

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                this.isSelected = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }
        public bool IsVisible
        {
            get
            {
                return this.layer.IsVisible;
            }
            set
            {
                this.layer.IsVisible = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsVisible)));
                }
            }
        }
        public string DisplayName
        {
            get
            {
                return this.layer.DisplayName;
            }
        }
        public Layer RelatedLayer
        {
            get
            {
                return this.layer;
            }
            set
            {
                this.layer = value;
            }
        }
        public ObservableCollection<LayersTreeViewItem> ChildItems
        {
            get
            {
                return this.childItems;
            }
            set
            {
                this.childItems = value;
                if(PropertyChanged!=null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(ChildItems)));
                }
            }
        }
        //public List<LayersTreeViewItem> ChildItems { get; set; } = new List<LayersTreeViewItem>();

        private Layer layer;
        private bool isSelected = false;
        private ObservableCollection<LayersTreeViewItem> childItems;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
