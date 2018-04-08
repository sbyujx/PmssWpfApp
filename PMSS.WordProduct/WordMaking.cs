using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word = Microsoft.Office.Interop.Word;
using Core = Microsoft.Office.Core;

namespace PMSS.WordProduct
{
    public class WordMaking
    {
        public static void MakingViaConfig(WordConfig config)
        {
            Word.Range range;
            object saveWithDocument = true;
            object missing = Type.Missing;
            Word.Application app = new Word.Application();
            app.Visible = true;
            Word.Document doc = app.Documents.Open(config.TemplateFileName, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing);

            foreach (PicBookmark item in config.ListPicBookmark)  //图片书签
            {
                object bookmark = item.Bookmark;
                range = doc.Bookmarks.get_Item(ref bookmark).Range;
                Word.InlineShape pic = doc.InlineShapes.AddPicture(item.PicFileName, ref missing, ref saveWithDocument, range);
                float sc = 380 / pic.Width;
                pic.Width = 380;
                pic.Height = pic.Height * sc;

                if (item.ListTextBox != null)
                {
                    foreach (TextBoxOnPic tb in item.ListTextBox)
                    {
                        Word.Shape text = doc.Shapes.AddTextbox(Core.MsoTextOrientation.msoTextOrientationHorizontal, tb.Left, tb.Top, tb.Width, tb.Height);
                        text.Line.Visible = Core.MsoTriState.msoFalse;  //无边框
                        text.TextFrame.ContainingRange.Text = tb.Text;
                        text.TextFrame.TextRange.Font.Size = tb.Size;
                        text.TextFrame.TextRange.Font.Bold = tb.Bold;
                        text.TextFrame.TextRange.Font.Name = tb.FontName;
                        text.TextFrame.TextRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                        text.ZOrder(Core.MsoZOrderCmd.msoBringToFront);
                    }
                }
            }

            foreach (TextBookmark item in config.ListTextBookmark)  //文字书签
            {
                object bookmark = item.Bookmark;
                range = doc.Bookmarks.get_Item(ref bookmark).Range;
                range.Text = item.Text;
            }

            doc.SaveAs2(config.OutFileName, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing);
            //app.Quit();
        }

