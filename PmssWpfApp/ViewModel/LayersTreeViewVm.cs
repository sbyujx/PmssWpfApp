using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Controls;
using System.ComponentModel;

namespace PmssWpfApp.ViewModel
{
    public class LayersTreeViewVm : INotifyPropertyChanged
    {
        public LayersTreeViewVm(GroupLayer baseGroupLayer0, GroupLayer baseGroupLayer, GroupLayer readonlyGroupLayer, GroupLayer editableGroupLayer, GroupLayer productGroupLayer)
        {
            this.BaseLayer0Item = new LayersTreeViewItem(baseGroupLayer0);
            this.BaseLayerItem = new LayersTreeViewItem(baseGroupLayer);
            this.ReadOnlyLayerItem = new LayersTreeViewItem(readonlyGroupLayer);
            this.EditableLayerItem = new LayersTreeViewItem(editableGroupLayer);
            this.ProductLayerItem = new LayersTreeViewItem(productGroupLayer);

            Dict.Add(baseGroupLayer0, this.BaseLayer0Item);
            Dict.Add(baseGroupLayer, this.BaseLayerItem);
            Dict.Add(readonlyGroupLayer, this.ReadOnlyLayerItem);
            Dict.Add(editableGroupLayer, this.EditableLayerItem);
            Dict.Add(productGroupLayer, this.ProductLayerItem);

            Items.Add(this.BaseLayer0Item);
            Items.Add(this.BaseLayerItem);
            Items.Add(this.ReadOnlyLayerItem);
            Items.Add(this.EditableLayerItem);
            Items.Add(this.ProductLayerItem);
        }

        public ObservableCollection<LayersTreeViewItem> Items { get; set; } = new ObservableCollection<LayersTreeViewItem>();
        public Dictionary<Layer, LayersTreeViewItem> Dict { get; set; } = new Dictionary<Layer, LayersTreeViewItem>();

        public void AddBaseLayer0(Layer layer)
        {
            AddLayer(this.BaseLayer0Item, layer);
        }
        public void AddBaseLayer(Layer layer)
        {
            AddLayer(this.BaseLayerItem, layer);
        }
        public void AddReadonlyLayer(Layer layer)
        {
            AddLayer(this.ReadOnlyLayerItem, layer);
        }
        public void AddEditableLayer(Layer layer)
        {
            AddLayer(this.EditableLayerItem, layer);
        }
        public void AddProductLayer(Layer layer)
        {
            AddLayer(this.ProductLayerItem, layer);
        }

        public void RemoveBaseLayer0(Layer layer)
        {
            removeLayer(this.BaseLayer0Item, layer);
        }
        public void RemoveBaseLayer(Layer layer)
        {
            removeLayer(this.BaseLayerItem, layer);
        }
        public void RemoveReadonlyLayer(Layer layer)
        {
            removeLayer(this.ReadOnlyLayerItem, layer);
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
        }
        public void RemoveEditableLayer(Layer layer)
        {
            removeLayer(this.EditableLayerItem, layer);
        }
        public void RemoveProductLayer(Layer layer)
        {
            removeLayer(this.ProductLayerItem, layer);
        }

        public void RemoveFirstBaseLayer()
        {
            var item = this.BaseLayerItem.ChildItems.First();
            var layer = item.RelatedLayer;

            Dict.Remove(layer);
            this.BaseLayerItem.ChildItems.Remove(item);
            (this.BaseLayerItem.RelatedLayer as GroupLayer).ChildLayers.Remove(layer);
        }
        public void AddFirstBaseLayer(Layer layer)
        {
            var item = new LayersTreeViewItem(layer);
            Dict.Add(layer, item);

            this.BaseLayerItem.ChildItems.Insert(0, item);
            (this.BaseLayerItem.RelatedLayer as GroupLayer).ChildLayers.Insert(0, layer);
        }

        public void RemoveFirstBaseLayer0()
        {
            var item = this.BaseLayer0Item.ChildItems.First();
            var layer = item.RelatedLayer;

            Dict.Remove(layer);
            this.BaseLayer0Item.ChildItems.Remove(item);
            (this.BaseLayer0Item.RelatedLayer as GroupLayer).ChildLayers.Remove(layer);
        }
        public void AddFirstBaseLayer0(Layer layer)
        {
            var item = new LayersTreeViewItem(layer);
            Dict.Add(layer, item);

            this.BaseLayer0Item.ChildItems.Insert(0, item);
            (this.BaseLayer0Item.RelatedLayer as GroupLayer).ChildLayers.Insert(0, layer);
        }

        public void SetVisibility(Layer layer, bool visible)
        {
            Dict[layer].IsVisible = visible;
        }
        public void SelectLayer(Layer layer)
        {
            Dict[layer].IsSelected = true;
        }

        private void AddLayer(LayersTreeViewItem groupItem, Layer layer)
        {
            var item = new LayersTreeViewItem(layer);
            Dict.Add(layer, item);

            groupItem.AddChild(item);
            (groupItem.RelatedLayer as GroupLayer).ChildLayers.Add(layer);
        }
        private void removeLayer(LayersTreeViewItem groupItem, Layer layer)
        {
            var item = Dict[layer];
            Dict.Remove(layer);
            //refer = new WeakReference(item);

            groupItem.RemoveChild(item);
            (groupItem.RelatedLayer as GroupLayer).ChildLayers.Remove(layer);
        }

        //private WeakReference refer = null;

        private LayersTreeViewItem BaseLayer0Item; // 底图
        private LayersTreeViewItem ProductLayerItem;
        private LayersTreeViewItem BaseLayerItem; // 基础地理数据
        private LayersTreeViewItem ReadOnlyLayerItem;
        private LayersTreeViewItem EditableLayerItem;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
