using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domino;
using PMSS.Configure;

namespace PMSS.Send
{
    public class NotesSend
    {
        public NotesSend()
        {
            ReadConfig();
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

        private List<string> mailTo;
        public List<string> MailTo
        {
            get
            {
                return mailTo;
            }
            set
            {
                mailTo = value;
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

        private string subject;
        public string Subject
        {
            get
            {
                return subject;
            }
            set
            {
                subject = value;
            }
        }

        private string body;
        public string Body
        {
            get
            {
                return body;
            }
            set
            {
                body = value;
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

        public void SendMail()
        {
            try
            {
                NotesSession ns = new NotesSessionClass();

                ns.Initialize(pwd);
                NotesDatabase ndb = ns.GetDatabase(serverName, nsfFile, false);

                NotesDocument doc = ndb.CreateDocument();
                doc.ReplaceItemValue("Form", "Memo");
                string[] sendtoString = mailTo.ToArray();
                doc.ReplaceItemValue("SendTo", sendtoString);
                doc.ReplaceItemValue("Subject", subject);
                NotesRichTextItem rt = doc.CreateRichTextItem("Body");
                rt.AppendText(body);

                if ((attFileName != null) && (attFileName.Count > 0))
                {
                    NotesRichTextItem item = doc.CreateRichTextItem("attachment");
                    foreach (string att in attFileName)
                    {
                        item.EmbedObject(EMBED_TYPE.EMBED_ATTACHMENT, "", att, "attachment");
                    }

                }

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