        public static void MakingHydroMonitorReport(HydroMonitorReportData data)
        {
            List<Word.WdColor> backgroundColorList = new List<Word.WdColor>();
            backgroundColorList.Add(Word.WdColor.wdColorSeaGreen);
            backgroundColorList.Add(Word.WdColor.wdColorLightOrange);
            backgroundColorList.Add(Word.WdColor.wdColorLightGreen);
            backgroundColorList.Add(Word.WdColor.wdColorLightYellow);

            string fileOutName = data.OutPath + "/全国主要江河水情监测报告" + DateTime.Now.ToString("yyyyMMdd");
            Word.Range range;
            object saveWithDocument = true;
            object missing = Type.Missing;
            object oMissing = System.Reflection.Missing.Value;
            Word.Application app = new Word.Application();
            app.Visible = true;
            Word.Document doc = app.Documents.Open(data.TemplateFullPath, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing);

            object bookmark = "日期年";
            range = doc.Bookmarks.get_Item(ref bookmark).Range;
            range.Text = data.ReportTime.Year.ToString("D4");

            bookmark = "日期月";
            range = doc.Bookmarks.get_Item(ref bookmark).Range;
            range.Text = data.ReportTime.Month.ToString("D2");

            bookmark = "日期日";
            range = doc.Bookmarks.get_Item(ref bookmark).Range;
            range.Text = data.ReportTime.Day.ToString("D2");

            bookmark = "日期时";
            range = doc.Bookmarks.get_Item(ref bookmark).Range;
            range.Text = data.ReportTime.Hour.ToString("D2");

            bookmark = "制作人";
            range = doc.Bookmarks.get_Item(ref bookmark).Range;
            range.Text = data.ReportPerson;

            bookmark = "水情截至月";
            range = doc.Bookmarks.get_Item(ref bookmark).Range;
            range.Text = data.ReportTime.Month.ToString("D2");

            bookmark = "水情截至日";
            range = doc.Bookmarks.get_Item(ref bookmark).Range;
            range.Text = data.ReportTime.Day.ToString("D2");

            bookmark = "水情截至时";
            range = doc.Bookmarks.get_Item(ref bookmark).Range;
            range.Text = data.ReportTime.Hour.ToString("D2");

            bookmark = "水库月";
            range = doc.Bookmarks.get_Item(ref bookmark).Range;
            range.Text = data.ReportTime.Month.ToString("D2");

            bookmark = "水库日";
            range = doc.Bookmarks.get_Item(ref bookmark).Range;
            range.Text = data.ReportTime.Day.ToString("D2");

            bookmark = "水库时";
            range = doc.Bookmarks.get_Item(ref bookmark).Range;
            range.Text = data.ReportTime.Hour.ToString("D2");

            Word.Table tb = doc.Tables[2]; //主要水文站实时超警水情表
            int row = 2;
            int color = 0;
            var Basin1 = "";
            foreach (var item in data.RiverWarningGroup)
            {
                if(item.Basin == null)
                {
                    item.Basin = "";
                }

                if (!item.Basin.Equals(Basin1))
                {
                    color++;
                }

                tb.Rows.Add(oMissing);
                if (item.IsBeyond == true)  //设置该行是否超警戒需要加粗
                {
                    tb.Rows[row].Range.Bold = 1;
                }
                else
                {
                    tb.Rows[row].Range.Bold = 0;
                }
                tb.Rows[row].Range.Shading.BackgroundPatternColor = backgroundColorList[color % backgroundColorList.Count];  //设置一行的颜色
                tb.Rows[row].Cells[1].Range.Text = item.Basin;
                tb.Rows[row].Cells[2].Range.Text = item.Province;
                tb.Rows[row].Cells[3].Range.Text = item.River;
                tb.Rows[row].Cells[4].Range.Text = item.Name;
                tb.Rows[row].Cells[5].Range.Text = item.Time.ToString("MM-dd HH:MM");
                string comp = "";
                Word.WdColor compColor = Word.WdColor.wdColorBlack;
                if (item.ComparedBefore == Trend.Rise)
                {
                    comp = "↑";
                    compColor = Word.WdColor.wdColorRed;
                }
                else if (item.ComparedBefore == Trend.Fall)
                {
                    comp = "↓";
                    compColor = Word.WdColor.wdColorGreen;
                }
                else
                {
                    comp = "—";
                }
                range = tb.Rows[row].Cells[6].Range;
                range.Text = item.Level.ToString("N2") + comp; //水位
                Word.Range colorRange = app.ActiveDocument.Range(range.End - 2, range.End - 1);
                colorRange.Font.Color = compColor;
                tb.Rows[row].Cells[7].Range.Text = item.WarningLevel.ToString("N2");
                tb.Rows[row].Cells[8].Range.Text = item.OverWarningLevel.ToString("N2");
                tb.Rows[row].Cells[8].Range.Font.Color = Word.WdColor.wdColorRed;
                row++;
                
                Basin1 = item.Basin;
            }

            tb = doc.Tables[3]; //全国重点关注水库站实时水情监测表
            row = 2;

            foreach (var item in data.KeyReservoirWarning)
            {
                tb.Rows.Add(oMissing);
                tb.Rows[row].Range.Bold = 0;
                tb.Rows[row].Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorWhite;  //设置一行的颜色
                tb.Rows[row].Cells[1].Range.Text = item.Name;
                tb.Rows[row].Cells[3].Range.Text = item.Time.ToString("MM-dd HH:MM");
                string comp = "";
                Word.WdColor compColor = Word.WdColor.wdColorBlack;
                if (item.ComparedBefore == Trend.Rise)
                {
                    comp = "↑";
                    compColor = Word.WdColor.wdColorRed;
                }
                else if (item.ComparedBefore == Trend.Fall)
                {
                    comp = "↓";
                    compColor = Word.WdColor.wdColorGreen;
                }
                else
                {
                    comp = "—";
                }
                range = tb.Rows[row].Cells[2].Range;
                range.Text = item.Level.ToString("N2") + comp; //水位
                Word.Range colorRange = app.ActiveDocument.Range(range.End - 2, range.End - 1);
                colorRange.Font.Color = compColor;
                tb.Rows[row].Cells[4].Range.Text = item.WarningLevel.ToString("N2");
                tb.Rows[row].Cells[5].Range.Text = item.OverWarningLevel.ToString("N2");
                tb.Rows[row].Cells[5].Range.Font.Color = Word.WdColor.wdColorRed;
                row++;
            }

            tb = doc.Tables[4]; //全国重点关注主要湖泊实时水情监测表
            row = 2;

            foreach (var item in data.LakeWarning)
            {
                tb.Rows.Add(oMissing);
                tb.Rows[row].Range.Bold = 0;
                tb.Rows[row].Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorWhite;  //设置一行的颜色
                tb.Rows[row].Cells[1].Range.Text = item.Name;
                tb.Rows[row].Cells[3].Range.Text = item.Time.ToString("MM-dd HH:MM");
                string comp = "";
                Word.WdColor compColor = Word.WdColor.wdColorBlack;
                if (item.ComparedBefore == Trend.Rise)
                {
                    comp = "↑";
                    compColor = Word.WdColor.wdColorRed;
                }
                else if (item.ComparedBefore == Trend.Fall)
                {
                    comp = "↓";
                    compColor = Word.WdColor.wdColorGreen;
                }
                else
                {
                    comp = "—";
                }
                range = tb.Rows[row].Cells[2].Range;
                range.Text = item.Level.ToString("N2") + comp; //水位
                Word.Range colorRange = app.ActiveDocument.Range(range.End - 2, range.End - 1);
                colorRange.Font.Color = compColor;
                tb.Rows[row].Cells[4].Range.Text = item.WarningLevel.ToString("N2");
                tb.Rows[row].Cells[5].Range.Text = item.OverWarningLevel.ToString("N2");
                tb.Rows[row].Cells[5].Range.Font.Color = Word.WdColor.wdColorRed;
                row++;
            }


            tb = doc.Tables[5]; //主要水库实时超汛限水情表
            row = 2;
            color = 0;
            var Basin2 = "";
            foreach (var item in data.ReservoirWarningGroup)
            {
                if (item.Basin == null)
                {
                    item.Basin = "";
                }

                if (!item.Basin.Equals(Basin2))
                {
                    color++;
                }

                tb.Rows.Add(oMissing);
                if (item.IsBeyond == true)  //设置该行是否超警戒需要加粗
                {
                    tb.Rows[row].Range.Bold = 1;
                }
                else
                {
                    tb.Rows[row].Range.Bold = 0;
                }
                tb.Rows[row].Range.Shading.BackgroundPatternColor = backgroundColorList[color % backgroundColorList.Count];  //设置一行的颜色
                tb.Rows[row].Cells[1].Range.Text = item.Basin;
                tb.Rows[row].Cells[2].Range.Text = item.Province;
                tb.Rows[row].Cells[3].Range.Text = item.River;
                tb.Rows[row].Cells[4].Range.Text = item.Name;
                tb.Rows[row].Cells[8].Range.Text = item.Time.ToString("MM-dd HH:MM");
                string comp = "";
                Word.WdColor compColor = Word.WdColor.wdColorBlack;
                if (item.ComparedBefore == Trend.Rise)
                {
                    comp = "↑";
                    compColor = Word.WdColor.wdColorRed;
                }
                else if (item.ComparedBefore == Trend.Fall)
                {
                    comp = "↓";
                    compColor = Word.WdColor.wdColorGreen;
                }
                else
                {
                    comp = "—";
                }
                range = tb.Rows[row].Cells[9].Range;
                range.Text = item.Level.ToString("N2") + comp; //水位
                Word.Range colorRange = app.ActiveDocument.Range(range.End - 2, range.End - 1);
                colorRange.Font.Color = compColor;
                tb.Rows[row].Cells[6].Range.Text = item.WarningLevel.ToString("N2");
                tb.Rows[row].Cells[7].Range.Text = item.OverWarningLevel.ToString("N2");
                tb.Rows[row].Cells[7].Range.Font.Color = Word.WdColor.wdColorRed;
                row++;
                                
                Basin2 = item.Basin;
            }

            doc.SaveAs2(fileOutName, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing);
        }
    }
}
