using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Pmss.Micaps.DataChecking
{
    public class BaseMetaDataGenerator
    {
        public BaseMetaDataGenerator(string folderPath, string pattern)
        {
            this.folderPath = folderPath;
            this.pattern = pattern;
        }
        public NavMetaData GenerateMetaData()
        {
            if (string.IsNullOrWhiteSpace(this.folderPath))
            {
                throw new ArgumentNullException("folderPath");
            }
            if (string.IsNullOrWhiteSpace(this.pattern))
            {
                throw new ArgumentNullException("pattern");
            }
            if (!Directory.Exists(this.folderPath))
            {
                throw new Exception($"文件夹不存在： {this.folderPath}");
            }

            var regex = new Regex(pattern);
            var files = Directory.EnumerateFiles(this.folderPath, "*.*", SearchOption.TopDirectoryOnly);

            var result = new NavMetaData
            {
                FolderPath = this.folderPath
            };
            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                if (regex.IsMatch(fileName))
                {
                    result.AllFileNames.Add(fileName);
                }
            }
            if (result.AllFileNames.Count <= 0)
            {
                throw new Exception($"文件夹中没有指定命名格式的文件: {this.folderPath}");
            }

            result.AllFileNames.Sort();
            result.CurrentIndex = result.AllFileNames.Count - 1;
            while (result.HasLastFile && HasSameName(result.AllFileNames[result.CurrentIndex - 1], result.AllFileNames[result.CurrentIndex]))
            {
                result.CurrentIndex--;
            }

            return result;
        }

        private bool HasSameName(string filename1, string filename2)
        {
            string tmp1 = filename1.Substring(0, filename1.LastIndexOf('.'));
            string tmp2 = filename2.Substring(0, filename2.LastIndexOf('.'));
            return tmp1 == tmp2;
        }
        private string folderPath;
        private string pattern;
    }
}
