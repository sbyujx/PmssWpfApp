using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMSS.Configure
{
    public class NotesConfig
    {
        public NotesConfig()
        {
            notesServer = Properties.Settings.Default.NotesServer;
            notesPwd = Properties.Settings.Default.NotesPwd;
            notesFile = Properties.Settings.Default.NotesFile;
        }

        private string notesServer;
        public string NotesServer
        {
            get
            {
                return notesServer;
            }
            set
            {
                notesServer = value;
                Properties.Settings.Default.NotesServer = value;
                Properties.Settings.Default.Save();
            }
        }

        private string notesPwd;
        public string NotesPwd
        {
            get
            {
                return notesPwd;
            }
            set
            {
                notesPwd = value;
                Properties.Settings.Default.NotesPwd = value;
                Properties.Settings.Default.Save();
            }
        }

        private string notesFile;
        public string NotesFile
        {
            get
            {
                return notesFile;
            }
            set
            {
                notesFile = value;
                Properties.Settings.Default.NotesFile = value;
                Properties.Settings.Default.Save();
            }
        }
    }
}
