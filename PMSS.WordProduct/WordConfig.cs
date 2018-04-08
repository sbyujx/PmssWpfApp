using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSS.WordProduct
{
    public class WordConfig
    {
        public string TemplateFileName { get; set; }  //模板文件
        public string OutFileName { get; set; }  //输出文件
        public List<PicBookmark> ListPicBookmark { get; set; }
        public List<TextBookmark> ListTextBookmark { get; set; } 

        public WordConfig(string tempName, string outName)
        {
            this.TemplateFileName = tempName;
            this.OutFileName = outName;
            this.ListPicBookmark = new List<PicBookmark>();
            this.ListTextBookmark = new List<TextBookmark>();
        }

    }
}
