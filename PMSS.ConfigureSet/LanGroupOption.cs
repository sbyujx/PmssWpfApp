using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMSS.Configure;

namespace PMSS.ConfigureSet
{
    public class LanGroupOption
    {
        public LanAddress Address { get; set; }
        public string ShowWord
        {
            get
            {
                return this.Address.LanAlias + "(" + this.Address.LanPath + ")";
            }
        }

        public bool IsSelected { get; set; }

        public LanGroupOption(LanAddress addr)
        {
            this.Address = addr;
            this.IsSelected = false;
        }
    }
}
