using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace PMSS.Configure
{
    public class PreAndSignConfig
    {
        private readonly string fileName = "PreAndSign.json";
        private readonly string dir = "Config";
        private string fullPath = "";
        public PreAndSign Pas { get; set; }

        public PreAndSignConfig()
        {
            this.fullPath = Path.Combine(Directory.GetCurrentDirectory(), dir, fileName);
            this.Pas = new PreAndSign();
            ReadConfigFromFile();
        }

        public void ReadConfigFromFile()
        {
            EnsurePathAndFile();

            string json = File.ReadAllText(this.fullPath);
            this.Pas = JsonConvert.DeserializeObject<PreAndSign>(json);
            if (this.Pas == null)
            {
                this.Pas = new PreAndSign();
            }
        }

        public void WriteConfigToFile()
        {
            EnsurePathAndFile();

            string json = JsonConvert.SerializeObject(this.Pas);
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
