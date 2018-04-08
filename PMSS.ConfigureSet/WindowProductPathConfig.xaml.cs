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

namespace PMSS.ConfigureSet
{
    /// <summary>
    /// WindowProductPathConfig.xaml 的交互逻辑
    /// </summary>
    public partial class WindowProductPathConfig : Window
    {
        public WindowProductPathConfig()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.ShowInTaskbar = false;
            ViewModel.ProductPathConfigVm vm = new ViewModel.ProductPathConfigVm();
            this.DataContext = vm;
            vm.wd = wd;
        }
    }
}
