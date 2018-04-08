using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using System.ComponentModel;
using PMSS.Configure;

namespace PMSS.ConfigureSet.ViewModel
{
    public class PreSignVm : INotifyPropertyChanged
    {
        public DelegateCommand SaveConfigCmd { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
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

        private string preName;
        public string PreName
        {
            get
            {
                return this.preName;
            }
            set
            {
                this.preName = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(PreName)));
                }
            }
        }

        private string signName;
        public string SignName
        {
            get
            {
                return this.signName;
            }
            set
            {
                this.signName = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(SignName)));
                }
            }
        }

        public PreSignVm()
        {
            this.SaveConfigCmd = new DelegateCommand(SaveConfig);
            PreAndSignConfig config = new PreAndSignConfig();
            this.PreName = config.Pas.PreName;
            this.SignName = config.Pas.SignName;
        }

        public void SaveConfig()
        {
            if (this.preName == null || this.preName.Trim().Equals(string.Empty) || this.signName == null || this.signName.Trim().Equals(string.Empty))
            {
                this.ConfigHint = "预报人和签发人不能为空!";
            }
            else
            {
                PreAndSignConfig config = new PreAndSignConfig();
                config.Pas.PreName = this.preName.Trim();
                config.Pas.SignName = this.signName.Trim();
                config.WriteConfigToFile();
                this.ConfigHint = "配置保存成功!";
            }
        }
    }
}
