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

namespace PMSS.DatabaseUI
{
    /// <summary>
    /// WindowRiverOutput.xaml 的交互逻辑
    /// </summary>
    public partial class WindowRiverOutput : Window
    {
        public WindowRiverOutput()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.ShowInTaskbar = false;
            ViewModel.RiverOutputVm vm = new ViewModel.RiverOutputVm();
            this.DataContext = vm;
            vm.RiverDataGrid = RiverGrid;
        }
    }
}
