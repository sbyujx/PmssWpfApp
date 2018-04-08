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
    public class SendNotesVm : INotifyPropertyChanged
    {
        public DelegateCommand SendCmd { get; set; }
        public DelegateCommand AddAttCmd { get; set; }
        public DelegateCommand AddRcverCmd { get; set; }
        public DelegateCommand SelectContactCmd { get; set; }
        public DelegateCommand SelectRcverCmd { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private NotesAddressConfig config;

        public SendNotesVm()
        {
            this.AddAttCmd = new DelegateCommand(AddAtt);
            this.SendCmd = new DelegateCommand(Send);
            this.AddRcverCmd = new DelegateCommand(AddRcver);
            this.SelectContactCmd = new DelegateCommand(AddContanctToRcver);
            this.SelectRcverCmd = new DelegateCommand(RemoveRcver);
            this.rcverList = new ObservableCollection<NotesAddress>();
            this.attList = new ObservableCollection<string>();
            config = new NotesAddressConfig();
            this.ContactList = new ObservableCollection<NotesAddress>(config.ListNotesAddr);
        }

        private NotesAddress selectContactItem;
        public NotesAddress SelectContactItem
        {
            get
            {
                return this.selectContactItem;
            }
            set
            {
                this.selectContactItem = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(selectContactItem)));
                }
            }
        }

        private NotesAddress selectRcverItem;
        public NotesAddress SelectRcverItem
        {
            get
            {
                return this.selectRcverItem;
            }
            set
            {
                this.selectRcverItem = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectRcverItem)));
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

        private string subject;
        public string Subject
        {
            get
            {
                return this.subject;
            }
            set
            {
                this.subject = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(Subject)));
                }
            }
        }

        private string rcverText;
        public string RcverText
        {
            get
            {
                return this.rcverText;
            }
            set
            {
                this.rcverText = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(RcverText)));
                }
            }
        }

        private string body;
        public string Body
        {
            get
            {
                return this.body;
            }
            set
            {
                this.body = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(Body)));
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

        private ObservableCollection<NotesAddress> rcverList;
        public ObservableCollection<NotesAddress> RcverList
        {
            get
            {
                return this.rcverList;
            }
            set
            {
                this.rcverList = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(RcverList)));
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


        public void AddAtt()
        {
            OpenFileDialog op = new OpenFileDialog();
            if (op.ShowDialog() == true)
            {
                this.AttList.Add(op.FileName);
                FileInfo fi = new FileInfo(op.FileName);
                if (fi.Extension.Equals(".doc") || fi.Extension.Equals(".docx"))
                {
                    string name = Path.GetFileNameWithoutExtension(op.FileName);
                    this.Subject = name;
                }
            }
        }

        public void AddRcver()
        {
            if (this.rcverText == null || this.rcverText.Trim().Equals(string.Empty))
            {
                this.Hint = "添加收件人时，不能为空！";
            }
            else
            {
                NotesAddress addr = new NotesAddress();
                addr.NotesAlias = this.rcverText;
                addr.NotesName = "";
                this.RcverList.Add(addr);
                this.RcverText = "";
            }
        }

        public void Send()
        {
            NotesSend notes = new NotesSend();
            notes.Subject = this.subject;
            notes.Body = this.body;
            List<string> to = new List<string>();
            foreach (var item in this.rcverList)
            {
                string str = item.NotesName;
                to.Add(str);
            }
            notes.MailTo = to;

            if (this.attList.Count > 0)
            {
                List<string> attStrList = new List<string>();
                foreach (object item in this.attList)
                {
                    string fileName = (string)item;
                    attStrList.Add(fileName);
                }
                notes.AttFileName = attStrList;
            }

            if (to.Count == 0)
            {
                this.Hint = "收件人不能为空！";
            }
            else
            {
                try
                {
                    notes.SendMail();
                }
                catch(Exception)
                {
                    this.Hint = "邮件发送失败，请检查设置!";
                }
                this.Hint = "邮件发送成功！";
            }
        }

        public void AddContanctToRcver()
        {
            if (this.selectContactItem != null)
            {
                this.RcverList.Add(this.selectContactItem);
            }
        }

        public void RemoveRcver()
        {
            if (this.selectRcverItem != null)
            {
                this.RcverList.Remove(this.selectRcverItem);
            }
        }
    }
}
