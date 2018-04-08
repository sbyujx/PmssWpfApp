using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using System.ComponentModel;
using PMSS.WordProduct;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using PMSS.Configure;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace PMSS.WordProductUI.ViewModel
{
    public class WaterloggingWordVm : INotifyPropertyChanged
    {
        public Window wd;
        public DelegateCommand OpenPicCmd { get; set; }
        public DelegateCommand OpenOutPathCmd { get; set; }
        public DelegateCommand WordMakeCmd { get; set; }
        public DelegateCommand<object> ChangeCmd { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        private ProductPathConfig config;

        private string templateName;
        private readonly string templateFileName = "渍涝模板.doc";
        private readonly string dir = "WordTemplate";


        public WaterloggingWordVm()
        {
            this.OpenPicCmd = new DelegateCommand(OpenPic);
            this.OpenOutPathCmd = new DelegateCommand(OpenOutPath);
            this.WordMakeCmd = new DelegateCommand(WordMake);
            this.ChangeCmd = new DelegateCommand<object>(Change);

            config = new ProductPathConfig();
            this.DirList = new ObservableCollection<ProductPath>(config.ListProductPath);

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

            PreAndSignConfig cf = new PreAndSignConfig();
            this.PrePeople = cf.Pas.PreName;
            this.SignPeople = cf.Pas.SignName;
            this.Section = new ProductVolumeConfig().productVolume.WaterloggingVolume.ToString();
        }

        private string prePeople;
        public string PrePeople
        {
            get
            {
                return this.prePeople;
            }
            set
            {
                this.prePeople = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(PrePeople)));
                }
            }
        }

        private string signPeople;
        public string SignPeople
        {
            get
            {
                return this.signPeople;
            }
            set
            {
                this.signPeople = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(SignPeople)));
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

        private string picName;
        public string PicName
        {
            get
            {
                return this.picName;
            }
            set
            {
                this.picName = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(PicName)));
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

        private string section;
        public string Section
        {
            get
            {
                return this.section;
            }
            set
            {
                this.section = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(Section)));
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

        public void Change(object o)
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

        public void OpenPic()
        {
            OpenFileDialog op = new OpenFileDialog();
            if (op.ShowDialog() == true)
            {
                this.PicName = op.FileName;
            }
        }

        public void OpenOutPath()
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
            if (this.templateName == null || this.templateName.Trim().Equals(string.Empty))
            {
                this.Hint = "未找到对应的模板!";
                return;
            }

            if (this.picName == null || this.picName.Trim().Equals(string.Empty))
            {
                this.Hint = "必须指定图片产品!";
                return;
            }

            if (this.outPath == null || this.outPath.Trim().Equals(string.Empty))
            {
                this.Hint = "必须一个输出目录!";
                return;
            }

            if (this.section == null || this.section.Trim().Equals(string.Empty))
            {
                this.Hint = "必须指定产品期数!";
                return;
            }

            if (this.signPeople == null || this.signPeople.Trim().Equals(string.Empty))
            {
                this.Hint = "必须指定签发人！";
                return;
            }

            if (this.prePeople == null || this.prePeople.Trim().Equals(string.Empty))
            {
                this.Hint = "必须指定预报人！";
                return;
            }

            try
            {
                int volume = int.Parse(this.section);
                volume++;
                ProductVolumeConfig con = new ProductVolumeConfig();
                con.productVolume.WaterloggingVolume = volume;
                con.WriteConfigToFile();
            }
            catch (Exception)
            {
                this.Hint = "请输入有效的产品期数数字！";
                return;
            }

            string outFileYear = (DateTime.Now.Year % 100).ToString("00");
            string outFileMonth = DateTime.Now.Month.ToString("00");
            string outFileDay = DateTime.Now.Day.ToString("00");
            string outFileHour = DateTime.Now.Hour.ToString("00");
            string outFileName = this.outPath + "/zl" + outFileYear + outFileMonth + outFileDay + outFileHour;
            DateTime tomorrow = DateTime.Now.AddDays(1);

            WordConfig config = new WordConfig(this.templateName, outFileName);
            PicBookmark pbm = new PicBookmark();
            pbm.PicFileName = this.picName;
            pbm.Bookmark = "图形产品";
            /*pbm.ListTextBox = new List<TextBoxOnPic>();
            TextBoxOnPic titleText = new TextBoxOnPic
            {
                Left = 250,
                Top = 364,
                Width = 140,
                Height = 21,
                Bold = 1,
                FontName = "宋体",
                Size = 11,
                Text = "全国渍涝风险气象预报图" };
            pbm.ListTextBox.Add(titleText);
            TextBoxOnPic fromToText = new TextBoxOnPic
            {
                Left = 240,
                Top = 390,
                Width = 150,
                Height = 19,
                Bold = 0,
                FontName = "宋体",
                Size = 7,
                Text = DateTime.Now.Year + "年" + outFileMonth + "月" + outFileDay + "日" + outFileHour + "时～" + tomorrow.Month.ToString("00") + "月"
                + tomorrow.Day.ToString("00") + "日" + outFileHour + "时"
            };
            pbm.ListTextBox.Add(fromToText);
            TextBoxOnPic pubText = new TextBoxOnPic
            {
                Left = 250,
                Top = 420,
                Width = 120,
                Height = 19,
                Bold = 0,
                FontName = "宋体",
                Size = 7,
                Text = "中央气象台" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日" + DateTime.Now.Hour + "时发布"
            };
            pbm.ListTextBox.Add(pubText);*/
            config.ListPicBookmark.Add(pbm);

            TextBookmark sign = new TextBookmark { Bookmark = "签发人", Text = this.signPeople };
            config.ListTextBookmark.Add(sign);
            TextBookmark pre = new TextBookmark { Bookmark = "预报人", Text = this.prePeople };
            config.ListTextBookmark.Add(pre);

            TextBookmark fromDay = new TextBookmark { Bookmark = "起始日", Text = DateTime.Now.Day.ToString() };
            config.ListTextBookmark.Add(fromDay);
            TextBookmark fromHour = new TextBookmark { Bookmark = "起始时", Text = DateTime.Now.Hour.ToString() };
            config.ListTextBookmark.Add(fromHour);
            TextBookmark fromMonth = new TextBookmark { Bookmark = "起始月", Text = DateTime.Now.Month.ToString() };
            config.ListTextBookmark.Add(fromMonth);

            TextBookmark PubDay = new TextBookmark { Bookmark = "发布日", Text = DateTime.Now.Day.ToString() };
            config.ListTextBookmark.Add(PubDay);
            TextBookmark PubHour = new TextBookmark { Bookmark = "发布时", Text = DateTime.Now.Hour.ToString() };
            config.ListTextBookmark.Add(PubHour);
            TextBookmark PubMonth = new TextBookmark { Bookmark = "发布月", Text = DateTime.Now.Month.ToString() };
            config.ListTextBookmark.Add(PubMonth);

            TextBookmark sect = new TextBookmark { Bookmark = "期数", Text = this.section };
            config.ListTextBookmark.Add(sect);
            TextBookmark SectionYear = new TextBookmark { Bookmark = "期数年", Text = DateTime.Now.Year.ToString() };
            config.ListTextBookmark.Add(SectionYear);

            TextBookmark TitleDay = new TextBookmark { Bookmark = "标题日", Text = DateTime.Now.Day.ToString() };
            config.ListTextBookmark.Add(TitleDay);
            TextBookmark TitleHour = new TextBookmark { Bookmark = "标题时", Text = DateTime.Now.Hour.ToString() };
            config.ListTextBookmark.Add(TitleHour);
            TextBookmark TitleMonth = new TextBookmark { Bookmark = "标题月", Text = DateTime.Now.Month.ToString() };
            config.ListTextBookmark.Add(TitleMonth);
            TextBookmark TitleYear = new TextBookmark { Bookmark = "标题年", Text = DateTime.Now.Year.ToString() };
            config.ListTextBookmark.Add(TitleYear);


            TextBookmark ToDay = new TextBookmark { Bookmark = "结束日", Text = tomorrow.Day.ToString() };
            config.ListTextBookmark.Add(ToDay);
            TextBookmark ToHour = new TextBookmark { Bookmark = "结束时", Text = DateTime.Now.Hour.ToString() };
            config.ListTextBookmark.Add(ToHour);
            TextBookmark ToMonth = new TextBookmark { Bookmark = "结束月", Text = tomorrow.Month.ToString() };
            config.ListTextBookmark.Add(ToMonth);

            WordMaking.MakingViaConfig(config);
            this.Hint = "文字产品生成成功!";
            this.Section = new ProductVolumeConfig().productVolume.WaterloggingVolume.ToString();
        }
    }
}
