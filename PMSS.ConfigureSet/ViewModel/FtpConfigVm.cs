using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using PMSS.Configure;

namespace PMSS.ConfigureSet.ViewModel
{
    public class FtpConfigVm : INotifyPropertyChanged
    {
        public DelegateCommand SaveConfigCmd { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

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

        public FtpConfigVm()
        {
            FtpConfig config = new FtpConfig();
            this.Address = config.FtpUri;
            this.UserName = config.FtpUser;
            this.Pwd = config.FtpPwd;
            this.SaveConfigCmd = new DelegateCommand(SaveConfig);
        }

        public void SaveConfig()
        {
            if (this.address == null || this.address.Trim().Equals(string.Empty))
            {
                this.ConfigHint = "FTP地址不能为空!";
            }
            else
            {
                this.ConfigHint = "保存成功!";
                FtpConfig config = new FtpConfig();
                config.FtpUri = this.address;
                config.FtpUser = this.userName;
                config.FtpPwd = this.pwd;
            }
        }
    }
}
