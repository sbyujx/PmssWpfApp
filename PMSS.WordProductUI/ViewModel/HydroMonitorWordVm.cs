using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using System.ComponentModel;
using PMSS.WordProduct;
using PMSS.Configure;
using System.Collections.ObjectModel;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Windows;


namespace PMSS.WordProductUI.ViewModel
{
    public class HydroMonitorWordVm : INotifyPropertyChanged
    {
        public Window wd;
        public DelegateCommand OpenDirCmd { get; set; }
        public DelegateCommand WordMakeCmd { get; set; }
        public DelegateCommand ChangeCmd { get; set; }

        private ProductPathConfig config;
        private string templateName;
        private readonly string templateFileName = "水情监测报告模板.docx";
        private readonly string dir = "WordTemplate";
        private readonly BackgroundWorker worker = new BackgroundWorker();

        public event PropertyChangedEventHandler PropertyChanged;

        public HydroMonitorWordVm()
        {
            this.OpenDirCmd = new DelegateCommand(OpenDir);
            this.WordMakeCmd = new DelegateCommand(WordMake);
            this.ChangeCmd = new DelegateCommand(Change);

            this.MakingTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0);

            config = new ProductPathConfig();
            this.DirList = new ObservableCollection<ProductPath>(config.ListProductPath);

            PreAndSignConfig cf = new PreAndSignConfig();
            this.MakingPerson = cf.Pas.PreName;

            this.templateName = Path.Combine(Directory.GetCurrentDirectory(), dir, templateFileName);
            FileInfo fileInfo = new FileInfo(this.templateName);
            if (!fileInfo.Exists)
            {
                this.templateName = null;
            }

            if (!File.Exists(this.templateName))
            {
                this.templateName = null;
            }

            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        public void DoMonitorWord()
        {
            MonitorDataSave dataSave = new MonitorDataSave();
            dataSave.ReadDataFromFile();
            List<RiverWarningRecord> groupsRiver = dataSave.groupsRiver;
            List<ReservoirWarningRecord> groupsReservoir = dataSave.groupsReservoir;
            IEnumerable<ReservoirWarningRecord> keyReservoir = dataSave.keyReservoir;
            List<RiverWarningRecord> keyLake = dataSave.lakes;

            HydroMonitorReportData data = new HydroMonitorReportData();
            data.TemplateFullPath = this.templateName;
            data.OutPath = this.outPath;
            data.ReportPerson = this.makingPerson;
            data.ReportTime = this.makingTime;
            data.RiverWarningGroup = groupsRiver;
            data.ReservoirWarningGroup = groupsReservoir;
            data.KeyReservoirWarning = keyReservoir;
            data.LakeWarning = keyLake;

            WordMaking.MakingHydroMonitorReport(data);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            DoMonitorWord();
        }

        private void worker_RunWorkerCompleted(object sender,
                                       RunWorkerCompletedEventArgs e)
        {
            //update ui once worker complete his work
            this.Hint = "水情监测报告制作完成！";
        }

        private DateTime makingTime;
        public DateTime MakingTime
        {
            get
            {
                return this.makingTime;
            }
            set
            {
                this.makingTime = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(MakingTime)));
                }
            }
        }

        private string makingPerson;
        public string MakingPerson
        {
            get
            {
                return this.makingPerson;
            }
            set
            {
                this.makingPerson = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(MakingPerson)));
                }
            }
        }

        private string outPath;
        public string OutPath
        {
            get
            {
                return this.outPath;
            }
            set
            {
                this.outPath = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(OutPath)));
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

        private ObservableCollection<ProductPath> dirList;
        public ObservableCollection<ProductPath> DirList
        {
            get
            {
                return this.dirList;
            }
            set
            {
                this.dirList = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(DirList)));
                }
            }
        }

        private ProductPath selectItem;
        public ProductPath SelectItem
        {
            get
            {
                return this.selectItem;
            }
            set
            {
                this.selectItem = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectItem)));
                }
            }
        }

        public void Change()
        {
            if (this.selectItem != null)
            {
                if (this.selectItem.Path == null)
                {
                    this.OutPath = "";
                }
                else
                {
                    this.OutPath = this.selectItem.Path;
                }
            }
        }

        public void OpenDir()
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "指定输出目录";
            dlg.IsFolderPicker = true;
            //dlg.InitialDirectory = currentDirectory;

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            //dlg.DefaultDirectory = currentDirectory;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.OutPath = dlg.FileName;
                wd.Activate();
            }
        }

        public void WordMake()
        {
            if(this.makingPerson == null || this.makingPerson.Trim().Equals(string.Empty))
            {
                this.Hint = "必须指定制作人！";
                return;
            }

            if(this.outPath == null || this.outPath.Trim().Equals(string.Empty) || !Directory.Exists(this.outPath))
            {
                this.Hint = "必须指定一个有效的输出路径！";
                return;
            }

            if (this.templateName == null || this.templateName.Trim().Equals(string.Empty))
            {
                this.Hint = "未找到对应的模板!";
                return;
            }

            this.Hint = "水情监测报告生成中...";

            worker.RunWorkerAsync();

        }
    }
}
