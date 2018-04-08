using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMSS.Configure;
using Prism.Commands;
using Prism.Events;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;

namespace PMSS.ConfigureSet.ViewModel
{
    public class ProductPathConfigVm : INotifyPropertyChanged
    {
        public Window wd;
        public DelegateCommand AddCmd { get; set; }
        public DelegateCommand ModifyCmd { get; set; }
        public DelegateCommand DeleteCmd { get; set; }
        public DelegateCommand OpenCmd { get; set; }
        public DelegateCommand<object> ChangeCmd { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private ProductPathConfig config;

        public ProductPathConfigVm()
        {
            this.AddCmd = new DelegateCommand(Add);
            this.ModifyCmd = new DelegateCommand(Modify);
            this.DeleteCmd = new DelegateCommand(Delete);
            this.OpenCmd = new DelegateCommand(Open);
            this.ChangeCmd = new DelegateCommand<object>(Change);
            config = new ProductPathConfig();
            this.DirList = new ObservableCollection<ProductPath>(config.ListProductPath);
        }

        private string pathName;
        public string PathName
        {
            get
            {
                return this.pathName;
            }
            set
            {
                this.pathName = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(PathName)));
                }
            }
        }

        private string pathDir;
        public string PathDir
        {
            get
            {
                return this.pathDir;
            }
            set
            {
                this.pathDir = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(PathDir)));
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

        private ProductPath selectItem;
        public ProductPath SelectItem
        {
            get
            {
                return this.selectItem;
            }
            set
            {
                this.selectItem = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectItem)));
                }
            }
        }

        public void Add()
        {
            if (this.pathName == null || this.pathName.Trim().Equals(string.Empty)
                || this.pathDir == null || this.pathDir.Trim().Equals(string.Empty))
            {
                this.Hint = "目录名、目录路径不能为空！";
            }
            else
            {
                ProductPath item = new ProductPath();
                item.Name = this.pathName.Trim();
                item.Path = this.pathDir.Trim();
                this.DirList.Add(item);
                config.ListProductPath = this.DirList.ToList();
                config.WriteConfigToFile();
                this.Hint = "常用产品目录保存成功!";
                this.PathName = "";
                this.PathDir = "";
            }
        }

        public void Modify()
        {
            if (this.selectItem == null)
            {
                this.Hint = "请先选择一个产品目录!";
            }
            else
            {
                if (this.pathName == null || this.pathName.Trim().Equals(string.Empty)
                    || this.pathDir == null || this.pathDir.Trim().Equals(string.Empty))
                {
                    this.Hint = "目录名、目录路径不能为空!";
                    return;
                }
                else
                {
                    this.DirList[this.DirList.IndexOf(this.selectItem)].Name = this.pathName;
                    this.DirList[this.DirList.IndexOf(this.selectItem)].Path = this.pathDir;
                    config.ListProductPath = this.dirList.ToList();
                    config.WriteConfigToFile();
                    this.DirList = new ObservableCollection<ProductPath>(config.ListProductPath);
                    this.Hint = "修改成功!";
                }
            }
        }

        public void Delete()
        {
            if (this.selectItem == null)
            {
                this.Hint = "请先选择一个产品目录!";
            }
            else
            {
                this.dirList.Remove(this.selectItem);
                config.ListProductPath = this.dirList.ToList();
                config.WriteConfigToFile();
                this.Hint = "删除成功!";
                this.PathName = "";
                this.PathDir = "";
            }
        }

        public void Change(object o)
        {
            if (this.selectItem != null)
            {
                if (this.selectItem.Name == null)
                {
                    this.PathName = "";
                }
                else
                {
                    this.PathName = this.selectItem.Name;
                }

                if (this.selectItem.Path == null)
                {
                    this.PathDir = "";
                }
                else
                {
                    this.PathDir = this.selectItem.Path;
                }
            }
        }

        public void Open()
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "指定输出目录";
            dlg.IsFolderPicker = true;
            //dlg.InitialDirectory = currentDirectory;

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            //dlg.DefaultDirectory = currentDirectory;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.PathDir = dlg.FileName;
                this.wd.Activate();
            }

        }
    }
}
