using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domino;
using PMSS.Configure;

namespace PMSS.Send
{
    public class NotesPub
    {
        private string dateNum;
        private string datePy;
        public NotesPub()
        {
            ReadConfig();
            dateNum = DateTime.Now.ToString("yyyy-MM-dd");
            datePy = DateTime.Now.ToString("yyyy年MM月dd日");
        }

        private string serverName;
        public string ServerName
        {
            get
            {
                return serverName;
            }
            set
            {
                serverName = value;
            }
        }

        private string nsfFile;
        public string NsfFile
        {
            get
            {
                return nsfFile;
            }
            set
            {
                nsfFile = value;
            }
        }

        private string pwd;
        public string Pwd
        {
            set
            {
                pwd = value;
            }
        }

        private string title;
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
            }
        }

        private List<string> attFileName;
        public List<string> AttFileName
        {
            get
            {
                return attFileName;
            }
            set
            {
                attFileName = value;
            }
        }

        public void ReadConfig()
        {
            NotesConfig config = new NotesConfig();
            serverName = config.NotesServer;
            nsfFile = config.NotesFile;
            pwd = config.NotesPwd;
        }

        public void Pub()
        {
            try
            {
                NotesSession ns = new NotesSessionClass();
                ns.Initialize(pwd);
                NotesDatabase ndb = ns.GetDatabase(serverName, nsfFile, false);

                NotesDocument doc = ndb.CreateDocument();

                doc.ReplaceItemValue("Form", "中央气象台产品记录");
               
                doc.ReplaceItemValue("dbname_11", "juece.nsf");
                doc.ReplaceItemValue("dbname_12", "juece.nsf");
                doc.ReplaceItemValue("dbname_13", "juece.nsf");
                doc.ReplaceItemValue("dbname_21", "juece.nsf");
                doc.ReplaceItemValue("dbname_22", "juece.nsf");
                doc.ReplaceItemValue("dbname_222", "juece.nsf");
                doc.ReplaceItemValue("dbname_23", "juece.nsf");
                doc.ReplaceItemValue("dbname_24", "juece.nsf");
                doc.ReplaceItemValue("dbname_30", "juece.nsf");
                doc.ReplaceItemValue("doctype", "OCX");
                doc.ReplaceItemValue("Encrypt", "0");
                
                doc.ReplaceItemValue("img", "");
                doc.ReplaceItemValue("issuetime", this.dateNum);
                doc.ReplaceItemValue("SendTo", "CMA决策");
                doc.ReplaceItemValue("SendTobyHub", "各省气象信息共享数据库");
                doc.ReplaceItemValue("sendOk", "0");
                doc.ReplaceItemValue("role", "周军");
                doc.ReplaceItemValue("role_1", "短期科");

                doc.ReplaceItemValue("标题域", this.Title);
                doc.ReplaceItemValue("标题域_1", this.Title);
                doc.ReplaceItemValue("部委", "0");
                doc.ReplaceItemValue("草稿", "0");
                doc.ReplaceItemValue("产品", "天气预报");
                doc.ReplaceItemValue("产品_1", "天气预报");
                doc.ReplaceItemValue("发布单位", "国家气象中心");
                doc.ReplaceItemValue("发布单位_1", "中央气象台");
                doc.ReplaceItemValue("发布单位_1_1", "中央气象台");
                doc.ReplaceItemValue("发布单位_2", "国家气象中心");
                doc.ReplaceItemValue("发布单位_e", "qxt");
                doc.ReplaceItemValue("发布时间", this.dateNum);
                doc.ReplaceItemValue("发布时间_1", this.datePy);
                doc.ReplaceItemValue("发布时间_2", this.dateNum);
                //doc.ReplaceItemValue("内容", this.Content);
                doc.ReplaceItemValue("日期1", this.dateNum);
                doc.ReplaceItemValue("状态", "1");

                if ((attFileName != null) && (attFileName.Count > 0))
                {
                    NotesRichTextItem item = doc.CreateRichTextItem("attachment");
                    foreach (string att in attFileName)
                    {
                        item.EmbedObject(EMBED_TYPE.EMBED_ATTACHMENT, "", att, "attachment");
                        doc.ReplaceItemValue("fname", System.IO.Path.GetFileName(att));
                    }
                }

                doc.ComputeWithForm(true, false);
                object obj = doc.GetItemValue("SendTo");
                doc.Send(false, ref obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
