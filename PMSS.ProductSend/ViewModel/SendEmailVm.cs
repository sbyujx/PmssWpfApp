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
    public class SendEmailVm : INotifyPropertyChanged
    {
        public DelegateCommand SendCmd { get; set; }
        public DelegateCommand AddAttCmd { get; set; }
        public DelegateCommand AddRcverCmd { get; set; }
        public DelegateCommand SelectContactCmd { get; set; }
        public DelegateCommand SelectRcverCmd { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private EmailAddressConfig config;

        public SendEmailVm()
        {
            this.AddAttCmd = new DelegateCommand(AddAtt);
            this.SendCmd = new DelegateCommand(Send);
            this.AddRcverCmd = new DelegateCommand(AddRcver);
            this.SelectContactCmd = new DelegateCommand(AddContanctToRcver);
            this.SelectRcverCmd = new DelegateCommand(RemoveRcver);
            this.rcverList = new ObservableCollection<EmailAddress>();
            this.attList = new ObservableCollection<string>();
            config = new EmailAddressConfig();
            this.ContactList = new ObservableCollection<EmailAddress>(config.ListEmailAddr);
        }

        private EmailAddress selectContactItem;
        public EmailAddress SelectContactItem
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

        private EmailAddress selectRcverItem;
        public EmailAddress SelectRcverItem
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

        private ObservableCollection<EmailAddress> rcverList;
        public ObservableCollection<EmailAddress> RcverList
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
                if(fi.Extension.Equals(".doc") || fi.Extension.Equals(".docx"))
                {
                    string name = Path.GetFileNameWithoutExtension(op.FileName);
                    this.Subject = name;
                }
            }
        }

        public void AddRcver()
        {
            if(this.rcverText == null || this.rcverText.Trim().Equals(string.Empty))
            {
                this.Hint = "添加收件人时，不能为空！";
            }
            else
            {
                EmailAddress addr = new EmailAddress();
                addr.EmailName = this.rcverText;
                addr.EmailAlias = "";
                this.RcverList.Add(addr);
                this.RcverText = "";
            }
        }

        public void Send()
        {
            EmailSend email = new EmailSend();
            email.Subject = this.subject;
            email.Body = this.body;
            List<string> to = new List<string>();
            foreach (var item in this.rcverList)
            {
                string str = item.EmailName;
                to.Add(str);
            }
            email.EmailTo = to;

            if(this.attList.Count > 0)
            {
                List<string> attStrList = new List<string>();
                foreach (object item in this.attList)
                {
                    string fileName = (string)item;
                    attStrList.Add(fileName);
                }
                email.AttFileName = attStrList;
            }

            if(to.Count == 0)
            {
                this.Hint = "收件人不能为空！";
            }
            else
            {
                email.Send();
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
            if(this.selectRcverItem != null)
            {
                this.RcverList.Remove(this.selectRcverItem);
            }
        }
    }
}
