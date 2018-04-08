using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMSS.CrawlerService
{
    public class RiverHydrology
    {
        private string m_stationName;
        public string StationName
        {
            get
            {
                return m_stationName;
            }
            set
            {
                m_stationName = value;
            }
        }

        private string m_stationAddress;
        public string StationAddress
        {
            get
            {
                return m_stationAddress;
            }
            set
            {
                m_stationAddress = value;
            }
        }

        private string m_riverName;
        public string RiverName
        {
            get
            {
                return m_riverName;
            }
            set
            {
                m_riverName = value;
            }
        }

        private double m_waterLevel;
        public double WaterLevel
        {
            get
            {
                return m_waterLevel;
            }
            set
            {
                m_waterLevel = value;
            }
        }

        private double m_flow;
        public double Flow
        {
            get
            {
                return m_flow;
            }
            set
            {
                m_flow = value;
            }
        }

        private DateTime m_time;
        public DateTime Time
        {
            get
            {
                return m_time;
            }
            set
            {
                m_time = value;
            }
        }

        private double m_warningWaterLevel;
        public double WarningWaterLevel
        {
            get
            {
                return m_warningWaterLevel;
            }
            set
            {
                m_warningWaterLevel = value;
            }
        }

        public string Basin { get; set; }

        public string AdministrativeRegion { get; set; }
    }
}
