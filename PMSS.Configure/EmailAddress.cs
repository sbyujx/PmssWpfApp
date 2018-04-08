using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSS.Configure
{
    public class EmailAddress
    {
        public string EmailAlias { get; set; }
        public string EmailName { get; set; }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            EmailAddress p = obj as EmailAddress;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (this.EmailAlias.Equals(p.EmailAlias)) && (this.EmailName.Equals(p.EmailName));
        }

        public override int GetHashCode()
        {
            return this.EmailAlias.GetHashCode() + this.EmailName.GetHashCode();
        }
    }
}
