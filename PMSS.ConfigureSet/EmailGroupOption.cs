using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMSS.Configure;

namespace PMSS.ConfigureSet
{
    public class EmailGroupOption
    {
        public EmailAddress Address { get; set; }
        public string ShowWord
        {
            get
            {
                return this.Address.EmailAlias + "(" + this.Address.EmailName + ")";
            }
        }

        public bool IsSelected { get; set; }

        public EmailGroupOption(EmailAddress addr)
        {
            this.Address = addr;
            this.IsSelected = false;
        }
    }
}
