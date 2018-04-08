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
    public class LanRcverConfigVm : INotifyPropertyChanged
    {
        public DelegateCommand AddCmd { get; set; }
        public DelegateCommand ModifyCmd { get; set; }
        public DelegateCommand DeleteCmd { get; set; }
        public DelegateCommand<object> ChangeCmd { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private LanAddressConfig config;

        public LanRcverConfigVm()
        {
            this.AddCmd = new DelegateCommand(Add);
            this.ModifyCmd = new DelegateCommand(Modify);
            this.DeleteCmd = new DelegateCommand(Delete);
            this.ChangeCmd = new DelegateCommand<object>(Change);
            config = new LanAddressConfig();
            this.ContactList = new ObservableCollection<LanAddress>(config.ListLanAddr);
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

        public void Add()
        {
            if (this.aliasName == null || this.aliasName.Trim().Equals(string.Empty)
                || this.path == null || this.path.Trim().Equals(string.Empty))
            {
                this.Hint = "地址名、LAN地址不能为空！";
            }
            else
            {
                LanAddress item = new LanAddress();
                item.LanAlias = this.aliasName.Trim();
                item.LanPath = this.path.Trim();
                if (this.userName != null)
                {
                    item.LanName = this.userName.Trim();
                }
                else
                {
                    item.LanName = "";
                }
                if (this.pwd != null)
                {
                    item.LanPwd = this.pwd.Trim();
                }
                else
                {
                    item.LanPwd = "";
                }
                this.ContactList.Add(item);
                config.ListLanAddr = this.contactList.ToList();
                config.WriteConfigToFile();
                this.Hint = "LAN地址保存成功!";
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
                this.Hint = "请先选择一个LAN地址!";
            }
            else
            {
                if (this.aliasName == null || this.aliasName.Trim().Equals(string.Empty)
                    || this.path == null || this.path.Trim().Equals(string.Empty))
                {
                    this.Hint = "地址名和LAN地址不能为空!";
                    return;
                }
                else
                {
                    this.ContactList[this.ContactList.IndexOf(this.selectItem)].LanAlias = this.aliasName.Trim();
                    this.ContactList[this.ContactList.IndexOf(this.selectItem)].LanPath = this.path.Trim();
                    if (this.userName != null)
                    {
                        this.ContactList[this.ContactList.IndexOf(this.selectItem)].LanName = this.userName.Trim();
                    }
                    else
                    {
                        this.ContactList[this.ContactList.IndexOf(this.selectItem)].LanName = "";
                    }

                    if (this.pwd != null)
                    {
                        this.ContactList[this.ContactList.IndexOf(this.selectItem)].LanPwd = this.pwd.Trim();
                    }
                    else
                    {
                        this.ContactList[this.ContactList.IndexOf(this.selectItem)].LanPwd = "";
                    }
                    config.ListLanAddr = this.contactList.ToList();
                    config.WriteConfigToFile();
                    this.ContactList = new ObservableCollection<LanAddress>(config.ListLanAddr);
                    this.Hint = "修改成功!";
                }
            }
        }

        public void Delete()
        {
            if (this.selectItem == null)
            {
                this.Hint = "请先选择一个LAN地址!";
            }
            else
            {
                this.contactList.Remove(this.selectItem);
                config.ListLanAddr = this.contactList.ToList();
                config.WriteConfigToFile();
                this.Hint = "LAN地址删除成功!";
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
                if (this.selectItem.LanAlias == null)
                {
                    this.AliasName = "";
                }
                else
                {
                    this.AliasName = this.selectItem.LanAlias;
                }

                if (this.selectItem.LanPath == null)
                {
                    this.Path = "";
                }
                else
                {
                    this.Path = this.selectItem.LanPath;
                }

                if (this.selectItem.LanName== null)
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
