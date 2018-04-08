using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMSS.SqlDataOutput
{
    //河道水文记录
    public class RiverHydrologyRecord
    {
        private int recordId;
        public int RecordId
        {
            get
            {
                return recordId;
            }
            set
            {
                recordId = value;
            }
        }

        private string stationId;
        public string StationId
        {
            get
            {
                return stationId;
            }
            set
            {
                stationId = value;
            }
        }

        private string stationName;
        public string StationName
        {
            get
            {
                return stationName;
            }
            set
            {
                stationName = value;
            }
        }

        private string stationLocation;
        public string StationLocation
        {
            get
            {
                return stationLocation;
            }
            set
            {
                stationLocation = value;
            }
        }

        private string riverName;
        public string RiverName
        {
            get
            {
                return riverName;
            }
            set
            {
                riverName = value;
            }
        }

        private double level;
        public double Level
        {
            get
            {
                return level;
            }
            set
            {
                level = value;
            }
        }

        private double flow;
        public double Flow
        {
            get
            {
                return flow;
            }
            set
            {
                flow = value;
            }
        }

        private DateTime time;
        public DateTime Time
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
            }
        }

        private double warningLevel;
        public double WarningLevel
        {
            get
            {
                return warningLevel;
            }
            set
            {
                warningLevel = value;
            }
        }

        private double safetyLevel;
        public double SafetyLevel
        {
            get
            {
                return safetyLevel;
            }
            set
            {
                safetyLevel = value;
            }
        }

        private double longitude;
        public double Longitude
        {
            get
            {
                return longitude;
            }
            set
            {
                longitude = value;
            }
        }

        private double latitude;
        public double Latitude
        {
            get
            {
                return latitude;
            }
            set
            {
                latitude = value;
            }
        }
    }
}
