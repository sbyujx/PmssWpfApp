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
    /// Interaction logic for MonitorSetting.xaml
    /// </summary>
    public partial class MonitorSetting : Window
    {
        public MonitorSetting()
        {
            InitializeComponent();
            this.textBox1.Text = Properties.Settings.Default.MonitorRain01Value.ToString();
            this.textBox3.Text = Properties.Settings.Default.MonitorRain03Value.ToString();
            this.textBox6.Text = Properties.Settings.Default.MonitorRain06Value.ToString();
            this.textBox24.Text = Properties.Settings.Default.MonitorRain24Value.ToString();
            this.Owner = App.Current.MainWindow;
            this.ShowInTaskbar = false;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            int value;
            if (int.TryParse(this.textBox1.Text, out value))
            {
                Properties.Settings.Default.MonitorRain01Value = value;
            }
            if (int.TryParse(this.textBox3.Text, out value))
            {
                Properties.Settings.Default.MonitorRain03Value = value;
            }
            if (int.TryParse(this.textBox6.Text, out value))
            {
                Properties.Settings.Default.MonitorRain06Value = value;
            }
            if (int.TryParse(this.textBox24.Text, out value))
            {
                Properties.Settings.Default.MonitorRain24Value = value;
            }
            Properties.Settings.Default.Save();
            this.DialogResult = true;
        }
    }
}
