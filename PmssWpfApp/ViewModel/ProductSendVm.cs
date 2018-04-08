using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using PMSS.ConfigureSet;
using PMSS.ProductSend;
using System.Windows;

namespace PmssWpfApp.ViewModel
{
    class ProductSendVm
    {
        public DelegateCommand EmailSendCmd { get; set; }
        public DelegateCommand NotesSendCmd { get; set; }
        public DelegateCommand FtpSendCmd { get; set; }
        public DelegateCommand LanSendCmd { get; set; }
        public DelegateCommand EmailConfigCmd { get; set; }
        public DelegateCommand NotesConfigCmd { get; set; }
        public DelegateCommand EmailContactCmd { get; set; }
        public DelegateCommand NotesContactCmd { get; set; }
        public DelegateCommand FtpContactCmd { get; set; }
        public DelegateCommand LanContactCmd { get; set; }
        public DelegateCommand GroupCmd { get; set; }
        public DelegateCommand OneKeyCmd { get; set; }
        public DelegateCommand DisGeoPubCmd { get; set; }
        public DelegateCommand TorrentPubCmd { get; set; }
        public DelegateCommand WaterlogginPubCmd { get; set; }

        public ProductSendVm()
        {
            this.EmailSendCmd = new DelegateCommand(EmailSend);
            this.NotesSendCmd = new DelegateCommand(NotesSend);
            this.FtpSendCmd = new DelegateCommand(FtpSend);
            this.LanSendCmd = new DelegateCommand(LanSend);
            this.EmailConfigCmd = new DelegateCommand(EmailConfig);
            this.NotesConfigCmd = new DelegateCommand(NotesConfig);
            this.EmailContactCmd = new DelegateCommand(EmailContact);
            this.NotesContactCmd = new DelegateCommand(NotesContact);
            this.FtpContactCmd = new DelegateCommand(FtpContact);
            this.LanContactCmd = new DelegateCommand(LanContact);
            this.GroupCmd = new DelegateCommand(GroupConfig);
            this.OneKeyCmd = new DelegateCommand(OneKeySend);
            this.DisGeoPubCmd = new DelegateCommand(DisGeoPub);
            this.TorrentPubCmd = new DelegateCommand(TorrentPub);
            this.WaterlogginPubCmd = new DelegateCommand(WaterlogginPub);
        }

        public void DisGeoPub()
        {
            try
            {
                WindowDisGeoPub wd = new WindowDisGeoPub();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void TorrentPub()
        {
            try
            {
                WindowTorrentPub wd = new WindowTorrentPub();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void WaterlogginPub()
        {
            try
            {
                WindowWaterLoggingPub wd = new WindowWaterLoggingPub();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void OneKeySend()
        {
            try
            {
                WindowOneKeySend wd = new WindowOneKeySend();
                wd.ShowDialog();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void GroupConfig()
        {
            try
            {
                WindowGroupConfig wd = new WindowGroupConfig();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void EmailSend()
        {
            try
            {
                WindowSendEmail wd = new WindowSendEmail();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void NotesSend()
        {
            try
            {
                WindowSendNotes wd = new WindowSendNotes();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void FtpSend()
        {
            try
            {
                WindowSendFtp wd = new WindowSendFtp();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LanSend()
        {
            try
            {
                WindowSendLan wd = new WindowSendLan();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void EmailConfig()
        {
            try
            {
                WindowEmailConfig wd = new WindowEmailConfig();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void NotesConfig()
        {
            try
            {
                WindowNotesConfig wd = new WindowNotesConfig();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void EmailContact()
        {
            try
            {
                WindowEmailRcverConfig wd = new WindowEmailRcverConfig();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void NotesContact()
        {
            try
            {
                WindowNotesRcverConfig wd = new WindowNotesRcverConfig();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void FtpContact()
        {
            try
            {
                WindowFtpRcverConfig wd = new WindowFtpRcverConfig();
                wd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LanContact()
        {
            try
            {
                WindowLanRcverConfig wd = new WindowLanRcverConfig();
                wd.ShowDialog();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
