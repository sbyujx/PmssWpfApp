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
using Pmss.Micaps.Product;
using System.Collections.ObjectModel;

namespace PmssWpfApp.Dialogs
{
    /// <summary>
    /// Interaction logic for ProductChooseSaveOption.xaml
    /// </summary>
    public partial class ProductChooseSaveOption : Window
    {
        public ProductChooseSaveOption()
        {
            InitializeComponent();
            this.Owner = App.Current.MainWindow;
            this.ShowInTaskbar = false;

            this.textBoxStartDate.Text = DateTime.Now.ToString("M月d日HH时");
            this.textBoxEndDate.Text = DateTime.Now.AddDays(1).ToString("M月d日HH时");

            this.comboBox.Visibility = Visibility.Hidden;
            this.AddTemplateOptions();
        }
        public ProductChooseSaveOption(ProductSaveOption option) : this()
        {
            //InitializeComponent();
            //this.Owner = App.Current.MainWindow;
            //this.ShowInTaskbar = false;

            this.saveOption = option;
            titleDict.Add(ProductTypeEnum.Flood, "山洪灾害气象风险预警图");
            titleDict.Add(ProductTypeEnum.River, "中小河流洪水气象风险预警图");
            titleDict.Add(ProductTypeEnum.Disaster, "地质灾害气象风险预警图");
            titleDict.Add(ProductTypeEnum.Zilao, "渍涝风险气象预报图");
            titleDict.Add(ProductTypeEnum.Unknown, string.Empty);

            string title = titleDict[option.ProductType];
            if (option.ProductRegion == ProductRegionEnum.Country)
            {
                title = "全国" + title;
                this.comboBox.Visibility = Visibility.Visible;
            }
            this.textBoxTitle.Text = title;

            this.textBoxPublishDate.Text = DateTime.Now.ToString("M月d日HH时制作");
            if (option.ProductRegion == ProductRegionEnum.Country)
            {
                this.textBoxPublishDate.Text = DateTime.Now.ToString("M月d日HH时发布");
            }
        }

        public string ProductTitle
        {
            get
            {
                return this.textBoxTitle.Text;
            }
        }
        public string ProductStartDate
        {
            get
            {
                return this.textBoxStartDate.Text;
            }
        }
        public string ProductEndDate
        {
            get
            {
                return this.textBoxEndDate.Text;
            }
        }
        public string ProductPublishDate
        {
            get
            {
                return this.textBoxPublishDate.Text;
            }
        }
        public int TemplateType
        {
            get
            {
                if (this.comboBox.SelectedValue.ToString() == "2")
                    return 20;
                else if (this.comboBox.SelectedValue.ToString() == "1")
                    return 8;
                else
                    return 0;
            }
        }

        private void confirmBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void AddTemplateOptions()
        {
            options = new ObservableCollection<KeyValuePair<string, string>>();
            options.Add(new KeyValuePair<string, string>("默认模板", "0"));
            options.Add(new KeyValuePair<string, string>("全国08时模板", "1"));
            options.Add(new KeyValuePair<string, string>("全国20时模板", "2"));

            this.comboBox.ItemsSource = options;
            this.comboBox.DisplayMemberPath = "Key";
            this.comboBox.SelectedValuePath = "Value";
            this.comboBox.SelectedValue = "0";
            this.comboBox.SelectionChanged += ComboBox_SelectionChanged;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.saveOption.ProductType == ProductTypeEnum.Flood)
            {
                this.textBoxTitle.Text = "全国山洪灾害气象预警图";
            }

            if (this.comboBox.SelectedValue.ToString() == "0")
            {
                this.textBoxStartDate.Text = DateTime.Now.ToString("M月d日HH时");
                this.textBoxEndDate.Text = DateTime.Now.AddDays(1).ToString("M月d日HH时");
                this.textBoxPublishDate.Text = DateTime.Now.ToString("M月d日HH时发布");
            }

            if (this.comboBox.SelectedValue.ToString() == "1")
            {
                this.textBoxStartDate.Text = DateTime.Now.ToString("M月d日08时");
                this.textBoxEndDate.Text = DateTime.Now.AddDays(1).ToString("M月d日08时");
                this.textBoxPublishDate.Text = DateTime.Now.ToString("M月d日07时发布");
            }

            if (this.comboBox.SelectedValue.ToString() == "2")
            {
                this.textBoxStartDate.Text = DateTime.Now.ToString("M月d日20时");
                this.textBoxEndDate.Text = DateTime.Now.AddDays(1).ToString("M月d日20时");
                this.textBoxPublishDate.Text = DateTime.Now.ToString("M月d日18时");

                string tmp = "发布";
                if (this.saveOption.ProductType == ProductTypeEnum.Disaster || this.saveOption.ProductType == ProductTypeEnum.Flood)
                {
                    tmp = "联合发布";
                }
                this.textBoxPublishDate.Text += tmp;

                if (this.saveOption.ProductType == ProductTypeEnum.Flood)
                {
                    this.textBoxTitle.Text = "全国山洪灾害气象预警图";
                }
            }
        }

        private ProductSaveOption saveOption;
        private Dictionary<ProductTypeEnum, string> titleDict = new Dictionary<ProductTypeEnum, string>();
        private ObservableCollection<KeyValuePair<string, string>> options;
    }
}
