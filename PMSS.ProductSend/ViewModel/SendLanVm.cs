using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using System.ComponentModel;
using PMSS.Send;
using Microsoft.Win32;
using PMSS.Configure;
using System.Collections.ObjectModel;

namespace PMSS.ProductSend.ViewModel
{
    public class SendLanVm : INotifyPropertyChanged
    {
        public DelegateCommand SendCmd { get; set; }
        public DelegateCommand OpenCmd { get; set; }
        public DelegateCommand<object> ChangeCmd { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private LanAddressConfig config;


        public SendLanVm()
        {
            this.SendCmd = new DelegateCommand(Send);
            this.OpenCmd = new DelegateCommand(OpenFile);
            this.ChangeCmd = new DelegateCommand<object>(Change);
            config = new LanAddressConfig();
            this.ContactList = new ObservableCollection<LanAddress>(config.ListLanAddr);
        }

        private string path;
        public string Path
        {
            get
            {
                return this.path;
            }
            set
            {
                this.path = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(Path)));
                }
            }
        }

        private string userName;
        public string UserName
        {
            get
            {
                return this.userName;
            }
            set
            {
                this.userName = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(UserName)));
                }
            }
        }

        private string pwd;
        public string Pwd
        {
            get
            {
                return this.pwd;
            }
            set
            {
                this.pwd = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(Pwd)));
                }
            }
        }

        private LanAddress selectItem;
        public LanAddress SelectItem
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

        private ObservableCollection<LanAddress> contactList;
        public ObservableCollection<LanAddress> ContactList
        {
            get
            {
                return this.contactList;
            }
            set
            {
                this.contactList = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(ContactList)));
                }
            }
        }

        private string fileName;
        public string FileName
        {
            get
            {
                return this.fileName;
            }
            set
            {
                this.fileName = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(FileName)));
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

        public void Send()
        {
            if (this.fileName == null || this.fileName.Trim().Equals(string.Empty))
            {
                this.Hint = "请指定上传的本地文件！";
                return;
            }
            
            if(this.path == null || this.path.Trim().Equals(string.Empty))
            {
                this.Hint = "请指定上传的局域网地址！";
                return;
            }                  

            LanSend lanSend = new LanSend(this.fileName.Trim(), this.path.Trim());

            if (this.userName != null && (!this.userName.Trim().Equals(string.Empty)))
            {
                lanSend.UserName = this.userName.Trim();
            }

            if (this.pwd != null && (!this.pwd.Trim().Equals(string.Empty)))
            {
                lanSend.Pwd = this.pwd.Trim();
            }

            if (lanSend.SendFile() == true)
            {
                this.Hint = "文件传输成功!";
            }
            else
            {
                this.Hint = "文件传输失败!";
            }
        }

        public void OpenFile()
        {
            OpenFileDialog op = new OpenFileDialog();
            if (op.ShowDialog() == true)
            {
                this.FileName = op.FileName;
                this.Hint = "已打开文件！";
            }
        }

        public void Change(object o)
        {
            if (this.selectItem != null)
            {
                if (this.selectItem.LanPath == null)
                {
                    this.Path = "";
                }
                else
                {
                    this.Path = this.selectItem.LanPath;
                }

                if (this.selectItem.LanName == null)
                {
                    this.UserName = "";
                }
                else
                {
                    this.UserName = this.selectItem.LanName;
                }

                if (this.selectItem.LanPwd == null)
                {
                    this.Pwd = "";
                }
                else
                {
                    this.Pwd = this.selectItem.LanPwd;
                }
            }
        }
    }
}
