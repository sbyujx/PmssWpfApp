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
    public class NotesRcverConfigVm : INotifyPropertyChanged
    {
        public DelegateCommand AddCmd { get; set; }
        public DelegateCommand ModifyCmd { get; set; }
        public DelegateCommand DeleteCmd { get; set; }
        public DelegateCommand<object> ChangeCmd { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private NotesAddressConfig config;

        public NotesRcverConfigVm()
        {
            this.AddCmd = new DelegateCommand(Add);
            this.ModifyCmd = new DelegateCommand(Modify);
            this.DeleteCmd = new DelegateCommand(Delete);
            this.ChangeCmd = new DelegateCommand<object>(Change);
            config = new NotesAddressConfig();
            this.ContactList = new ObservableCollection<NotesAddress>(config.ListNotesAddr);
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

        private ObservableCollection<NotesAddress> contactList;
        public ObservableCollection<NotesAddress> ContactList
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

        private NotesAddress selectItem;
        public NotesAddress SelectItem
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
                this.Hint = "姓名、Notes地址不能为空！";
            }
            else
            {
                NotesAddress item = new NotesAddress();
                item.NotesAlias = this.name.Trim();
                item.NotesName = this.address.Trim();
                this.ContactList.Add(item);
                config.ListNotesAddr = this.contactList.ToList();
                config.WriteConfigToFile();
                this.Hint = "Notes联系人保存成功!";
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
                    this.Hint = "联系人或Notes地址不能为空!";
                    return;
                }
                else
                {
                    this.ContactList[this.ContactList.IndexOf(this.selectItem)].NotesAlias = this.name;
                    this.ContactList[this.ContactList.IndexOf(this.selectItem)].NotesName = this.address;
                    config.ListNotesAddr = this.contactList.ToList();
                    config.WriteConfigToFile();
                    this.ContactList = new ObservableCollection<NotesAddress>(config.ListNotesAddr);
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
                config.ListNotesAddr = this.contactList.ToList();
                config.WriteConfigToFile();
                this.Hint = "Notes联系人删除成功!";
                this.Address = "";
                this.Name = "";
            }
        }

        public void Change(object o)
        {
            if (this.selectItem != null)
            {
                if (this.selectItem.NotesName == null)
                {
                    this.Address = "";
                }
                else
                {
                    this.Address = this.selectItem.NotesName;
                }

                if (this.selectItem.NotesAlias == null)
                {
                    this.Name = "";
                }
                else
                {
                    this.Name = this.selectItem.NotesAlias;
                }
            }
        }
    }
}
