using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMSS.SqlDataOutput
{
    public class OutPutSelectOption
    {
        public const string FILE_TYPE_TXT = ".TXT";
        public const string FILE_TYPE_EXCEL = ".XLXS";

        public OutPutSelectOption()
        {
            isSetTimeRange = false;
            isSetStationRange = false;
            from = new DateTime();
            to = new DateTime();
            stationIdList = new List<string>();
            fileType = FILE_TYPE_TXT;
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

        private bool isSetStationRange;
        public bool IsSetStationRange
        {
            get
            {
                return isSetStationRange;
            }
            set
            {
                isSetStationRange = value;
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

        private List<string> stationIdList;
        public List<string> StationIdList
        {
            get
            {
                return stationIdList;
            }
            set
            {
                stationIdList = value;
            }
        }

        public void AddStationId(string id)
        {
            stationIdList.Add(id);
        }
    }
}
