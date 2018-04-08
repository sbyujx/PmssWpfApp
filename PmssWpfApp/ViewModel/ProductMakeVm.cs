using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using PMSS.WordProductUI;
using PMSS.ConfigureSet;
using System.Windows;
using PMSS.Log;

namespace PmssWpfApp.ViewModel
{
    public class ProductMakeVm
    {
        public DelegateCommand OutputPathCmd { get; set; }
        public DelegateCommand TorrentWordCmd { get; set; }
        public DelegateCommand PreSignConfigCmd { get; set; }
        public DelegateCommand WaterLoggingWordCmd { get; set; }
        public DelegateCommand GeoDisWordCmd { get; set; }
        public DelegateCommand HydroMoniterWordCmd { get; set; }

        public ProductMakeVm()
        {
            this.TorrentWordCmd = new DelegateCommand(TorrentWord);
            this.OutputPathCmd = new DelegateCommand(OutputPath);
            this.PreSignConfigCmd = new DelegateCommand(PreSignConfig);
            this.WaterLoggingWordCmd = new DelegateCommand(WaterLoggingWord);
            this.GeoDisWordCmd = new DelegateCommand(GeoDisWord);
            this.HydroMoniterWordCmd = new DelegateCommand(HydroMoniterWord);
        }

        public void HydroMoniterWord()
        {
            try
            {
                WindowHydroMonitorWord wd = new WindowHydroMonitorWord();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void GeoDisWord()
        {
            try
            {
                WindowGeoDisWord wd = new WindowGeoDisWord();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void WaterLoggingWord()
        {
            try
            {
                WindowWaterloggingWord wd = new WindowWaterloggingWord();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void PreSignConfig()
        {
            try
            {
                WindowPreSign wd = new WindowPreSign();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void TorrentWord()
        {
            try
            {
                WindowTorrentWord wd = new WindowTorrentWord();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void OutputPath()
        {
            try
            {
                WindowProductPathConfig wd = new WindowProductPathConfig();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
