using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMSS.SqlDataOutput
{
    //水库水文记录
    public class ReservoirHydrologyRecord
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

        private double pondage;
        public double Pondage
        {
            get
            {
                return pondage;
            }
            set
            {
                pondage = value;
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
