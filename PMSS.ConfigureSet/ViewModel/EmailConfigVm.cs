using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using System.ComponentModel;
using PMSS.Configure;
using System.Threading;
using PMSS.Send;
using System.Net.Mail;

namespace PMSS.ConfigureSet.ViewModel
{
    public class EmailConfigVm : INotifyPropertyChanged
    {
        public DelegateCommand SaveConfigCmd { get; set; }
        public DelegateCommand VerifyConfigCmd { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
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
                if(this.PropertyChanged!=null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(ConfigHint)));
                }
            }
        }

        private string emailName;
        public string EmailName
        {
            get
            {
                return this.emailName;
            }
            set
            {
                this.emailName = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(EmailName)));
                }
            }
        }

        private string emailPwd;
        public string EmailPwd
        {
            get
            {
                return this.emailPwd;
            }
            set
            {
                this.emailPwd = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(EmailPwd)));
                }
            }
        }

        public EmailConfigVm()
        {
            this.SaveConfigCmd = new DelegateCommand(SaveConfig);
            this.VerifyConfigCmd = new DelegateCommand(VerifyConfig);
            EmailConfig config = new EmailConfig();
            this.EmailName = config.EmailName;
            this.EmailPwd = config.EmailPwd;
        }

        public void SaveConfig()
        {
            if (this.emailName == null || this.emailName.Trim().Equals(string.Empty) || this.emailPwd == null || this.emailPwd.Trim().Equals(string.Empty))
            {
                this.ConfigHint = "用户名或密码不能为空!";
            }
            else
            {
                EmailConfig config = new EmailConfig();
                config.EmailName = this.emailName.Trim();
                config.EmailPwd = this.emailPwd.Trim();
                this.ConfigHint = "配置保存成功!";
            }
        }

        public void VerifyConfig()
        {
            //检查邮箱格式
            SaveConfig();

            EmailSend email = new EmailSend();
            try
            {
                email.SendTestMail();
            }
            catch (SmtpException)
            {
                this.ConfigHint = "邮箱验证失败，请检查用户名和密码!";
                return;
            }
            this.ConfigHint = "已使用此配置向该邮箱发送了验证邮件，请查收！";
        }
    }
}
