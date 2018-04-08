using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using PMSS.Configure;
using Prism.Commands;
using System.ComponentModel;

namespace PMSS.ConfigureSet.ViewModel
{
    public class GroupConfigVm : INotifyPropertyChanged
    {
        public DelegateCommand AddCmd { get; set; }
        public DelegateCommand ModifyCmd { get; set; }
        public DelegateCommand DeleteCmd { get; set; }
        public DelegateCommand ChangeCmd { get; set; }

        private RcvGroupConfig groupConfig;

        public event PropertyChangedEventHandler PropertyChanged;

        public GroupConfigVm()
        {
            this.AddCmd = new DelegateCommand(Add);
            this.ModifyCmd = new DelegateCommand(Modify);
            this.DeleteCmd = new DelegateCommand(Delete);
            this.ChangeCmd = new DelegateCommand(Change);
            InitEmailInfo();
            InitNotesInfo();
            InitFtpInfo();
            InitLanInfo();
            InitGroupInfo();
        }


        private string groupName;
        public string GroupName
        {
            get
            {
                return this.groupName;
            }
            set
            {
                this.groupName = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(GroupName)));
                }
            }
        }

        private ObservableCollection<EmailGroupOption> emailOptions;
        public ObservableCollection<EmailGroupOption> EmailOptions
        {
            get
            {
                return this.emailOptions;
            }
            set
            {
                this.emailOptions = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(EmailOptions)));
                }
            }
        }

        private ObservableCollection<NotesGroupOption> notesOptions;
        public ObservableCollection<NotesGroupOption> NotesOptions
        {
            get
            {
                return this.notesOptions;
            }
            set
            {
                this.notesOptions = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(NotesOptions)));
                }
            }
        }

        private ObservableCollection<FtpGroupOption> ftpOptions;
        public ObservableCollection<FtpGroupOption> FtpOptions
        {
            get
            {
                return this.ftpOptions;
            }
            set
            {
                this.ftpOptions = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(FtpOptions)));
                }
            }
        }

        private ObservableCollection<LanGroupOption> lanOptions;
        public ObservableCollection<LanGroupOption> LanOptions
        {
            get
            {
                return this.lanOptions;
            }
            set
            {
                this.lanOptions = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(LanOptions)));
                }
            }
        }

        private ObservableCollection<RcvGroup> groups;
        public ObservableCollection<RcvGroup> Groups
        {
            get
            {
                return this.groups;
            }
            set
            {
                this.groups = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(Groups)));
                }
            }
        }

        private RcvGroup selectItem;
        public RcvGroup SelectItem
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

        public void InitEmailInfo()
        {
            EmailAddressConfig config = new EmailAddressConfig();
            this.EmailOptions = new ObservableCollection<EmailGroupOption>();
            foreach (var item in config.ListEmailAddr)
            {
                var op = new EmailGroupOption(item);
                this.EmailOptions.Add(op);
            }
        }

        public void InitNotesInfo()
        {
            NotesAddressConfig config = new NotesAddressConfig();
            this.NotesOptions = new ObservableCollection<NotesGroupOption>();
            foreach (var item in config.ListNotesAddr)
            {
                var op = new NotesGroupOption(item);
                this.NotesOptions.Add(op);
            }
        }

        public void InitFtpInfo()
        {
            FtpAddressConfig config = new FtpAddressConfig();
            this.FtpOptions = new ObservableCollection<FtpGroupOption>();
            foreach (var item in config.ListFtpAddr)
            {
                var op = new FtpGroupOption(item);
                this.FtpOptions.Add(op);
            }
        }

        public void InitLanInfo()
        {
            LanAddressConfig config = new LanAddressConfig();
            this.LanOptions = new ObservableCollection<LanGroupOption>();
            foreach (var item in config.ListLanAddr)
            {
                var op = new LanGroupOption(item);
                this.LanOptions.Add(op);
            }
        }

        public void InitGroupInfo()
        {
            this.groupConfig = new RcvGroupConfig();
            this.Groups = new ObservableCollection<RcvGroup>();
            foreach (var item in this.groupConfig.ListGroup)
            {
                this.Groups.Add(item);
            }
        }

        public void CopyNow(ref RcvGroup gp)
        {
            gp.GroupName = this.GroupName;

            gp.ListEmail = new List<EmailAddress>();
            foreach (var item in this.EmailOptions)
            {
                if (item.IsSelected == true)
                {
                    gp.ListEmail.Add(item.Address);
                }
            }

            gp.ListNotes = new List<NotesAddress>();
            foreach (var item in this.NotesOptions)
            {
                if (item.IsSelected == true)
                {
                    gp.ListNotes.Add(item.Address);
                }
            }

            gp.ListFtp = new List<FtpAddress>();
            foreach (var item in this.FtpOptions)
            {
                if (item.IsSelected == true)
                {
                    gp.ListFtp.Add(item.Address);
                }
            }

            gp.ListLan = new List<LanAddress>();
            foreach (var item in this.LanOptions)
            {
                if (item.IsSelected == true)
                {
                    gp.ListLan.Add(item.Address);
                }
            }
        }

        public void Add()
        {
            if (this.groupName == null || this.groupName.Trim().Equals(string.Empty))
            {
                this.Hint = "群组名不能为空！";
            }
            else
            {
                RcvGroup rgp = new RcvGroup();
                this.CopyNow(ref rgp);
                this.Groups.Add(rgp);
                this.groupConfig.ListGroup.Add(rgp);
                this.groupConfig.WriteConfigToFile();
                InitEmailInfo();
                InitNotesInfo();
                InitFtpInfo();
                InitLanInfo();
                this.GroupName = "";
                this.Hint = "添加配置成功！";
            }
        }

        public void Modify()
        {
            if (this.selectItem == null)
            {
                this.Hint = "请先选择一个群组!";
            }
            else
            {
                if (this.groupName == null || this.groupName.Trim().Equals(string.Empty))
                {
                    this.Hint = "群组名不能为空！";
                }
                else
                {
                    RcvGroup rgp = new RcvGroup();
                    this.CopyNow(ref rgp);
                    this.groupConfig.ListGroup[this.groupConfig.ListGroup.IndexOf(this.selectItem)] = rgp;
                    this.groupConfig.WriteConfigToFile();
                    this.Hint = "修改成功！";
                }
            }
        }

        public void Delete()
        {
            if (this.selectItem == null)
            {
                this.Hint = "请先选择一个群组!";
            }
            else
            {
                int index = this.Groups.IndexOf(this.selectItem);
                this.Groups.Remove(this.selectItem);
                this.groupConfig.ListGroup.RemoveAt(index);
                this.groupConfig.WriteConfigToFile();
                InitEmailInfo();
                InitNotesInfo();
                InitFtpInfo();
                InitLanInfo();
                this.GroupName = "";
                this.Hint = "群组删除成功!";
            }
        }

        public void Change()
        {
            if (this.selectItem != null)
            {
                this.GroupName = this.selectItem.GroupName;
                InitEmailInfo();
                foreach (var item in this.selectItem.ListEmail)
                {
                    var q = this.EmailOptions.Where(x => x.Address.Equals(item)).FirstOrDefault();
                    if(q != null)
                    {
                        q.IsSelected = true;
                    }
                }

                InitNotesInfo();
                foreach (var item in this.selectItem.ListNotes)
                {
                    var q = this.NotesOptions.Where(x => x.Address.Equals(item)).FirstOrDefault();
                    if (q != null)
                    {
                        q.IsSelected = true;
                    }
                }

                InitFtpInfo();
                foreach (var item in this.selectItem.ListFtp)
                {
                    var q = this.FtpOptions.Where(x => x.Address.Equals(item)).FirstOrDefault();
                    if (q != null)
                    {
                        q.IsSelected = true;
                    }
                }

                InitLanInfo();
                foreach (var item in this.selectItem.ListLan)
                {
                    var q = this.LanOptions.Where(x => x.Address.Equals(item)).FirstOrDefault();
                    if (q != null)
                    {
                        q.IsSelected = true;
                    }
                }
            }

        }
    }
}
