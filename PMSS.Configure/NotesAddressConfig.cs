using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace PMSS.Configure
{
    public class NotesAddressConfig
    {
        private readonly string notesConfigFileName = "NotesAddress.json";
        private readonly string dir = "Config";
        private string fullPath = "";
        public List<NotesAddress> ListNotesAddr { get; set; }

        public NotesAddressConfig()
        {
            this.fullPath = Path.Combine(Directory.GetCurrentDirectory(), dir, notesConfigFileName);
            this.ListNotesAddr = new List<NotesAddress>();
            ReadConfigFromFile();
        }

        public void ReadConfigFromFile()
        {
            EnsurePathAndFile();

            string json = File.ReadAllText(this.fullPath);
            this.ListNotesAddr = JsonConvert.DeserializeObject<List<NotesAddress>>(json);
            if(this.ListNotesAddr == null)
            {
                this.ListNotesAddr = new List<NotesAddress>();
            }
        }

        public void WriteConfigToFile()
        {
            EnsurePathAndFile();

            string json = JsonConvert.SerializeObject(this.ListNotesAddr);
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
