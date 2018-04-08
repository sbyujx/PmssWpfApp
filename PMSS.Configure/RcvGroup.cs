using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSS.Configure
{
    public class RcvGroup
    {
        public string GroupName { get; set; }
        public List<EmailAddress> ListEmail { get; set; }
        public List<NotesAddress> ListNotes { get; set; }
        public List<FtpAddress> ListFtp { get; set; }
        public List<LanAddress> ListLan { get; set; }
    }
}
