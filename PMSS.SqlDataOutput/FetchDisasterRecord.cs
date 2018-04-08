using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using System.Linq.Expressions;

namespace PMSS.SqlDataOutput
{
    public class FetchDisasterRecord
    {
        private Configuration myConfigration;
        private ISessionFactory mySessionFactory;
        private ISession mySession;
        private List<Geologicaldisaster> listDisaster;

        public FetchDisasterRecord()
        {
            listDisaster = new List<Geologicaldisaster>();
            myConfigration = new Configuration();
            myConfigration.Configure();
            try
            {
                mySessionFactory = myConfigration.BuildSessionFactory();
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            mySession = mySessionFactory.OpenSession();
        }

        public List<Geologicaldisaster> ListDisaster
        {
            get
            {
                return listDisaster;
            }
        }

        public void GetDisaterRecord(String type, String area, DateTime timeFrom, DateTime timeTo)
        {
            listDisaster = new List<Geologicaldisaster>();
            var datas = mySession.Query<Geologicaldisaster>();

           var query = from d in datas
                        where d.Type == type
                        where d.Time > timeFrom && d.Time < timeTo
                        where d.Area.Contains(area)
                        select d;
                        

            foreach (Geologicaldisaster record in query)
            {
                listDisaster.Add(record);
            }
        }

        public void SaveToExcelFile(string fileName)
        {
            Excel.Application oXL;
            Excel._Workbook oWB;
            Excel._Worksheet oSheet; ;
            Excel.Range oRng;

            try
            {
                //Start Excel and get Application object.
                oXL = new Excel.Application();
                oXL.Visible = true;

                //Get a new workbook.
                oWB = (Excel._Workbook)(oXL.Workbooks.Add(Missing.Value));
                oSheet = (Excel._Worksheet)oWB.ActiveSheet;

                //Add table headers going cell by cell.
                oSheet.Cells[1, 1] = "记录号";
                oSheet.Cells[1, 2] = "时间";
                oSheet.Cells[1, 3] = "区域";
                oSheet.Cells[1, 4] = "过程";
                oSheet.Cells[1, 5] = "灾情";
                oSheet.Cells[1, 6] = "预警区内";
                oSheet.Cells[1, 7] = "备注";

                //Format A1:D1 as bold, vertical alignment = center.
                oSheet.get_Range("A1", "G1").Font.Bold = true;
                oSheet.get_Range("A1", "G1").VerticalAlignment =
                    Excel.XlVAlign.xlVAlignCenter;

                int row = 2;

                foreach (Geologicaldisaster gd in listDisaster)
                {
                    oSheet.Cells[row, 1] = gd.Recordid.ToString();
                    oSheet.Cells[row, 2] = gd.Time.ToString();
                    oSheet.Cells[row, 3] = gd.Area;
                    oSheet.Cells[row, 4] = gd.Process;
                    oSheet.Cells[row, 5] = gd.Disastersituation;
                    if(gd.Inwarningarea == 0)
                    {
                        oSheet.Cells[row, 6] = "否";
                    }
                    else
                    {
                        oSheet.Cells[row, 6] = "是";
                    }
                    oSheet.Cells[row, 7] = gd.Comment;
                    row++;
                }

                oRng = oSheet.Range["A:A", Type.Missing];
                oRng.EntireColumn.ColumnWidth = 5;
                oRng = oSheet.Range["B:B", Type.Missing];
                oRng.EntireColumn.ColumnWidth = 16;
                oRng = oSheet.Range["C:C", Type.Missing];
                oRng.EntireColumn.ColumnWidth = 25;
                oRng = oSheet.Range["D:D", Type.Missing];
                oRng.EntireColumn.ColumnWidth = 50;
                oRng = oSheet.Range["E:E", Type.Missing];
                oRng.EntireColumn.ColumnWidth = 50;
                oRng = oSheet.Range["F:F", Type.Missing];
                oRng.EntireColumn.ColumnWidth = 5;
                oRng = oSheet.Range["G:G", Type.Missing];
                oRng.EntireColumn.ColumnWidth = 16;


                //oRng = oSheet.Range[oSheet.Cells[1, 1], oSheet.Cells[row, 7]];
                //oRng.EntireColumn.AutoFit();  //列宽自适应

                oWB.SaveAs(fileName, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                //oXL.Quit();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
