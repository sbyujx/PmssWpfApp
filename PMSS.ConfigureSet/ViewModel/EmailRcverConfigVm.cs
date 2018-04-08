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
    public class EmailRcverConfigVm : INotifyPropertyChanged
    {
        public DelegateCommand AddCmd { get; set; }
        public DelegateCommand ModifyCmd { get; set; }
        public DelegateCommand DeleteCmd { get; set; }
        public DelegateCommand<object> ChangeCmd { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private EmailAddressConfig config;

        public EmailRcverConfigVm()
        {
            this.AddCmd = new DelegateCommand(Add);
            this.ModifyCmd = new DelegateCommand(Modify);
            this.DeleteCmd = new DelegateCommand(Delete);
            this.ChangeCmd = new DelegateCommand<object>(Change);
            config = new EmailAddressConfig();
            this.ContactList = new ObservableCollection<EmailAddress>(config.ListEmailAddr);
        }

        private string name;
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            }
        }

        private string address;
        public string Address
        {
            get
            {
                return this.address;
            }
            set
            {
                this.address = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(Address)));
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

        private ObservableCollection<EmailAddress> contactList;
        public ObservableCollection<EmailAddress> ContactList
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

        private EmailAddress selectItem;
        public EmailAddress SelectItem
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
            if (this.address == null || this.address.Trim().Equals(string.Empty)
                || this.name == null || this.name.Trim().Equals(string.Empty))
            {
                this.Hint = "姓名、Email地址不能为空！";
            }
            else
            {
                EmailAddress item = new EmailAddress();
                item.EmailAlias = this.name.Trim();
                item.EmailName = this.address.Trim();
                this.ContactList.Add(item);
                config.ListEmailAddr = this.contactList.ToList();
                config.WriteConfigToFile();
                this.Hint = "Email联系人保存成功!";
                this.Address = "";
                this.Name = "";
            }
        }

        public void Modify()
        {
            if (this.selectItem == null)
            {
                this.Hint = "请先选择一个联系人!";
            }
            else
            {
                if (this.name == null || this.name.Trim().Equals(string.Empty)
                    || this.address == null || this.address.Trim().Equals(string.Empty))
                {
                    this.Hint = "联系人或Email不能为空!";
                    return;
                }
                else
                {
                    this.ContactList[this.ContactList.IndexOf(this.selectItem)].EmailAlias = this.name;
                    this.ContactList[this.ContactList.IndexOf(this.selectItem)].EmailName = this.address;
                    config.ListEmailAddr = this.contactList.ToList();
                    config.WriteConfigToFile();
                    this.ContactList = new ObservableCollection<EmailAddress>(config.ListEmailAddr);
                    this.Hint = "修改成功!";
                }
            }
        }

        public void Delete()
        {
            if (this.selectItem == null)
            {
                this.Hint = "请先选择一个联系人!";
            }
            else
            {
                this.contactList.Remove(this.selectItem);
                config.ListEmailAddr = this.contactList.ToList();
                config.WriteConfigToFile();
                this.Hint = "Email联系人删除成功!";
                this.Address = "";
                this.Name = "";
            }
        }

        public void Change(object o)
        {
            if (this.selectItem != null)
            {
                if (this.selectItem.EmailName == null)
                {
                    this.Address = "";
                }
                else
                {
                    this.Address = this.selectItem.EmailName;
                }

                if (this.selectItem.EmailAlias == null)
                {
                    this.Name = "";
                }
                else
                {
                    this.Name = this.selectItem.EmailAlias;
                }
            }
        }
    }
}
