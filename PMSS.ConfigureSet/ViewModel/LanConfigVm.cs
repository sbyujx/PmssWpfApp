using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using PMSS.Configure;
using System.ComponentModel;

namespace PMSS.ConfigureSet.ViewModel
{
    public class LanConfigVm : INotifyPropertyChanged
    {
        public DelegateCommand SaveConfigCmd { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public LanConfigVm()
        {
            this.SaveConfigCmd = new DelegateCommand(SaveConfig);
            LanConfig config = new LanConfig();
            this.address = config.LanAddr;
        }

        private string address;
        public string Address
        {
            get
            {
                return this.address;
            }
            set
            {
                this.address = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(Address)));
                }
            }
        }

        private string configHint;
        public string ConfigHint
        {
            get
            {
                return this.configHint;
            }
            set
            {
                this.configHint = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(ConfigHint)));
                }
            }
        }

        public void SaveConfig()
        {
            if(this.address == null || this.address.Trim().Equals(string.Empty))
            {
                this.ConfigHint = "局域网地址不能为空！";
            }
            else
            {
                LanConfig config = new LanConfig();
                config.LanAddr = this.address;
                this.ConfigHint = "保存成功！";
            }
        }
    }
}
