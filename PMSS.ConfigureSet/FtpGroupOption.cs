using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMSS.Configure;

namespace PMSS.ConfigureSet
{
    public class FtpGroupOption
    {
        public FtpAddress Address { get; set; }
        public string ShowWord
        {
            get
            {
                return this.Address.FtpAlias + "(" + this.Address.FtpPath + ")";
            }
        }

        public bool IsSelected { get; set; }

        public FtpGroupOption(FtpAddress addr)
        {
            this.Address = addr;
            this.IsSelected = false;
        }
    }
}
