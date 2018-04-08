﻿using System;
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
    /// WindowNotesConfig.xaml 的交互逻辑
    /// </summary>
    public partial class WindowNotesConfig : Window
    {
        public WindowNotesConfig()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.ShowInTaskbar = false;
            this.DataContext = new ViewModel.NotesConfigVm();
        }
    }
}
