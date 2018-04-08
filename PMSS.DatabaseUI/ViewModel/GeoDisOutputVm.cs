using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using System.ComponentModel;
using PMSS.SqlDataOutput;
using Microsoft.Win32;
using System.Threading;
using System.Windows.Threading;
using System.Windows;

namespace PMSS.DatabaseUI.ViewModel
{
    public class GeoDisOutputVm : INotifyPropertyChanged
    {
        public DelegateCommand SaveCmd { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly string type = "地灾";

        public GeoDisOutputVm()
        {
            this.TimeFrom = new DateTime(DateTime.Now.Year, 1, 1);
            this.TimeTo = DateTime.Now;
            this.SaveCmd = new DelegateCommand(Save);
        }

        private string area;
        public string Area
        {
            get
            {
                return this.area;
            }
            set
            {
                this.area = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(Area)));
                }
            }
        }

        private DateTime timeFrom;
        public DateTime TimeFrom
        {
            get
            {
                return this.timeFrom;
            }
            set
            {
                this.timeFrom = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(TimeFrom)));
                }
            }
        }

        private DateTime timeTo;
        public DateTime TimeTo
        {
            get
            {
                return this.timeTo;
            }
            set
            {
                this.timeTo = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(TimeTo)));
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

        public void Save()
        {
            if (this.area == null || this.area.Trim().Equals(string.Empty))
            {
                this.Hint = "导出区域不能为空!";
                return;
            }

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "EXCEL文件 (*.xlsx)|*.xlsx";
            dlg.DefaultExt = "xlsx";
            dlg.Title = "导出地灾数据";
            dlg.ShowDialog();

            OutputDisasterOption option = new OutputDisasterOption(this.type);
            option.IsSetTimeRange = true;
            option.From = this.timeFrom;
            option.To = this.timeTo;
            option.Area = this.area;

            if (dlg.FileName != "")
            {
                switch (dlg.FilterIndex)
                {
                    case 1: //EXECL文件
                        option.FileType = OutputDisasterOption.FILE_TYPE_EXCEL;
                        break;
                }
                Thread thread = new Thread(unused => SaveFile(dlg.FileName, option));
                thread.Start();
            }
            else
            {
                this.Hint = "请选择一个文件名!";
            }
        }

        private void SaveFile(object fileName, object op)
        {
            SaveFile((string)fileName, (OutputDisasterOption)op);
        }

        private void SaveFile(string fileName, OutputDisasterOption op)
        {
            this.Hint = "导出中...";

            FetchDisasterRecord fdr = new FetchDisasterRecord();
            fdr.GetDisaterRecord(op.Type, op.Area, op.From, op.To);
            if (op.FileType == OutputDisasterOption.FILE_TYPE_EXCEL)
            {
                fdr.SaveToExcelFile(fileName);
            }

            this.Hint = "导出完成!";
        }
    }
}
