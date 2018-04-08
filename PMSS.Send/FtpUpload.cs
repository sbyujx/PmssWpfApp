using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace PMSS.Send
{
    public class FtpUpload
    {
        public FtpUpload(string dir, string file, string newName)
        {
            ftpDir = dir;
            fileLocalPath = file;
            newFileName = newName;
        }

        private string newFileName;
        public string NewFileName
        {
            get
            {
                return newFileName;
            }
            set
            {
                newFileName = value;
            }
        }

        private string user;
        public string User
        {
            get
            {
                return user;
            }
            set
            {
                user = value;
            }
        }

        private string pwd;
        public string Pwd
        {
            get
            {
                return pwd;
            }
            set
            {
                pwd = value;
            }
        }

        private string ftpDir;
        public string FtpDir
        {
            get
            {
                return ftpDir;
            }
            set
            {
                ftpDir = value;
            }
        }

        private string fileLocalPath;
        public string FileLocalPath
        {
            get
            {
                return fileLocalPath;
            }
            set
            {
                fileLocalPath = value;
            }
        }


        public void Upload()
        {
            string comb = ftpDir + "/" + newFileName;
            Uri uri = new Uri(comb);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            if (user != null && !user.Trim().Equals(string.Empty))
            {
                request.Credentials = new NetworkCredential(user, pwd);
            }
            Stream requestStream = request.GetRequestStream();
            using (FileStream fileStream = File.Open(fileLocalPath, FileMode.Open))
            {
                byte[] buffer = new byte[fileStream.Length];
                int bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                requestStream.Write(buffer, 0, bytesRead);
            }

            requestStream.Close();
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        }
    }
}
