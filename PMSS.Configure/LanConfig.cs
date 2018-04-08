using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMSS.Configure
{
    public class LanConfig
    {
        public LanConfig()
        {
            lanAddr = Properties.Settings.Default.LanAddr;
            lanUser = Properties.Settings.Default.LanUser;
            lanPwd = Properties.Settings.Default.LanPwd;
        }

        private string lanAddr;
        public string LanAddr
        {
            get
            {
                return lanAddr;
            }
            set
            {
                lanAddr = value;
                Properties.Settings.Default.LanAddr = value;
                Properties.Settings.Default.Save();
            }
        }

        private string lanUser;
        public string LanUser
        {
            get
            {
                return lanUser;
            }
            set
            {
                lanUser = value;
                Properties.Settings.Default.LanUser = value;
                Properties.Settings.Default.Save();
            }
        }

        private string lanPwd;
        public string LanPwd
        {
            get
            {
                return lanPwd;
            }
            set
            {
                lanPwd = value;
                Properties.Settings.Default.LanPwd = value;
                Properties.Settings.Default.Save();
            }
        }
    }
}
