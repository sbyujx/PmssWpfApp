using PMSS.SqlDataOutput;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSS.DatabaseUI.ViewModel
{
    public class MapTriggerVm : INotifyPropertyChanged
    {
        public DelegateCommand DataShowCmd { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public MapTriggerVm()
        {
            this.DataShowCmd = new DelegateCommand(DataShow);

            this.TimeFrom = DateTime.Now.AddDays(-7);
            this.TimeTo = DateTime.Now;
        }

        private string stationId;
        public string StationId
        {
            get
            {
                return this.stationId;
            }
            set
            {
                this.stationId = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(StationId)));
                }
            }
        }

        private string stationName;
        public string StationName
        {
            get
            {
                return this.stationName;
            }
            set
            {
                this.stationName = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(StationName)));
                }
            }
        }

        private string stationType;
        public string StationType
        {
            get
            {
                return this.stationType;
            }
            set
            {
                this.stationType = value;

                if (this.stationType.Equals("河道站"))
                {
                    this.LName = "水位（米）：";
                    this.QName = "流量（立方米/秒）：";
                    this.WLName = "警戒水位（米）：";
                }

                if (this.stationType.Equals("水库站"))
                {
                    this.LName = "库水位（米）：";
                    this.QName = "蓄水量（亿立方米）：";
                    this.WLName = "汛限水位（米）：";
                }

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(StationType)));
                }
            }
        }

        private string lName;
        public string LName
        {
            get
            {
                return this.lName;
            }
            set
            {
                this.lName = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(LName)));
                }
            }
        }

        private string l;
        public string L
        {
            get
            {
                return this.l;
            }
            set
            {
                this.l = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(L)));
                }
            }
        }

        private string qName;
        public string QName
        {
            get
            {
                return this.qName;
            }
            set
            {
                this.qName = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(QName)));
                }
            }
        }

        private string q;
        public string Q
        {
            get
            {
                return this.q;
            }
            set
            {
                this.q = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(Q)));
                }
            }
        }

        private string wLName;
        public string WLName
        {
            get
            {
                return this.wLName;
            }
            set
            {
                this.wLName = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(WLName)));
                }
            }
        }

        private string wL;
        public string WL
        {
            get
            {
                return this.wL;
            }
            set
            {
                this.wL = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(WL)));
                }
            }
        }

        private string time;
        public string Time
        {
            get
            {
                return this.time;
            }
            set
            {
                this.time = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(Time)));
                }
            }
        }

        private DateTime timeFrom;
        public DateTime TimeFrom
        {
            get
            {
                return this.timeFrom;
            }
            set
            {
                this.timeFrom = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(TimeFrom)));
                }
            }
        }

        private DateTime timeTo;
        public DateTime TimeTo
        {
            get
            {
                return this.timeTo;
            }
            set
            {
                this.timeTo = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(TimeTo)));
                }
            }
        }

        private void ShowRiverData(OutPutSelectOption option)
        {
            FetchRiverRecord frr = new FetchRiverRecord();

            if (option.IsSetTimeRange == true)
            {
                frr.SetDuration(option.From, option.To);
            }

            if (option.IsSetStationRange == true)
            {
                frr.SetStationId(option.StationIdList);
            }

            frr.FecthData();

            frr.RiverList.Sort(delegate (RiverHydrologyRecord x, RiverHydrologyRecord y)
                {
                    return x.Time.CompareTo(y.Time);
                });
            WindowShowRiverChart wd = new WindowShowRiverChart(frr.RiverList);
            wd.ShowDialog();
        }

        private void ShowReservoirData(OutPutSelectOption option)
        {
            FetchReservoirRecord frr = new FetchReservoirRecord();

            if (option.IsSetTimeRange == true)
            {
                frr.SetDuration(option.From, option.To);
            }

            if (option.IsSetStationRange == true)
            {
                frr.SetStationId(option.StationIdList);
            }

            frr.FecthData();
            frr.ReservoirList.Sort(delegate (ReservoirHydrologyRecord x, ReservoirHydrologyRecord y)
            {
                return x.Time.CompareTo(y.Time);
            });

            WindowShowReserviorChart wd = new WindowShowReserviorChart(frr.ReservoirList);
            wd.ShowDialog();
        }

        public void DataShow()
        {
            OutPutSelectOption option = new OutPutSelectOption();
            option.IsSetStationRange = true;
            if (this.StationId != null)
            {
                option.AddStationId(this.StationId);
            }

            if (option.StationIdList.Count == 0)
            {
                return;
            }

            option.IsSetTimeRange = true;
            option.From = this.timeFrom;
            option.To = this.timeTo;

            if (this.StationType.Equals("河道站"))
            {
                ShowRiverData(option);
            }

            if(this.StationType.Equals("水库站"))
            {
                ShowReservoirData(option);
            }
        }
    }
}
