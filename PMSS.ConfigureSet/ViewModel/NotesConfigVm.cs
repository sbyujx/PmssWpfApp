using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using PMSS.Configure;
using System.ComponentModel;

namespace PMSS.ConfigureSet.ViewModel
{
    public class NotesConfigVm : INotifyPropertyChanged
    {
        public DelegateCommand SaveConfigCmd { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private string severName;
        public string ServerName
        {
            get
            {
                return this.severName;
            }
            set
            {
                this.severName = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(ServerName)));
                }
            }
        }

        private string nsfName;
        public string NsfName
        {
            get
            {
                return this.nsfName;
            }
            set
            {
                this.nsfName = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(NsfName)));
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

        private string configHint;
        public string ConfigHint
        {
            get
            {
                return this.configHint;
            }
            set
            {
                this.configHint = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(ConfigHint)));
                }
            }
        }

        public NotesConfigVm()
        {
            this.SaveConfigCmd = new DelegateCommand(SaveConfig);
            NotesConfig config = new NotesConfig();
            this.ServerName = config.NotesServer;
            this.NsfName = config.NotesFile;
            this.Pwd = config.NotesPwd;
        }

        public void SaveConfig()
        {
            if (this.severName == null || this.severName.Trim().Equals(string.Empty)
                || this.nsfName == null || this.nsfName.Trim().Equals(string.Empty)
                || this.pwd == null || this.pwd.Trim().Equals(string.Empty))
            {
                this.ConfigHint = "服务器名、NOTES配置文件名、密码不能为空！";
            }
            else
            {
                NotesConfig config = new NotesConfig();
                config.NotesServer = this.severName;
                config.NotesFile = this.nsfName;
                config.NotesPwd = this.pwd;
                this.ConfigHint = "保存成功！";
            }
        }
    }
}
