using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSS.WordProduct
{
    public class PicBookmark
    {
        public string PicFileName { get; set; }
        public string Bookmark { get; set; }
        public List<TextBoxOnPic> ListTextBox { get; set; }
    }

    public class TextBoxOnPic
    {
        public float Left { get; set; }
        public float Top { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string Text { get; set; }
        public float Size { get; set; }
        public int Bold { get; set; }
        public string FontName { get; set; }
    }
}
