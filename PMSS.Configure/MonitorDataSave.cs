using Newtonsoft.Json;
using PMSS.WordProduct;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace PMSS.Configure
{
    public class MonitorDataSave
    {
        private readonly string riverWarningInfoFileName = "RiverWarningInfo.json";
        private readonly string reservoirWarningInfoFileName = "ReservoirWarningInfo.json";
        private readonly string keyReservoirWarningInfoFileName = "KeyReservoirWarningInfo.json";
        private readonly string lakeWarningInfoFileName = "LakeWarningInfo.json";
        private readonly string dir = "PmssWpfApp/bin/Release/Config";
        public string fullPathRi = "";
        public string fullPathRe = "";
        public string fullPathKre = "";
        public string fullPathLake = "";
        public List<RiverWarningRecord> groupsRiver;
        public List<ReservoirWarningRecord> groupsReservoir;
        public IEnumerable<ReservoirWarningRecord> keyReservoir;
        public List<RiverWarningRecord> lakes;

        public MonitorDataSave()
        {
            DirectoryInfo info = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            string path = info.Parent.Parent.Parent.FullName;
            this.fullPathRi = Path.Combine(path, dir, riverWarningInfoFileName);
            this.fullPathRe = Path.Combine(path, dir, reservoirWarningInfoFileName);
            this.fullPathKre = Path.Combine(path, dir, keyReservoirWarningInfoFileName);
            this.fullPathLake = Path.Combine(path, dir, lakeWarningInfoFileName);
            //ReadDataFromFile();
        }

        public void EnsurePathAndFile()
        {
            FileInfo fileInfo = new FileInfo(this.fullPathRi);
            if (!fileInfo.Exists)
            {
                Directory.CreateDirectory(fileInfo.Directory.FullName);
            }

            if (!File.Exists(this.fullPathRi))
            {
                File.Create(this.fullPathRi).Close();
            }

            fileInfo = new FileInfo(this.fullPathKre);
            if (!fileInfo.Exists)
            {
                Directory.CreateDirectory(fileInfo.Directory.FullName);
            }

            if (!File.Exists(this.fullPathKre))
            {
                File.Create(this.fullPathKre).Close();
            }

            fileInfo = new FileInfo(this.fullPathRe);
            if (!fileInfo.Exists)
            {
                Directory.CreateDirectory(fileInfo.Directory.FullName);
            }

            if (!File.Exists(this.fullPathRe))
            {
                File.Create(this.fullPathRe).Close();
            }

            fileInfo = new FileInfo(this.fullPathLake);
            if (!fileInfo.Exists)
            {
                Directory.CreateDirectory(fileInfo.Directory.FullName);
            }

            if (!File.Exists(this.fullPathLake))
            {
                File.Create(this.fullPathLake).Close();
            }
        }

        public void ReadDataFromFile()
        {
            EnsurePathAndFile();

            try
            {
                string json = File.ReadAllText(this.fullPathRi);
                this.groupsRiver = JsonConvert.DeserializeObject<List<RiverWarningRecord>>(json);
                json = File.ReadAllText(this.fullPathRe);
                this.groupsReservoir = JsonConvert.DeserializeObject<List<ReservoirWarningRecord>>(json);
                json = File.ReadAllText(this.fullPathKre);
                this.keyReservoir = JsonConvert.DeserializeObject<IEnumerable<ReservoirWarningRecord>>(json);
                json = File.ReadAllText(this.fullPathLake);
                this.lakes = JsonConvert.DeserializeObject<List<RiverWarningRecord>>(json);
            }
            catch(Exception ex)
            {
                ;
            }
        }

        public void WriteDataToFile()
        {
            EnsurePathAndFile();

            string json = JsonConvert.SerializeObject(this.groupsRiver);
            File.WriteAllText(this.fullPathRi, json);
            json = JsonConvert.SerializeObject(this.groupsReservoir);
            File.WriteAllText(this.fullPathRe, json);
            json = JsonConvert.SerializeObject(this.keyReservoir);
            File.WriteAllText(this.fullPathKre, json);
            json = JsonConvert.SerializeObject(this.lakes);
            File.WriteAllText(this.fullPathLake, json);
        }

    }
}
