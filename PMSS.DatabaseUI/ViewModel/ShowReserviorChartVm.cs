using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using System.ComponentModel;
using LiveCharts;
using LiveCharts.CoreComponents;
using PMSS.SqlDataOutput;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace PMSS.DatabaseUI.ViewModel
{
    public class ShowReserviorChartVm : INotifyPropertyChanged
    {
        public DelegateCommand SavePicCmd { get; set; }
        public DelegateCommand XunxianCmd { get; set; }
        public DelegateCommand XushuiliangCmd { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private List<ReservoirHydrologyRecord> listRecord;
        public List<ReservoirHydrologyRecord> ListRecord
        {
            get
            {
                return this.listRecord;
            }
            set
            {
                this.listRecord = value;
                ShowData(this.listRecord);
            }
        }

        LineSeries level;
        LineSeries warningLevel;
        LineSeries pondage;

        private bool xuxianEnable;
        public bool XuxianEnable
        {
            get
            {
                return this.xuxianEnable;
            }
            set
            {
                this.xuxianEnable = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(XuxianEnable)));
                }
            }
        }

        private bool xushuiliangEnable;
        public bool XushuiliangEnable
        {
            get
            {
                return this.xushuiliangEnable;
            }
            set
            {
                this.xushuiliangEnable = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(XushuiliangEnable)));
                }
            }
        }

        private string hint;
        public string Hint
        {
            get
            {
                return this.hint;
            }
            set
            {
                this.hint = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(Hint)));
                }
            }
        }

        private double minY;
        public double MinY
        {
            get
            {
                return this.minY;
            }
            set
            {
                this.minY = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(MinY)));
                }
            }
        }

        private void ModifyPosition(FrameworkElement fe)
        {
            /// get the size of the visual with margin
            Size fs = new Size(
                fe.ActualWidth +
                fe.Margin.Left + fe.Margin.Right,
                fe.ActualHeight +
                fe.Margin.Top + fe.Margin.Bottom);

            /// measure the visual with new size
            fe.Measure(fs);

            /// arrange the visual to align parent with (0,0)
            fe.Arrange(new Rect(
                -fe.Margin.Left, -fe.Margin.Top,
                fs.Width, fs.Height));
        }

        private void ModifyPositionBack(FrameworkElement fe)
        {
            /// remeasure a size smaller than need, wpf will
            /// rearrange it to the original position
            fe.Measure(new Size());
        }

        public void ShowData(List<ReservoirHydrologyRecord> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }

            this.TitleText = list[0].StationName + "站点水情曲线";

            level = new LineSeries
            {
                Title = "库水位：米",
                Values = new ChartValues<DatasOfHydro>(),
                ScalesYAt = 0,
                Stroke = Brushes.Blue,
                PointRadius = 0.2,
                Fill = Brushes.Transparent
            };

            warningLevel = new LineSeries
            {
                Title = "汛限水位：米",
                Values = new ChartValues<DatasOfHydro>(),
                Fill = Brushes.Transparent,
                ScalesYAt = 0,
                Stroke = Brushes.Red,
                PointRadius = 0.2
            };

            pondage = new LineSeries
            {
                Title = "蓄水量:亿立方米",
                Values = new ChartValues<DatasOfHydro>(),
                Fill = Brushes.Transparent,
                ScalesYAt = 1,
                Stroke = Brushes.Orange,
                PointRadius = 0.2
            };

            var levelList = new List<DatasOfHydro>();
            var warningLevelList = new List<DatasOfHydro>();
            var pondageList = new List<DatasOfHydro>();
            DatasOfHydro preLevel = null;
            DatasOfHydro preWarningLevel = null;
            DatasOfHydro prePondage = null;

            double minimumLevel = double.MaxValue;

            foreach (ReservoirHydrologyRecord record in list)
            {
                if ((preLevel == null) || (!preLevel.Time.Equals(record.Time)))
                {
                    preLevel = new DatasOfHydro { Data = record.Level, Time = record.Time };
                    levelList.Add(preLevel);
                    if (record.Level < minimumLevel)
                    {
                        minimumLevel = record.Level;
                    }
                }

                if ((record.WarningLevel > 0) && ((preWarningLevel == null) || (!preWarningLevel.Time.Equals(record.Time))))
                {
                    preWarningLevel = new DatasOfHydro { Data = record.WarningLevel, Time = record.Time };
                    warningLevelList.Add(preWarningLevel);
                    if(record.WarningLevel < minimumLevel)
                    {
                        minimumLevel = record.WarningLevel;
                    }
                }

                if ((record.Pondage > 0) && ((prePondage == null) || (!prePondage.Time.Equals(record.Time))))
                {
                    prePondage = new DatasOfHydro { Data = record.Pondage, Time = record.Time };
                    pondageList.Add(prePondage);
                }
            }

            this.MinY = (int)(minimumLevel * 0.9);

            level.Values = levelList.AsChartValues();
            if (warningLevelList.Count > 0)
            {
                warningLevel.Values = warningLevelList.AsChartValues();
            }

            if (pondageList.Count > 0)
            {
                pondage.Values = pondageList.AsChartValues();
            }

            HydroDatas = new SeriesCollection { level, pondage, warningLevel }.Setup(new SeriesConfiguration<DatasOfHydro>().Y(m => m.Data).X(m => m.Time.ToOADate()).Y(m => m.Data));

            ZFormatter = f => f.ToString();
            YFormatter = l => l.ToString();
            XFormatter = date => DateTime.FromOADate(date).ToString("MM.dd:HH");
            this.Hint = "图片生成完成！";
        }

        public ShowReserviorChartVm()
        {
            this.SavePicCmd = new DelegateCommand(SavePic);
            this.XunxianCmd = new DelegateCommand(ReSelect);
            this.XushuiliangCmd = new DelegateCommand(ReSelect);
            this.XuxianEnable = true;
            this.XushuiliangEnable = true;
        }

        public void ReSelect()
        {
            if (this.XuxianEnable == false && this.XushuiliangEnable == false)
            {
                HydroDatas = new SeriesCollection { level }.Setup(new SeriesConfiguration<DatasOfHydro>().Y(m => m.Data).X(m => m.Time.ToOADate()).Y(m => m.Data));
            }
            else if (this.XuxianEnable == true && this.XushuiliangEnable == false)
            {
                HydroDatas = new SeriesCollection { level, warningLevel }.Setup(new SeriesConfiguration<DatasOfHydro>().Y(m => m.Data).X(m => m.Time.ToOADate()).Y(m => m.Data));
            }
            else if (this.XuxianEnable == false && this.XushuiliangEnable == true)
            {
                HydroDatas = new SeriesCollection { level, pondage }.Setup(new SeriesConfiguration<DatasOfHydro>().Y(m => m.Data).X(m => m.Time.ToOADate()).Y(m => m.Data));
            }
            else
            {
                HydroDatas = new SeriesCollection { level, warningLevel, pondage }.Setup(new SeriesConfiguration<DatasOfHydro>().Y(m => m.Data).X(m => m.Time.ToOADate()).Y(m => m.Data));
            }
        }

        public void SavePic()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "图像文件 (*.png)|*.png";
            dlg.DefaultExt = "png";
            dlg.Title = "保存图片";
            dlg.ShowDialog();

            if (dlg.FileName != "")
            {
                FrameworkElement ui = this.Ct;
                System.IO.FileStream fs = new System.IO.FileStream(dlg.FileName, System.IO.FileMode.Create);
                RenderTargetBitmap bmp = new RenderTargetBitmap((int)ui.ActualWidth + 10, (int)ui.ActualHeight + 10, 96d, 96d, PixelFormats.Pbgra32);
                bmp.Render(ui);
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                encoder.Save(fs);
                fs.Close();
                this.Hint = "图片保存完成！";
            }
        }

        private string titleText;
        public string TitleText
        {
            get
            {
                return this.titleText;
            }
            set
            {
                this.titleText = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(TitleText)));
                }
            }
        }

        private SeriesCollection sC;
        public SeriesCollection SC
        {
            get
            {
                return this.sC;
            }
            set
            {
                this.sC = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(SC)));
                }
            }
        }

        private Chart rchart;
        public Chart Rchart
        {
            get
            {
                return this.rchart;
            }
            set
            {
                this.rchart = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(Rchart)));
                }
            }
        }

        private Grid ct;
        public Grid Ct
        {
            get
            {
                return this.ct;
            }
            set
            {
                this.ct = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(Ct)));
                }
            }
        }

        private SeriesCollection hydroDatas;
        public SeriesCollection HydroDatas
        {
            get
            {
                return this.hydroDatas;
            }
            set
            {
                this.hydroDatas = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(HydroDatas)));
                }
            }
        }

        public Func<double, string> ZFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }
        public Func<double, string> XFormatter { get; set; }

    }
}
