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

namespace PMSS.DatabaseUI.ViewModel
{
    public class ShowRiverChartVm : INotifyPropertyChanged
    {
        public DelegateCommand SavePicCmd { get; set; }
        public DelegateCommand JJSWCmd { get; set; }
        public DelegateCommand BZSWCmd { get; set; }
        public DelegateCommand LLCmd { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private List<RiverHydrologyRecord> listRecord;
        public List<RiverHydrologyRecord> ListRecord
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

        private bool jJSWEnable;
        public bool JJSWEnable
        {
            get
            {
                return this.jJSWEnable;
            }
            set
            {
                this.jJSWEnable = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(JJSWEnable)));
                }
            }
        }

        private bool bZSWEnable;
        public bool BZSWEnable
        {
            get
            {
                return this.bZSWEnable;
            }
            set
            {
                this.bZSWEnable = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(BZSWEnable)));
                }
            }
        }

        private bool lLEnable;
        public bool LLEnable
        {
            get
            {
                return this.lLEnable;
            }
            set
            {
                this.lLEnable = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(LLEnable)));
                }
            }
        }

        LineSeries level;
        LineSeries warningLevel;
        LineSeries flow;
        LineSeries safetyLevel;

        public void ShowData(List<RiverHydrologyRecord> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }

            this.TitleText = list[0].StationName + "站点水情曲线";

            level = new LineSeries
            {
                Title = "水位:米",
                Values = new ChartValues<DatasOfHydro>(),
                ScalesYAt = 0,
                Stroke = Brushes.Blue,
                Fill = Brushes.Transparent,
                PointRadius = 0.2
            };

            warningLevel = new LineSeries
            {
                Title = "警戒水位:米",
                Values = new ChartValues<DatasOfHydro>(),
                Fill = Brushes.Transparent,
                ScalesYAt = 0,
                Stroke = Brushes.Red,
                PointRadius = 0.2
            };

            safetyLevel = new LineSeries
            {
                Title = "保证水位:米",
                Values = new ChartValues<DatasOfHydro>(),
                Fill = Brushes.Transparent,
                ScalesYAt = 0,
                Stroke = Brushes.ForestGreen,
                PointRadius = 0.2
            };

            flow = new LineSeries
            {
                Title = "流量:立方米/秒",
                Values = new ChartValues<DatasOfHydro>(),
                Fill = Brushes.Transparent,
                ScalesYAt = 1,
                Stroke = Brushes.Orange,
                PointRadius = 0.2
            };

            var levelList = new List<DatasOfHydro>();
            var warningLevelList = new List<DatasOfHydro>();
            var flowList = new List<DatasOfHydro>();
            var safetyLevelList = new List<DatasOfHydro>();
            DatasOfHydro preLevel = null;
            DatasOfHydro preWarningLevel = null;
            DatasOfHydro preFlow = null;
            DatasOfHydro preSafetyLevel = null;

            double minimumLevel = double.MaxValue;

            foreach (RiverHydrologyRecord record in list)
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
                    if (record.WarningLevel < minimumLevel)
                    {
                        minimumLevel = record.WarningLevel;
                    }
                }

                if ((record.SafetyLevel > 0) && ((preSafetyLevel == null) || (!preSafetyLevel.Time.Equals(record.Time))))
                {
                    preSafetyLevel = new DatasOfHydro { Data = record.SafetyLevel, Time = record.Time };
                    safetyLevelList.Add(preSafetyLevel);
                    if (record.SafetyLevel < minimumLevel)
                    {
                        minimumLevel = record.SafetyLevel;
                    }
                }

                if ((record.Flow > 0) && ((preFlow == null) || (!preFlow.Time.Equals(record.Time))))
                {
                    preFlow = new DatasOfHydro { Data = record.Flow, Time = record.Time };
                    flowList.Add(preFlow);
                }
            }

            this.MinY = (int)(minimumLevel * 0.9);

            level.Values = levelList.AsChartValues();
            if (warningLevelList.Count > 0)
            {
                warningLevel.Values = warningLevelList.AsChartValues();
            }

            if(flowList.Count > 0)
            {
                flow.Values = flowList.AsChartValues();
            }

            if(safetyLevelList.Count > 0)
            {
                safetyLevel.Values = safetyLevelList.AsChartValues();
            }

            HydroDatas = new SeriesCollection { level, flow, warningLevel, safetyLevel }.Setup(new SeriesConfiguration<DatasOfHydro>().Y(m => m.Data).X(m => m.Time.ToOADate()).Y(m => m.Data));

            ZFormatter = f => f.ToString();
            YFormatter = l => l.ToString();
            XFormatter = date => DateTime.FromOADate(date).ToString("MM.dd:HH");

            this.Hint = "图片生成完成！";
        }

        public ShowRiverChartVm()
        {
            this.SavePicCmd = new DelegateCommand(SavePic);
            this.JJSWCmd = new DelegateCommand(ReSelect);
            this.BZSWCmd = new DelegateCommand(ReSelect);
            this.LLCmd = new DelegateCommand(ReSelect);
            this.JJSWEnable = true;
            this.BZSWEnable = true;
            this.LLEnable = true;
        }

        public void ReSelect()
        {
            if (this.JJSWEnable == false && this.BZSWEnable == false && this.LLEnable == false)
            {
                HydroDatas = new SeriesCollection { level }.Setup(new SeriesConfiguration<DatasOfHydro>().Y(m => m.Data).X(m => m.Time.ToOADate()).Y(m => m.Data));
            }
            else if (this.JJSWEnable == true && this.BZSWEnable == false && this.LLEnable == false)
            {
                HydroDatas = new SeriesCollection { level, warningLevel}.Setup(new SeriesConfiguration<DatasOfHydro>().Y(m => m.Data).X(m => m.Time.ToOADate()).Y(m => m.Data));
            }
            else if (this.JJSWEnable == false && this.BZSWEnable == true && this.LLEnable == false)
            {
                HydroDatas = new SeriesCollection { level, safetyLevel }.Setup(new SeriesConfiguration<DatasOfHydro>().Y(m => m.Data).X(m => m.Time.ToOADate()).Y(m => m.Data));
            }
            else if (this.JJSWEnable == false && this.BZSWEnable == false && this.LLEnable == true)
            {
                HydroDatas = new SeriesCollection { level, flow}.Setup(new SeriesConfiguration<DatasOfHydro>().Y(m => m.Data).X(m => m.Time.ToOADate()).Y(m => m.Data));
            }
            else if (this.JJSWEnable == true && this.BZSWEnable == true && this.LLEnable == false)
            {
                HydroDatas = new SeriesCollection { level, warningLevel, safetyLevel }.Setup(new SeriesConfiguration<DatasOfHydro>().Y(m => m.Data).X(m => m.Time.ToOADate()).Y(m => m.Data));
            }
            else if (this.JJSWEnable == true && this.BZSWEnable == false && this.LLEnable == true)
            {
                HydroDatas = new SeriesCollection { level, flow, warningLevel }.Setup(new SeriesConfiguration<DatasOfHydro>().Y(m => m.Data).X(m => m.Time.ToOADate()).Y(m => m.Data));
            }
            else if (this.JJSWEnable == false && this.BZSWEnable == true && this.LLEnable == true)
            {
                HydroDatas = new SeriesCollection { level, flow, safetyLevel }.Setup(new SeriesConfiguration<DatasOfHydro>().Y(m => m.Data).X(m => m.Time.ToOADate()).Y(m => m.Data));
            }
            else
            {
                HydroDatas = new SeriesCollection { level, flow, warningLevel, safetyLevel }.Setup(new SeriesConfiguration<DatasOfHydro>().Y(m => m.Data).X(m => m.Time.ToOADate()).Y(m => m.Data));
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
                ModifyPosition(ui);
                RenderTargetBitmap bmp = new RenderTargetBitmap((int)ui.ActualWidth + 10, (int)ui.ActualHeight + 10, 96d, 96d, PixelFormats.Pbgra32);
                bmp.Render(ui);
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                encoder.Save(fs);
                fs.Close();
                ModifyPositionBack(ui);
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

        public Func<double, string> YFormatter { get; set; }
        public Func<double, string> XFormatter { get; set; }
        public Func<double, string> ZFormatter { get; set; }

    }
}
