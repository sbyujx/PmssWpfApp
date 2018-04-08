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
    /// Interaction logic for LineValueInput.xaml
    /// </summary>
    public partial class LineValueInput : Window
    {
        public LineValueInput()
        {
            InitializeComponent();
            this.Owner = App.Current.MainWindow;
            this.ShowInTaskbar = false;
        }

        public int? LineValue
        {
            get
            {
                int? result = null;
                if (!string.IsNullOrWhiteSpace(this.textBox.Text))
                {
                    try
                    {
                        result = Convert.ToInt32(this.textBox.Text);
                    }
                    catch
                    {

                    }
                }
                return result;
            }
            set
            {
                this.textBox.Text = value.ToString();
            }
        }

        //public void SetValue(int value)
        //{
        //    this.textBox.Text = value.ToString();
        //}

        //public int? GetValue()
        //{
        //    int? result = null;
        //    if (this.textBox.Text != null)
        //        result = Convert.ToInt32(this.textBox.Text);
        //    return result;
        //}

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
