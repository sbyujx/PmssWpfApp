using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSS.WordProduct
{
    public class HydroMonitorReportData
    {
        public DateTime ReportTime { get; set; }
        public string ReportPerson { get; set; }
        public List<RiverWarningRecord> RiverWarningGroup { get; set; }
        public List<ReservoirWarningRecord> ReservoirWarningGroup { get; set; }
        public IEnumerable<ReservoirWarningRecord> KeyReservoirWarning { get; set; }
        public List<RiverWarningRecord> LakeWarning { get; set; }
        public string TemplateFullPath { get; set; }
        public string OutPath { get; set; }
    }
}
