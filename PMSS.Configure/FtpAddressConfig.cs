using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace PMSS.Configure
{
    public class FtpAddressConfig
    {
        private readonly string emailConfigFileName = "FtpAddress.json";
        private readonly string dir = "Config";
        private string fullPath = "";
        public List<FtpAddress> ListFtpAddr { get; set; }

        public FtpAddressConfig()
        {
            this.fullPath = Path.Combine(Directory.GetCurrentDirectory(), dir, emailConfigFileName);
            this.ListFtpAddr = new List<FtpAddress>();
            ReadConfigFromFile();
        }

        public void ReadConfigFromFile()
        {
            EnsurePathAndFile();

            string json = File.ReadAllText(this.fullPath);
            this.ListFtpAddr = JsonConvert.DeserializeObject<List<FtpAddress>>(json);
            if(this.ListFtpAddr == null)
            {
                this.ListFtpAddr = new List<FtpAddress>();
            }
        }

        public void WriteConfigToFile()
        {
            EnsurePathAndFile();

            string json = JsonConvert.SerializeObject(this.ListFtpAddr);
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
