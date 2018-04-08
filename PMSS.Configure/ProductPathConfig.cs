using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace PMSS.Configure
{
    public class ProductPathConfig
    {
        private readonly string pathConfigFileName = "ProductPath.json";
        private readonly string dir = "Config";
        private string fullPath = "";
        public List<ProductPath> ListProductPath { get; set; }

        public ProductPathConfig()
        {
            this.fullPath = Path.Combine(Directory.GetCurrentDirectory(), dir, pathConfigFileName);
            this.ListProductPath = new List<ProductPath>();
            ReadConfigFromFile();
        }

        public void ReadConfigFromFile()
        {
            EnsurePathAndFile();

            string json = File.ReadAllText(this.fullPath);
            this.ListProductPath = JsonConvert.DeserializeObject<List<ProductPath>>(json);
            if (this.ListProductPath == null)
            {
                this.ListProductPath = new List<ProductPath>();
            }
        }

        public void WriteConfigToFile()
        {
            EnsurePathAndFile();

            string json = JsonConvert.SerializeObject(this.ListProductPath);
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
