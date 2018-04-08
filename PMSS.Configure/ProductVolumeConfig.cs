using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSS.Configure
{
    public class ProductVolumeConfig
    {
        private readonly string pathConfigFileName = "ProductVolume.json";
        private readonly string dir = "Config";
        private string fullPath = "";
        public ProductVolume productVolume { get; set; }

        public ProductVolumeConfig()
        {
            this.fullPath = Path.Combine(Directory.GetCurrentDirectory(), dir, pathConfigFileName);
            this.productVolume = new ProductVolume();
            ReadConfigFromFile();
        }

        public void ReadConfigFromFile()
        {
            EnsurePathAndFile();

            string json = File.ReadAllText(this.fullPath);
            this.productVolume = JsonConvert.DeserializeObject<ProductVolume>(json);
            if (this.productVolume == null)
            {
                this.productVolume = new ProductVolume();
                this.productVolume.GeoDisVolume = 1;
                this.productVolume.TorrentVolume = 1;
                this.productVolume.WaterloggingVolume = 1;
            }
        }

        public void WriteConfigToFile()
        {
            EnsurePathAndFile();

            string json = JsonConvert.SerializeObject(this.productVolume);
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
