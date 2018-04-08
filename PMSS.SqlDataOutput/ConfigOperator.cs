using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace PMSS.SqlDataOutput
{
    class ConfigOperator
    {
        private string xmlPath;
        private XmlDocument myDc;
        private string connSqlStr;

        public ConfigOperator(string Path)
        {
            try
            {
                myDc = new XmlDocument();
                if (!File.Exists(Path))
                {
                    throw new Exception("配置文件不存在， 路径： " + Path);
                }
                else
                {
                    xmlPath = Path;
                }

                myDc.Load(xmlPath);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string ConnSqlStr
        {
            get
            {
                connSqlStr = myDc.SelectSingleNode("system").SelectSingleNode("ConnSqlStr").InnerText.Trim();
                return connSqlStr;
            }
        }
    }
}
