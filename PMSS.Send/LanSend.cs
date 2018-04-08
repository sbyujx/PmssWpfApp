using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace PMSS.Send
{
    public class LanSend
    {
        private bool isSetUserName = false;
        private bool isSetNewFileName = false;

        public LanSend(string localFile, string remotePath)
        {
            localFilePath = localFile;
            sharePath = remotePath;
        }

        private string sharePath;
        public string SharePath
        {
            get
            {
                return sharePath;
            }
            set
            {
                sharePath = value;
            }
        }

        private string userName;
        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                isSetUserName = true;
                userName = value;
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

        private string newFileName;
        public string NewFileName
        {
            get
            {
                return newFileName;
            }
            set
            {
                isSetNewFileName = true;
                newFileName = value;
            }
        }

        private string localFilePath;
        public string LocalFilePath
        {
            get
            {
                return localFilePath;
            }
            set
            {
                localFilePath = value;
            }
        }

        public bool SendFile()
        {
            bool flag = false;
            Process proc = new Process();
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                string dosLine = "";
                if (!isSetUserName)
                {
                    dosLine = @"net use " + sharePath + " /PERSISTENT:YES";
                }
                else
                {
                    dosLine = @"net use " + sharePath + " /User:" + userName + " " + pwd + " /PERSISTENT:YES";
                }
                proc.StandardInput.WriteLine(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (!proc.HasExited)
                {
                    proc.WaitForExit(100);
                }
                string errormsg = proc.StandardError.ReadToEnd();
                proc.StandardError.Close();
                if (string.IsNullOrEmpty(errormsg) || errormsg.Contains("1219"))
                {
                    flag = true;
                    string target = "";
                    if (isSetNewFileName)
                    {
                        target = sharePath + "/" + newFileName;
                    }
                    else
                    {
                        target = sharePath + "/" + Path.GetFileName(localFilePath);
                    }
                    File.Copy(localFilePath, target, true);
                }
                else
                {
                    throw new Exception(errormsg);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                flag = false;
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }

            return flag;
        }
    }
}
