using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using System.ComponentModel;
using System.Collections.ObjectModel;
using PMSS.Configure;
using PMSS.ConfigureSet;
using System.IO;
using Microsoft.Win32;
using PMSS.Send;
using System.Threading;

namespace PMSS.ProductSend.ViewModel
{
    public class FilePathItem
    {
        public string FullPath { get; set; }
        public string FileName { get; set; }
    }
    public class OneKeySendVm : INotifyPropertyChanged
    {
        public DelegateCommand AddEmailCmd { get; set; }
        public DelegateCommand AddNotesCmd { get; set; }
        public DelegateCommand AddFtpCmd { get; set; }
        public DelegateCommand AddLanCmd { get; set; }
        public DelegateCommand TodayProductCmd { get; set; }
        public DelegateCommand AllProductCmd { get; set; }
        public DelegateCommand AddOtherProductCmd { get; set; }
        public DelegateCommand OneKeySendCmd { get; set; }
        public DelegateCommand GroupChangeCmd { get; set; }
        public DelegateCommand DirChangeCmd { get; set; }
        public DelegateCommand FileInCmd { get; set; }
        public DelegateCommand FileOutCmd { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly BackgroundWorker worker = new BackgroundWorker();

        public OneKeySendVm()
        {
            this.AddEmailCmd = new DelegateCommand(AddEmail);
            this.AddNotesCmd = new DelegateCommand(AddNotes);
            this.AddFtpCmd = new DelegateCommand(AddFtp);
            this.AddLanCmd = new DelegateCommand(AddLan);
            this.TodayProductCmd = new DelegateCommand(ShowTodayProduct);
            this.AllProductCmd = new DelegateCommand(ShowAllProduct);
            this.AddOtherProductCmd = new DelegateCommand(AddOtherProduct);
            this.OneKeySendCmd = new DelegateCommand(OneKey);
            this.GroupChangeCmd = new DelegateCommand(GroupChange);
            this.DirChangeCmd = new DelegateCommand(DirChange);
            this.FileInCmd = new DelegateCommand(FileIn);
            this.FileOutCmd = new DelegateCommand(FileOut);

            InitGroups();
            InitDirs();

            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            OneKeySend();
        }

        private void worker_RunWorkerCompleted(object sender,
                                      RunWorkerCompletedEventArgs e)
        {
            this.Hint = "全部发送完成！";
        }

        private string selectSendItem;
        public string SelectSendItem
        {
            get
            {
                return this.selectSendItem;
            }
            set
            {
                this.selectSendItem = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectSendItem)));
                }
            }

        }


        private ObservableCollection<string> sendList;
        public ObservableCollection<string> SendList
        {
            get
            {
                return this.sendList;
            }
            set
            {
                this.sendList = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(SendList)));
                }
            }
        }

        private FilePathItem selectFileListItem;
        public FilePathItem SelectFileListItem
        {
            get
            {
                return this.selectFileListItem;
            }
            set
            {
                this.selectFileListItem = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectFileListItem)));
                }
            }

        }

        private ObservableCollection<FilePathItem> fileList;
        public ObservableCollection<FilePathItem> FileList
        {
            get
            {
                return this.fileList;
            }
            set
            {
                this.fileList = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(FileList)));
                }
            }
        }

        private ObservableCollection<ProductPath> dirList;
        public ObservableCollection<ProductPath> DirList
        {
            get
            {
                return this.dirList;
            }
            set
            {
                this.dirList = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(DirList)));
                }
            }
        }

        private ProductPath selectDirItem;
        public ProductPath SelectDirItem
        {
            get
            {
                return this.selectDirItem;
            }
            set
            {
                this.selectDirItem = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectDirItem)));
                }
            }
        }

        private string emailTempRcver;
        public string EmailTempRcver
        {
            get
            {
                return this.emailTempRcver;
            }
            set
            {
                this.emailTempRcver = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(EmailTempRcver)));
                }
            }
        }

        private ObservableCollection<EmailGroupOption> emailOptions;
        public ObservableCollection<EmailGroupOption> EmailOptions
        {
            get
            {
                return this.emailOptions;
            }
            set
            {
                this.emailOptions = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(EmailOptions)));
                }
            }
        }

        private string notesTempRcver;
        public string NotesTempRcver
        {
            get
            {
                return this.notesTempRcver;
            }
            set
            {
                this.notesTempRcver = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(NotesTempRcver)));
                }
            }
        }

        private ObservableCollection<NotesGroupOption> notesOptions;
        public ObservableCollection<NotesGroupOption> NotesOptions
        {
            get
            {
                return this.notesOptions;
            }
            set
            {
                this.notesOptions = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(NotesOptions)));
                }
            }
        }

        private string ftpTempPath;
        public string FtpTempPath
        {
            get
            {
                return this.ftpTempPath;
            }
            set
            {
                this.ftpTempPath = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(FtpTempPath)));
                }
            }
        }

        private string ftpUserName;
        public string FtpUserName
        {
            get
            {
                return this.ftpUserName;
            }
            set
            {
                this.ftpUserName = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(FtpUserName)));
                }
            }
        }

        private string ftpPwd;
        public string FtpPwd
        {
            get
            {
                return this.ftpPwd;
            }
            set
            {
                this.ftpPwd = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(FtpPwd)));
                }
            }
        }

        private ObservableCollection<FtpGroupOption> ftpOptions;
        public ObservableCollection<FtpGroupOption> FtpOptions
        {
            get
            {
                return this.ftpOptions;
            }
            set
            {
                this.ftpOptions = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(FtpOptions)));
                }
            }
        }

        private string lanTempPath;
        public string LanTempPath
        {
            get
            {
                return this.lanTempPath;
            }
            set
            {
                this.lanTempPath = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(LanTempPath)));
                }
            }
        }

        private string lanUserName;
        public string LanUserName
        {
            get
            {
                return this.lanUserName;
            }
            set
            {
                this.lanUserName = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(LanUserName)));
                }
            }
        }

        private string lanPwd;
        public string LanPwd
        {
            get
            {
                return this.lanPwd;
            }
            set
            {
                this.lanPwd = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(LanPwd)));
                }
            }
        }

        private ObservableCollection<LanGroupOption> lanOptions;
        public ObservableCollection<LanGroupOption> LanOptions
        {
            get
            {
                return this.lanOptions;
            }
            set
            {
                this.lanOptions = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(LanOptions)));
                }
            }
        }

        private RcvGroup selectGroupItem;
        public RcvGroup SelectGroupItem
        {
            get 
            {
                return this.selectGroupItem;
            }
            set
            {
                this.selectGroupItem = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectGroupItem)));
                }
            }
        }

        private ObservableCollection<RcvGroup> groups;
        public ObservableCollection<RcvGroup> Groups
        {
            get
            {
                return this.groups;
            }
            set
            {
                this.groups = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(Groups)));
                }
            }
        }

        private string hint;
        public string Hint
        {
            get
            {
                return this.hint;
            }
            set
            {
                this.hint = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(Hint)));
                }
            }
        }

        public void InitGroups()
        {
            RcvGroupConfig config = new RcvGroupConfig();
            this.Groups = new ObservableCollection<RcvGroup>();
            foreach(var item in config.ListGroup)
            {
                this.Groups.Add(item);
            }
        }

        public void InitDirs()
        {
            ProductPathConfig config = new ProductPathConfig();
            this.DirList = new ObservableCollection<ProductPath>();
            foreach(var item in config.ListProductPath)
            {
                this.DirList.Add(item);
            }
        }

        public void GroupChange()
        {
            this.EmailOptions = new ObservableCollection<EmailGroupOption>();
            foreach(var item in this.SelectGroupItem.ListEmail)
            {
                EmailGroupOption op = new EmailGroupOption(item);
                op.IsSelected = true;
                this.EmailOptions.Add(op);
            }

            this.NotesOptions = new ObservableCollection<NotesGroupOption>();
            foreach (var item in this.SelectGroupItem.ListNotes)
            {
                NotesGroupOption op = new NotesGroupOption(item);
                op.IsSelected = true;
                this.NotesOptions.Add(op);
            }

            this.FtpOptions = new ObservableCollection<FtpGroupOption>();
            foreach(var item in this.SelectGroupItem.ListFtp)
            {
                FtpGroupOption op = new FtpGroupOption(item);
                op.IsSelected = true;
                this.FtpOptions.Add(op);
            }

            this.LanOptions = new ObservableCollection<LanGroupOption>();
            foreach(var item in this.SelectGroupItem.ListLan)
            {
                LanGroupOption op = new LanGroupOption(item);
                op.IsSelected = true;
                this.LanOptions.Add(op);
            }
           
        }

        public void ShowTodayProduct()
        {
            if (this.selectDirItem != null)
            {
                this.FileList = new ObservableCollection<FilePathItem>();
                DirectoryInfo folder = new DirectoryInfo(this.selectDirItem.Path);
                foreach (FileInfo info in folder.GetFiles())
                {
                    if (info.LastWriteTime.Date.Equals(DateTime.Now.Date))
                    {
                        FilePathItem fp = new FilePathItem();
                        fp.FullPath = info.FullName;
                        fp.FileName = info.Name;
                        this.FileList.Add(fp);
                    }
                }
            }
        }

        public void ShowAllProduct()
        {
            if (this.selectDirItem != null)
            {
                DirChange();
            }
        }

        public void AddOtherProduct()
        {
            OpenFileDialog op = new OpenFileDialog();
            if (op.ShowDialog() == true)
            {
                if(this.SendList == null)
                {
                    this.SendList = new ObservableCollection<string>();
                }
                this.SendList.Add(op.FileName);
            }
        }

        public void OneKey()
        {
            worker.RunWorkerAsync();
        }

        public void OneKeySend()
        {
            if (this.SendList == null || this.SendList.Count == 0)
            {
                this.Hint = "请至少选择一个分发产品!";
                return;
            }

            if (this.EmailOptions != null && this.EmailOptions.Count > 0)
            {
                this.Hint = "Email发送中...";
                EmailSend email = new EmailSend();
                email.EmailTo = new List<string>();
                foreach (var item in this.EmailOptions)
                {
                    if (item.IsSelected == true)
                    {
                        email.EmailTo.Add(item.Address.EmailName);
                    }
                }
                email.Subject = "";
                email.AttFileName = new List<string>();
                foreach (var item in this.SendList)
                {
                    email.AttFileName.Add(item);
                    FileInfo fi = new FileInfo(item);
                    if (fi.Extension.Equals(".doc") || fi.Extension.Equals(".docx"))
                    {
                        string name = Path.GetFileNameWithoutExtension(item);
                        email.Subject = name;
                        break;
                    }
                    email.Subject += new FileInfo(item).Name + " | ";
                }

                if (email.EmailTo.Count > 0)
                {
                    email.Send();
                }
                this.Hint = "Email发送完成！";
            }

            if (this.NotesOptions != null && this.NotesOptions.Count > 0)
            {
                this.Hint = "Notes发送中...";
                NotesSend notes = new NotesSend();
                notes.MailTo = new List<string>();
                foreach (var item in this.NotesOptions)
                {
                    if (item.IsSelected == true)
                    {
                        notes.MailTo.Add(item.Address.NotesName);
                    }
                }
                notes.Subject = "";
                notes.AttFileName = new List<string>();
                foreach (var item in this.SendList)
                {
                    notes.AttFileName.Add(item);
                    FileInfo fi = new FileInfo(item);
                    if (fi.Extension.Equals(".doc") || fi.Extension.Equals(".docx"))
                    {
                        string name = Path.GetFileNameWithoutExtension(item);
                        notes.Subject = name;
                        break;
                    }
                    notes.Subject += new FileInfo(item).Name + " | ";
                }

                if (notes.MailTo.Count > 0)
                {
                    notes.SendMail();
                }
                this.Hint = "Notes发送完成！";
            }

            if (this.ftpOptions != null && this.ftpOptions.Count > 0)
            {
                this.Hint = "FTP上传中...";
                foreach (var op in this.ftpOptions)
                {
                    if (op.IsSelected == true)
                    {
                        foreach (var file in this.SendList)
                        {
                            string newName = Path.GetFileName(new FileInfo(file).Name);

                            FtpUpload ftpUpload = new FtpUpload(op.Address.FtpPath, file, newName);
                            if (op.Address.FtpUserName != null && (!op.Address.FtpUserName.Trim().Equals(string.Empty)))
                            {
                                ftpUpload.User = op.Address.FtpUserName.Trim();
                            }

                            if (op.Address.FtpPwd != null && (!op.Address.FtpPwd.Trim().Equals(string.Empty)))
                            {
                                ftpUpload.Pwd = op.Address.FtpPwd.Trim();
                            }

                            try
                            {
                                ftpUpload.Upload();
                            }
                            catch (Exception)
                            {
                                this.Hint = "FTP文件上传失败，请检查FTP设置!";
                            }
                        }
                    }
                }

                this.Hint = "FTP上传完成！";
            }

            if (this.lanOptions != null && this.lanOptions.Count > 0)
            {
                this.Hint = "局域网上传中...";
                foreach (var op in this.lanOptions)
                {
                    if (op.IsSelected == true)
                    {
                        foreach (var file in this.SendList)
                        {
                            LanSend lanSend = new LanSend(file.Trim(), op.Address.LanPath.Trim());

                            if (op.Address.LanName != null && (!op.Address.LanName.Trim().Equals(string.Empty)))
                            {
                                lanSend.UserName = op.Address.LanName.Trim();
                            }

                            if (op.Address.LanPwd != null && (!op.Address.LanPwd.Trim().Equals(string.Empty)))
                            {
                                lanSend.Pwd = op.Address.LanPwd.Trim();
                            }

                            if (lanSend.SendFile() != true)
                            {
                                this.Hint = file +　"上传失败!";
                            }
                        }
                    }
                }

                this.Hint = "局域网上传完成！";
                this.Hint = "一键分发完成！";
            }
        }
        public void AddEmail()
        {
            if (this.emailTempRcver == null || this.emailTempRcver.Trim().Equals(string.Empty))
            {
                this.Hint = "临时收件人不能为空！";
                return;
            }
            if (this.EmailOptions == null)
            {
                this.EmailOptions = new ObservableCollection<EmailGroupOption>();
            }

            EmailAddress addr = new EmailAddress();
            addr.EmailName = this.emailTempRcver.Trim();
            EmailGroupOption op = new EmailGroupOption(addr);
            op.IsSelected = true;
            this.EmailOptions.Add(op);
            this.EmailTempRcver = "";
        }

        public void AddNotes()
        {
            if(this.notesTempRcver == null || this.notesTempRcver.Trim().Equals(string.Empty))
            {
                this.Hint = "临时收件人不能为空！";
                return;
            }
            if(this.NotesOptions == null)
            {
                this.NotesOptions = new ObservableCollection<NotesGroupOption>();
            }

            NotesAddress addr = new NotesAddress();
            addr.NotesName = this.notesTempRcver.Trim();
            NotesGroupOption op = new NotesGroupOption(addr);
            op.IsSelected = true;
            this.NotesOptions.Add(op);
            this.NotesTempRcver = "";
        }

        public void AddFtp()
        {
            if (this.ftpTempPath == null || this.ftpTempPath.Trim().Equals(string.Empty))
            {
                this.Hint = "临时路径不能为空！";
                return;
            }
            if (this.FtpOptions == null)
            {
                this.FtpOptions = new ObservableCollection<FtpGroupOption>();
            }

            FtpAddress addr = new FtpAddress();
            addr.FtpPath = this.ftpTempPath.Trim();
            addr.FtpUserName = this.ftpUserName;
            addr.FtpPwd = this.ftpPwd;
            FtpGroupOption op = new FtpGroupOption(addr);
            op.IsSelected = true;
            this.FtpOptions.Add(op);
            this.FtpTempPath = "";
            this.FtpUserName = "";
            this.FtpPwd = "";
        }

        public void AddLan()
        {
            if(this.lanTempPath == null || this.lanTempPath.Trim().Equals(string.Empty))
            {
                this.Hint = "临时路径不能为空！";
                return;
            }

            if(this.LanOptions == null)
            {
                this.LanOptions = new ObservableCollection<LanGroupOption>();
            }

            LanAddress addr = new LanAddress();
            addr.LanPath = this.lanTempPath.Trim();
            addr.LanName = this.lanUserName;
            addr.LanPwd = this.lanPwd;
            LanGroupOption op = new LanGroupOption(addr);
            op.IsSelected = true;
            this.LanOptions.Add(op);
            this.LanTempPath = "";
            this.LanUserName = "";
            this.LanPwd = "";
        }

        public void DirChange()
        {
            this.FileList = new ObservableCollection<FilePathItem>();
            DirectoryInfo folder = new DirectoryInfo(this.selectDirItem.Path);
            foreach(FileInfo info in folder.GetFiles())
            {
                FilePathItem fp = new FilePathItem();
                fp.FullPath = info.FullName;
                fp.FileName = info.Name;
                this.FileList.Add(fp);
            }
        }

        public void FileIn()
        {
            if(this.SendList == null)
            {
                this.SendList = new ObservableCollection<string>();
            }
            this.SendList.Add(this.selectFileListItem.FullPath);
        }

        public void FileOut()
        {
            this.SendList.Remove(this.selectSendItem);
        }
    }
}
