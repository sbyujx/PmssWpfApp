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

namespace PmssWpfApp.Dialogs
{
    /// <summary>
    /// Interaction logic for MonitorHydrologicHistory.xaml
    /// </summary>
    public partial class MonitorHydrologicHistory : Window
    {
        public MonitorHydrologicHistory()
        {
            InitializeComponent();
            this.startTime.Value = DateTime.Now.AddDays(-1);
            this.endTime.Value = DateTime.Now;
            this.Owner = App.Current.MainWindow;
            this.ShowInTaskbar = false;
        }

        public DateTime StartTime
        {
            get
            {
                if(this.startTime.Value==null)
                {
                    return DateTime.Now.AddDays(-1);
                }
                else
                {
                    return this.startTime.Value.Value;
                }
            }
        }
        public DateTime EndTime
        {
            get
            {
                if (this.endTime.Value == null)
                {
                    return DateTime.Now;
                }
                else
                {
                    return this.endTime.Value.Value;
                }
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
