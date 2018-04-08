using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSS.SqlDataOutput
{
    public class OutputDisasterOption
    {

        public const string FILE_TYPE_EXCEL = ".XLXS";

        public OutputDisasterOption(string tp)
        {
            isSetTimeRange = false;
            from = new DateTime();
            to = new DateTime();
            this.Area = "";
            fileType = FILE_TYPE_EXCEL;
            this.Type = tp;
        }
        private string fileType;
        public string FileType
        {
            get
            {
                return fileType;
            }
            set
            {
                fileType = value;
            }
        }

        private bool isSetTimeRange;
        public bool IsSetTimeRange
        {
            get
            {
                return isSetTimeRange;
            }
            set
            {
                isSetTimeRange = value;
            }
        }

        private DateTime from;
        public DateTime From
        {
            get
            {
                return from;
            }
            set
            {
                from = value;
            }
        }

        private DateTime to;
        public DateTime To
        {
            get
            {
                return to;
            }
            set
            {
                to = value;
            }
        }

        public string Area { get; set; }
        public string Type { get; set; }

    }
}
