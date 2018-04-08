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
    /// WindowMapTrigger.xaml 的交互逻辑
    /// </summary>
    public partial class WindowMapTrigger : Window
    {
        public WindowMapTrigger(string stationId, string stationName, string stationType, string L, string Q, string WL, string time)
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.ShowInTaskbar = false;
            ViewModel.MapTriggerVm vm = new ViewModel.MapTriggerVm();
            vm.StationName = stationName;
            vm.StationType = stationType;
            vm.StationId = stationId;
            vm.L = L;
            vm.Q = Q;
            vm.WL = WL;
            vm.Time = time;
            this.DataContext = vm;
        }
    }
}
