using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMSS.Configure
{
    public class EmailConfig
    {
        public EmailConfig()
        {
            emailName = Properties.Settings.Default.EmailName;
            emailPwd = Properties.Settings.Default.EmailPwd;
        }

        private string emailName;
        public string EmailName 
        {
            get
            {
                return emailName;
            }
            set
            {
                emailName = value;
                Properties.Settings.Default.EmailName = value;
                Properties.Settings.Default.Save();
            }
        }

        private string emailPwd;
        public string EmailPwd
        {
            get
            {
                return emailPwd;
            }
            set
            {
                emailPwd = value;
                Properties.Settings.Default.EmailPwd = value;
                Properties.Settings.Default.Save();
            }
        }
    }
}
