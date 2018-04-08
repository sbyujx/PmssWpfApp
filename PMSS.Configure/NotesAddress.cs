using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSS.Configure
{
    public class NotesAddress
    {
        public string NotesAlias { get; set; }
        public string NotesName { get; set; }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            NotesAddress p = obj as NotesAddress;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (this.NotesAlias.Equals(p.NotesAlias)) && (this.NotesName.Equals(p.NotesName));
        }

        public override int GetHashCode()
        {
            return this.NotesAlias.GetHashCode() + this.NotesName.GetHashCode();
        }
    }
}
