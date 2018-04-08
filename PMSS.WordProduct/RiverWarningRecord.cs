using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSS.WordProduct
{
    public enum Trend { Rise, Fall, Hold};

    public class RiverWarningRecord
    {
        public string Stationid { get; set; }  //站点号
        public string Name { get; set; }   //站名
        public string Basin { get; set; }   //流域
        public string Province { get; set; }  //省区
        public string River { get; set; }   //河流名
        public DateTime Time { get; set; }  //时间
        public double Level { get; set; }  //水位（河道站）
        public double WarningLevel { get; set; }  //警戒水位（河道站）
        public double OverWarningLevel { get; set; }  //超警戒水位
        public bool IsBeyond { get; set; }  //是否已经超过警戒水位
        public Trend ComparedBefore { get; set; }  //水位与之前比较的趋势
    }
}
