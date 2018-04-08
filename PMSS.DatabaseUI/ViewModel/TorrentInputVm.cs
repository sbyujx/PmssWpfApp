using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using System.ComponentModel;
using PMSS.SqlDataIn;
using MySql.Data.MySqlClient;

namespace PMSS.DatabaseUI.ViewModel
{
    public class TorrentInputVm : INotifyPropertyChanged
    {
        public DelegateCommand SaveCmd { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly string type = "山洪";

        public TorrentInputVm()
        {
            this.Time = DateTime.Now;
            this.SaveCmd = new DelegateCommand(SaveData);
        }

        private DateTime time;
        public DateTime Time
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

        private string area;
        public string Area
        {
            get
            {
                return this.area;
            }
            set
            {
                this.area = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(Area)));
                }
            }
        }

        private string proc;
        public string Proc
        {
            get
            {
                return this.proc;
            }
            set
            {
                this.proc = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(Proc)));
                }
            }
        }

        private string situation;
        public string Situation
        {
            get
            {
                return this.situation;
            }
            set
            {
                this.situation = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(Situation)));
                }
            }
        }

        private string comment;
        public string Comment
        {
            get
            {
                return this.comment;
            }
            set
            {
                this.comment = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(Comment)));
                }
            }
        }

        private string hint;
        public string Hint
        {
            get
            {
                return this.hint;
            }
            set
            {
                this.hint = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(Hint)));
                }
            }
        }

        private bool isWarning;
        public bool IsWarning
        {
            get
            {
                return this.isWarning;
            }
            set
            {
                this.isWarning = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsWarning)));
                }
            }
        }

        public void SaveData()
        {
            DisasterDataToDatabase dataIn = new DisasterDataToDatabase();
            int warning = 0;

            if (this.isWarning)
            {
                warning = 1;
            }
            else
            {
                warning = 0;
            }

            if (this.area == null || this.area.Trim().Equals(string.Empty))
            {
                this.Hint = "区域不能为空!";
                return;
            }

            try
            {
                if (this.Comment == null)
                {
                    this.Comment = "";
                }

                if (this.Situation == null)
                {
                    this.Situation = "";
                }

                if (this.Proc == null)
                {
                    this.Proc = "";
                }

                dataIn.AddOneRecord(this.time, this.type, warning, this.area, this.comment, this.situation, this.proc);
                this.Time = DateTime.Now;
                this.IsWarning = false;
                this.Area = "";
                this.Comment = "";
                this.Situation = "";
                this.Proc = "";
                this.Hint = "数据录入数据库成功!";
            }
            catch (MySqlException)
            {
                this.Hint = "数据库连接失败，数据录入不成功!";
            }
            catch (Exception ex)
            {
                this.Hint = ex.ToString();
            }
        }
    }
}
