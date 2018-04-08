using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace PMSS.Configure
{
    public class LanAddressConfig
    {
        private readonly string lanConfigFileName = "LanAddress.json";
        private readonly string dir = "Config";
        private string fullPath = "";
        public List<LanAddress> ListLanAddr { get; set; }

        public LanAddressConfig()
        {
            this.fullPath = Path.Combine(Directory.GetCurrentDirectory(), dir, lanConfigFileName);
            this.ListLanAddr = new List<LanAddress>();
            ReadConfigFromFile();
        }

        public void ReadConfigFromFile()
        {
            EnsurePathAndFile();

            string json = File.ReadAllText(this.fullPath);
            this.ListLanAddr = JsonConvert.DeserializeObject<List<LanAddress>>(json);
            if(this.ListLanAddr == null)
            {
                this.ListLanAddr = new List<LanAddress>();
            }
        }

        public void WriteConfigToFile()
        {
            EnsurePathAndFile();

            string json = JsonConvert.SerializeObject(this.ListLanAddr);
            File.WriteAllText(this.fullPath, json);
        }

        public void EnsurePathAndFile()
        {
            FileInfo fileInfo = new FileInfo(this.fullPath);
            if (!fileInfo.Exists)
            {
                Directory.CreateDirectory(fileInfo.Directory.FullName);
            }

            if (!File.Exists(this.fullPath))
            {
                File.Create(this.fullPath).Close();
            }
        }
    }
}
