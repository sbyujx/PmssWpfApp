using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace PMSS.Configure
{
    public class EmailAddressConfig
    {
        private readonly string emailConfigFileName = "EmailAddress.json";
        private readonly string dir = "Config";
        private string fullPath = "";
        public List<EmailAddress> ListEmailAddr { get; set; }

        public EmailAddressConfig()
        {
            this.fullPath = Path.Combine(Directory.GetCurrentDirectory(), dir, emailConfigFileName);
            this.ListEmailAddr = new List<EmailAddress>();
            ReadConfigFromFile();
        }

        public void ReadConfigFromFile()
        {
            EnsurePathAndFile();

            string json = File.ReadAllText(this.fullPath);
            this.ListEmailAddr = JsonConvert.DeserializeObject<List<EmailAddress>>(json);
            if(this.ListEmailAddr == null)
            {
                this.ListEmailAddr = new List<EmailAddress>();
            }
        }

        public void WriteConfigToFile()
        {
            EnsurePathAndFile();

            string json = JsonConvert.SerializeObject(this.ListEmailAddr);
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
