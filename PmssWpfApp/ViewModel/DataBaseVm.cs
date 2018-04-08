using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using PMSS.DatabaseUI;
using System.Windows;

namespace PmssWpfApp.ViewModel
{
    public class DataBaseVm
    {
        public DelegateCommand GeoDisInputCmd { get; set; }
        public DelegateCommand TorrentInputCmd { get; set; }
        public DelegateCommand WaterLoggingInputCmd { get; set; }
        public DelegateCommand RiverOutputCmd { get; set; }
        public DelegateCommand ReservoirOutputCmd { get; set; }
        public DelegateCommand GeoDisOutputCmd { get; set; }
        public DelegateCommand TorrentOutputCmd { get; set; }
        public DelegateCommand WaterLoggingOutputCmd { get; set; }

        public DataBaseVm()
        {
            this.GeoDisInputCmd = new DelegateCommand(GeoDisInput);
            this.TorrentInputCmd = new DelegateCommand(TorrentInput);
            this.WaterLoggingInputCmd = new DelegateCommand(WaterLoggingInput);
            this.RiverOutputCmd = new DelegateCommand(RiverOutput);
            this.ReservoirOutputCmd = new DelegateCommand(ReservoirOutput);
            this.GeoDisOutputCmd = new DelegateCommand(GeoDisOutput);
            this.TorrentOutputCmd = new DelegateCommand(TorrentOutput);
            this.WaterLoggingOutputCmd = new DelegateCommand(WaterLoggingOutput);
        }

        public void GeoDisInput()
        {
            try
            {
                WindowGeoDisInput wd = new WindowGeoDisInput();
                wd.ShowDialog();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void TorrentInput()
        {
            try
            {
                WindowTorrentInput wd = new WindowTorrentInput();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void WaterLoggingInput()
        {
            try
            {
                WindowWaterLoggingInput wd = new WindowWaterLoggingInput();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void RiverOutput()
        {
            try
            {
                WindowRiverOutput wd = new WindowRiverOutput();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ReservoirOutput()
        {
            try
            {
                WindowReserviorOutput wd = new WindowReserviorOutput();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void GeoDisOutput()
        {
            try
            {
                WindowGeoDisOutput wd = new WindowGeoDisOutput();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void TorrentOutput()
        {
            try
            {
                WindowToorrentOutput wd = new WindowToorrentOutput();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void WaterLoggingOutput()
        {
            try
            {
                WindowWaterLoggingOutput wd = new WindowWaterLoggingOutput();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
