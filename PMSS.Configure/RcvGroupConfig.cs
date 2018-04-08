using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace PMSS.Configure
{
    public class RcvGroupConfig
    {
        private readonly string groupConfigFileName = "RcvGroup.json";
        private readonly string dir = "Config";
        private string fullPath = "";
        public List<RcvGroup> ListGroup { get; set; }

        public RcvGroupConfig()
        {
            this.fullPath = Path.Combine(Directory.GetCurrentDirectory(), dir, groupConfigFileName);
            this.ListGroup = new List<RcvGroup>();
            ReadConfigFromFile();
        }

        public void ReadConfigFromFile()
        {
            EnsurePathAndFile();

            string json = File.ReadAllText(this.fullPath);
            this.ListGroup = JsonConvert.DeserializeObject<List<RcvGroup>>(json);
            if (this.ListGroup == null)
            {
                this.ListGroup = new List<RcvGroup>();
            }
        }

        public void WriteConfigToFile()
        {
            EnsurePathAndFile();

            string json = JsonConvert.SerializeObject(this.ListGroup);
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
