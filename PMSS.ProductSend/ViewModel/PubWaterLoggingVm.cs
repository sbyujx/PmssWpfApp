using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using System.ComponentModel;
using PMSS.Send;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using PMSS.Configure;
using System.IO;

namespace PMSS.ProductSend.ViewModel
{
    public class PubWaterLoggingVm : INotifyPropertyChanged
    {
        public DelegateCommand PubCmd { get; set; }
        public DelegateCommand AddAttCmd { get; set; }
        public DelegateCommand ConfirmCmd { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private NotesAddressConfig config;

        public PubWaterLoggingVm()
        {
            this.AddAttCmd = new DelegateCommand(AddAtt);
            this.PubCmd = new DelegateCommand(Pub);
            this.ConfirmCmd = new DelegateCommand(ConfirmSection);
            this.attList = new ObservableCollection<string>();
            config = new NotesAddressConfig();
        }

        private string title;
        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(Title)));
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


        private ObservableCollection<string> attList;
        public ObservableCollection<string> AttList
        {
            get
            {
                return this.attList;
            }
            set
            {
                this.attList = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(AttList)));
                }
            }
        }

        public void ConfirmSection()
        {
            try
            {
                int sec = int.Parse(this.Section);
                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month;
                int day = DateTime.Now.Day;
                this.Title = year + "年" + month + "月" + day + "日第" + sec + "期渍涝风险气象警报";
            }
            catch(Exception)
            {
                this.Hint = "请输入正确的数字产品期数！";
            }
        }

        public void AddAtt()
        {
            OpenFileDialog op = new OpenFileDialog();
            if (op.ShowDialog() == true)
            {
                this.AttList.Add(op.FileName);
            }
        }

        public void Pub()
        {
            NotesPub pub = new NotesPub();
            pub.Title = this.Title;

            if (this.attList.Count > 0)
            {
                List<string> attStrList = new List<string>();
                foreach (object item in this.attList)
                {
                    string fileName = (string)item;
                    attStrList.Add(fileName);
                }
                pub.AttFileName = attStrList;
            }

            if (this.Title.Trim().Equals(string.Empty))
            {
                this.Hint = "产品标题不能为空！";
            }
            else
            {
                try
                {
                    pub.Pub();
                }
                catch (Exception)
                {
                    this.Hint = "发布失败，请检查设置!";
                }
                this.Hint = "发布成功！";
            }
        }
    }
}
