using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSS.CrawlerService
{
    public class OldData
    {

        public OldData(string id, DateTime t)
        {
            Uid = id;
            Time = t;
        }

        private string uid; //站点号
        public string Uid
        {
            get
            {
                return uid;
            }
            set
            {
                uid = value;
            }
        }
        public DateTime Time { get; set; }
    }   
}
