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
using PMSS.SqlDataOutput;

namespace PMSS.DatabaseUI
{
    /// <summary>
    /// WindowShowRiverChart.xaml 的交互逻辑
    /// </summary>
    public partial class WindowShowRiverChart : Window
    {
        public WindowShowRiverChart(List<RiverHydrologyRecord> list)
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.ShowInTaskbar = false;
            ViewModel.ShowRiverChartVm vm = new ViewModel.ShowRiverChartVm();
            vm.ListRecord = list;
            this.DataContext = vm;
            vm.Ct = ChartPic;
        }
    }
}
