using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Pmss.Micaps.Render.Config;
using Pmss.Micaps.Product;
using System.Collections.ObjectModel;

namespace PmssWpfApp.Dialogs
{
    /// <summary>
    /// Interaction logic for ProductChooseLevel.xaml
    /// </summary>
    public partial class ProductChooseLevel : Window
    {
        public ProductChooseLevel()
        {
            InitializeComponent();
            typeList = new ObservableCollection<ProductType>();
            AddItems();
            this.Owner = App.Current.MainWindow;
            this.ShowInTaskbar = false;
        }

        public void AddUnknownOption()
        {
            typeList.Add(new ProductType { Name = "其它", Value = ProductTypeEnum.Unknown });
            this.comboBox.SelectedValue = ProductTypeEnum.Unknown;
        }
        public ProductTypeEnum Type
        {
            get
            {
                return (ProductTypeEnum)this.comboBox.SelectedValue;
            }
        }
        public string LevelSetting
        {
            get
            {
                if (this.Type == ProductTypeEnum.Flood || this.Type == ProductTypeEnum.River)
                {
                    return LevelValueManager.LevelNameFlood;
                }
                else
                {
                    return LevelValueManager.LevelNameDisaster;
                }
            }
        }

        private void AddItems()
        {
            typeList.Add(new ProductType { Name = "山洪", Value = ProductTypeEnum.Flood });
            typeList.Add(new ProductType { Name = "中小河流", Value = ProductTypeEnum.River });
            typeList.Add(new ProductType { Name = "地质灾害", Value = ProductTypeEnum.Disaster });
            typeList.Add(new ProductType { Name = "渍涝", Value = ProductTypeEnum.Zilao });

            this.comboBox.ItemsSource = typeList;
            this.comboBox.DisplayMemberPath = "Name";
            this.comboBox.SelectedValuePath = "Value";
            this.comboBox.SelectedValue = ProductTypeEnum.Flood;
        }

        private ObservableCollection<ProductType> typeList;
    }

    public class ProductType
    {
        public string Name { get; set; }
        public ProductTypeEnum Value { get; set; }
    }
}
