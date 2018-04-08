using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using System.ComponentModel;
using PMSS.Send;
using Microsoft.Win32;
using System.IO;
using PMSS.Configure;
using System.Collections.ObjectModel;

namespace PMSS.ProductSend.ViewModel
{
    public class SendFtpVm : INotifyPropertyChanged
    {
        public DelegateCommand SendCmd { get; set; }
        public DelegateCommand OpenCmd { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public DelegateCommand<object> ChangeCmd { get; set; }
        private FtpAddressConfig config;

        public SendFtpVm()
        {
            this.SendCmd = new DelegateCommand(Send);
            this.OpenCmd = new DelegateCommand(Open);
            this.ChangeCmd = new DelegateCommand<object>(Change);
            this.fileName = "";
            this.path = "";
            config = new FtpAddressConfig();
            this.ContactList = new ObservableCollection<FtpAddress>(config.ListFtpAddr);
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

        private FtpAddress selectItem;
        public FtpAddress SelectItem
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

        private ObservableCollection<FtpAddress> contactList;
        public ObservableCollection<FtpAddress> ContactList
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

        public void Send()
        {
            if (this.path == null || this.path.Trim().Equals(string.Empty))
            {
                this.Hint = "请指定上传的FTP目录！";
                return;
            }

            if (this.fileName == null || this.fileName.Trim().Equals(string.Empty))
            {
                this.Hint = "请选择一个文件!";
            }
            else
            {
                string newName = System.IO.Path.GetFileName(this.fileName);
                FtpUpload ftpUpload = new FtpUpload(this.path, this.fileName, newName);
                if (this.userName != null && (!this.userName.Trim().Equals(string.Empty)))
                {
                    ftpUpload.User = this.userName.Trim();
                }

                if (this.pwd != null && (!this.pwd.Trim().Equals(string.Empty)))
                {
                    ftpUpload.Pwd = this.pwd.Trim();
                }

                try
                {
                    ftpUpload.Upload();
                }
                catch(Exception)
                {
                    this.Hint = "文件上传失败，请检查FTP设置!";
                    return;
                }

                this.Hint = "文件上传成功!";
            }
        }

        public void Open()
        {
            OpenFileDialog op = new OpenFileDialog();
            if (op.ShowDialog() == true)
            {
                this.FileName = op.FileName;
                this.Hint = "文件打开成功!";
            }
        }

        public void Change(object o)
        {
            if (this.selectItem != null)
            {
                if (this.selectItem.FtpPath == null)
                {
                    this.Path = "";
                }
                else
                {
                    this.Path = this.selectItem.FtpPath;
                }

                if (this.selectItem.FtpUserName == null)
                {
                    this.UserName = "";
                }
                else
                {
                    this.UserName = this.selectItem.FtpUserName;
                }

                if (this.selectItem.FtpPwd == null)
                {
                    this.Pwd = "";
                }
                else
                {
                    this.Pwd = this.selectItem.FtpPwd;
                }
            }
        }
    }
}
