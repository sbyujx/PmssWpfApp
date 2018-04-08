using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMSS.Configure;
using Prism.Commands;
using Prism.Events;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace PMSS.ConfigureSet.ViewModel
{
    public class FtpRcverConfigVm : INotifyPropertyChanged
    {
        public DelegateCommand AddCmd { get; set; }
        public DelegateCommand ModifyCmd { get; set; }
        public DelegateCommand DeleteCmd { get; set; }
        public DelegateCommand<object> ChangeCmd { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private FtpAddressConfig config;

        public FtpRcverConfigVm()
        {
            this.AddCmd = new DelegateCommand(Add);
            this.ModifyCmd = new DelegateCommand(Modify);
            this.DeleteCmd = new DelegateCommand(Delete);
            this.ChangeCmd = new DelegateCommand<object>(Change);
            config = new FtpAddressConfig();
            this.ContactList = new ObservableCollection<FtpAddress>(config.ListFtpAddr);
        }

        private string aliasName;
        public string AliasName
        {
            get
            {
                return this.aliasName;
            }
            set
            {
                this.aliasName = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(AliasName)));
                }
            }
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

        public void Add()
        {
            if (this.aliasName == null || this.aliasName.Trim().Equals(string.Empty)
                || this.path == null || this.path.Trim().Equals(string.Empty))
            {
                this.Hint = "地址名、FTP地址不能为空！";
            }
            else
            {
                FtpAddress item = new FtpAddress();
                item.FtpAlias = this.aliasName.Trim();
                item.FtpPath = this.path.Trim();
                if (this.userName != null)
                {
                    item.FtpUserName = this.userName.Trim();
                }
                else
                {
                    item.FtpUserName = "";
                }
                if (this.pwd != null)
                {
                    item.FtpPwd = this.pwd.Trim();
                }
                else
                {
                    item.FtpPwd = "";
                }
                this.ContactList.Add(item);
                config.ListFtpAddr = this.contactList.ToList();
                config.WriteConfigToFile();
                this.Hint = "FTP地址保存成功!";
                this.AliasName = "";
                this.Path = "";
                this.UserName = "";
                this.Pwd = "";
            }
        }

        public void Modify()
        {
            if (this.selectItem == null)
            {
                this.Hint = "请先选择一个FTP地址!";
            }
            else
            {
                if (this.aliasName == null || this.aliasName.Trim().Equals(string.Empty)
                    || this.path == null || this.path.Trim().Equals(string.Empty))
                {
                    this.Hint = "地址名和FTP地址不能为空!";
                    return;
                }
                else
                {
                    this.ContactList[this.ContactList.IndexOf(this.selectItem)].FtpAlias = this.aliasName.Trim();
                    this.ContactList[this.ContactList.IndexOf(this.selectItem)].FtpPath = this.path.Trim();
                    if (this.userName != null)
                    {
                        this.ContactList[this.ContactList.IndexOf(this.selectItem)].FtpUserName = this.userName.Trim();
                    }
                    else
                    {
                        this.ContactList[this.ContactList.IndexOf(this.selectItem)].FtpUserName = "";
                    }

                    if (this.pwd != null)
                    {
                        this.ContactList[this.ContactList.IndexOf(this.selectItem)].FtpPwd = this.pwd.Trim();
                    }
                    else
                    {
                        this.ContactList[this.ContactList.IndexOf(this.selectItem)].FtpPwd = "";
                    }
                    config.ListFtpAddr = this.contactList.ToList();
                    config.WriteConfigToFile();
                    this.ContactList = new ObservableCollection<FtpAddress>(config.ListFtpAddr);
                    this.Hint = "修改成功!";
                }
            }
        }

        public void Delete()
        {
            if (this.selectItem == null)
            {
                this.Hint = "请先选择一个FTP地址!";
            }
            else
            {
                this.contactList.Remove(this.selectItem);
                config.ListFtpAddr = this.contactList.ToList();
                config.WriteConfigToFile();
                this.Hint = "FTP地址删除成功!";
                this.AliasName = "";
                this.Path = "";
                this.UserName = "";
                this.Pwd = "";
            }
        }

        public void Change(object o)
        {
            if (this.selectItem != null)
            {
                if (this.selectItem.FtpAlias == null)
                {
                    this.AliasName = "";
                }
                else
                {
                    this.AliasName = this.selectItem.FtpAlias;
                }

                if (this.selectItem.FtpPath == null)
                {
                    this.Path = "";
                }
                else
                {
                    this.Path = this.selectItem.FtpPath;
                }

                if (this.selectItem.FtpUserName== null)
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
