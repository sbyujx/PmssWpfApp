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
using System.Windows.Controls.Ribbon;

namespace PmssWpfApp
{
    /// <summary>
    /// Interaction logic for RibbonMainWindow.xaml
    /// </summary>
    public partial class RibbonMainWindow : RibbonWindow
    {
        public RibbonMainWindow()
        {
            Esri.ArcGISRuntime.ArcGISRuntimeEnvironment.ClientId = "VhWZbPLHQ39H3cmu";
            try
            {
                Esri.ArcGISRuntime.ArcGISRuntimeEnvironment.Initialize();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Unable to initialize the ArcGIS Runtime with the client ID provided: " + ex.Message);
            }
            InitializeComponent();
        }
    }
}
