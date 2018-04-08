using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using PMSS.Configure;

namespace PMSS.Send
{
    public class EmailSend
    {
        private string emailFrom;
        public string EmailFrom 
        {
            get
            {
                return emailFrom;
            }
            set
            {
                emailFrom = value;
            }
        }

        private List<string> emailTo;
        public List<string> EmailTo 
        {
            get
            {
                return emailTo;
            }
            set
            {
                emailTo = value;
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
            EmailConfig config = new EmailConfig();
            emailFrom = config.EmailName;
            pwd = config.EmailPwd;
        }

        public void Send()
        {
            ReadConfig();
            string[] sArrary = emailFrom.Split('@');
            SmtpClient client = new SmtpClient();
            if (sArrary.Length == 2)
            {
                if (sArrary[1].Trim().Equals("cma.gov.cn"))
                {
                    client.Host = "rays." + sArrary[1];
                }
                else
                {
                    client.Host = "smtp." + sArrary[1];
                }

                client.UseDefaultCredentials = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Credentials = new NetworkCredential(sArrary[0], pwd);
            }
            else
            {
                throw new SmtpException();
            }

            MailMessage message = new MailMessage();
            message.From = new MailAddress(emailFrom);
            foreach (string to in emailTo)
            {
                message.To.Add(to);
            }
            message.Subject = subject;
            message.Body = body;
            if (attFileName != null)
            {
                foreach (string att in attFileName)
                {
                    message.Attachments.Add(new Attachment(att));
                }
            }

            client.Send(message);
        }

        public void SendTestMail()
        {
            ReadConfig();
            string[] sArrary = emailFrom.Split('@');
            SmtpClient client = new SmtpClient();
            if (sArrary.Length == 2)
            {
                if(sArrary[1].Trim().Equals("cma.gov.cn"))
                {
                    client.Host = "rays." + sArrary[1];
                }
                else
                {
                    client.Host = "smtp." + sArrary[1];
                }
                
                client.UseDefaultCredentials = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Credentials = new NetworkCredential(sArrary[0], pwd);
            }
            else
            {
                throw new SmtpException();
            }


            MailMessage message = new MailMessage();
            message.From = new MailAddress(emailFrom);
            message.To.Add(emailFrom);
            message.Subject = "test邮件";
            message.Body = "这是一封测试邮件，收到此邮件证明Email配置正确!";

            client.Send(message);
        }
    }
}
