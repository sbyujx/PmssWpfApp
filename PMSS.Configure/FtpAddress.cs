using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSS.Configure
{
    public class FtpAddress
    {
        public string FtpAlias { get; set; }
        public string FtpPath { get; set; }
        public string FtpUserName { get; set; }
        public string FtpPwd { get; set; }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            FtpAddress p = obj as FtpAddress;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (this.FtpAlias.Equals(p.FtpAlias)) && (this.FtpPath.Equals(p.FtpPath)) && (this.FtpUserName.Equals(p.FtpUserName)) && (this.FtpPwd.Equals(p.FtpPwd));
        }

        public override int GetHashCode()
        {
            return this.FtpAlias.GetHashCode() + this.FtpPath.GetHashCode() + this.FtpUserName.GetHashCode() + this.FtpPwd.GetHashCode();
        }
    }
}
