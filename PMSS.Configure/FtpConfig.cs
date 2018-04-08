using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMSS.Configure
{
    public class FtpConfig
    {
        public FtpConfig()
        {
            ftpUri = Properties.Settings.Default.FtpUri;
            ftpUser = Properties.Settings.Default.FtpUser;
            ftpPwd = Properties.Settings.Default.FtpPwd;
        }

        private string ftpUri;
        public string FtpUri
        {
            get
            {
                return ftpUri;
            }
            set
            {
                ftpUri = value;
                Properties.Settings.Default.FtpUri = value;
                Properties.Settings.Default.Save();
            }
        }

        private string ftpUser;
        public string FtpUser
        {
            get
            {
                return ftpUser;
            }
            set
            {
                ftpUser = value;
                Properties.Settings.Default.FtpUser = value;
                Properties.Settings.Default.Save();
            }
        }

        private string ftpPwd;
        public string FtpPwd
        {
            get
            {
                return ftpPwd;
            }
            set
            {
                ftpPwd = value;
                Properties.Settings.Default.FtpPwd = value;
                Properties.Settings.Default.Save();
            }
        }


    }
}
