﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMSS.Configure;

namespace PMSS.ConfigureSet
{
    public class NotesGroupOption
    {
        public NotesAddress Address { get; set; }
        public string ShowWord
        {
            get
            {
                return this.Address.NotesAlias + "(" + this.Address.NotesName + ")";
            }
        }

        public bool IsSelected { get; set; }

        public NotesGroupOption(NotesAddress addr)
        {
            this.Address = addr;
            this.IsSelected = false;
        }
    }
}
