using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSS.Configure
{
    public class LanAddress
    {
        public string LanAlias { get; set; }
        public string LanPath { get; set; }
        public string LanName { get; set; }
        public string LanPwd { get; set; }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            LanAddress p = obj as LanAddress;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (this.LanAlias.Equals(p.LanAlias)) && (this.LanPath.Equals(p.LanPath)) && (this.LanName.Equals(p.LanName)) && (this.LanPwd.Equals(p.LanPwd));
        }

        public override int GetHashCode()
        {
            return this.LanAlias.GetHashCode() + this.LanPath.GetHashCode() + this.LanName.GetHashCode() + this.LanPwd.GetHashCode();
        }
    }
}
