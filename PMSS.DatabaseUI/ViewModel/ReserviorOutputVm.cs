﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using System.ComponentModel;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;
using PMSS.SqlDataOutput;
using Microsoft.Win32;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;

namespace PMSS.DatabaseUI.ViewModel
{
    public class ReserviorOutputVm : INotifyPropertyChanged
    {
        public DelegateCommand DataOutCmd { get; set; }
        public DelegateCommand DataShowCmd { get; set; }
        public DelegateCommand SearchCmd { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ReserviorOutputVm()
        {
            this.DataOutCmd = new DelegateCommand(DataOut);
            this.DataShowCmd = new DelegateCommand(DataShow);
            this.SearchCmd = new DelegateCommand(DataSearch);
            this.StationOption = new ObservableCollection<HydroStationOutOption>();
            this.TimeFrom = new DateTime(DateTime.Now.Year, 1, 1);
            this.TimeTo = DateTime.Now;
            Thread thread = new Thread(InitStationInfo);
            thread.Start();
        }

        private ObservableCollection<HydroStationOutOption> stationOption;
        public ObservableCollection<HydroStationOutOption> StationOption
        {
            get
            {
                return this.stationOption;
            }
            set
            {
                this.stationOption = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(StationOption)));
                }
            }
        }

        private DateTime timeFrom;
        public DateTime TimeFrom
        {
            get
            {
                return this.timeFrom;
            }
            set
            {
                this.timeFrom = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(TimeFrom)));
                }
            }
        }

        private DateTime timeTo;
        public DateTime TimeTo
        {
            get
            {
                return this.timeTo;
            }
            set
            {
                this.timeTo = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(TimeTo)));
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

        private string searchContent;
        public string SearchContent
        {
            get
            {
                return this.searchContent;
            }
            set
            {
                this.searchContent = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(SearchContent)));
                }
            }
        }

        public void DataShow()
        {
            OutPutSelectOption option = new OutPutSelectOption();
            option.IsSetStationRange = true;
            foreach (HydroStationOutOption item in this.StationOption)
            {
                if (item.IsSelected == true)
                {
                    option.AddStationId(item.StationId);
                    break;
                }
            }

            if(option.StationIdList.Count == 0)
            {
                this.Hint = "请至少选择一个站点！";
                return;
            }

            option.IsSetTimeRange = true;
            option.From = this.timeFrom;
            option.To = this.timeTo;

            this.Hint = "正在生成水库水情曲线...";
            ShowReservoirData(option);

            //Thread thread = new Thread(unused => ShowReservoirData(option));
            //thread.Start();
        }

        private void ShowReservoirData(object option)
        {
            ShowReservoirData((OutPutSelectOption)option);
        }

        private void ShowReservoirData(OutPutSelectOption option)
        {
            FetchReservoirRecord frr = new FetchReservoirRecord();

            if (option.IsSetTimeRange == true)
            {
                frr.SetDuration(option.From, option.To);
            }

            if (option.IsSetStationRange == true)
            {
                frr.SetStationId(option.StationIdList);
            }

            frr.FecthData();

            WindowShowReserviorChart wd = new WindowShowReserviorChart(frr.ReservoirList);
            wd.ShowDialog();

            this.Hint = "水库水情曲线生成完成!";
        }


        public void DataOut()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "TXT文件 (*.txt)|*.txt|EXCEL文件 (*.xlsx)|*.xlsx";
            dlg.DefaultExt = "txt";
            dlg.Title = "保存水库水情";
            dlg.ShowDialog();

            OutPutSelectOption option = new OutPutSelectOption();
            option.IsSetStationRange = true;
            foreach(HydroStationOutOption item in this.StationOption)
            {
                if(item.IsSelected == true)
                {
                    option.AddStationId(item.StationId);
                }
            }
            option.IsSetTimeRange = true;
            option.From = this.timeFrom;
            option.To = this.timeTo;

            if(dlg.FileName != "")
            {
                switch (dlg.FilterIndex)
                {
                    case 1:  //TXT文件
                        option.FileType = OutPutSelectOption.FILE_TYPE_TXT;
                        break;
                    case 2: //EXECL文件
                        option.FileType = OutPutSelectOption.FILE_TYPE_EXCEL;
                        break;
                }

                Thread thread = new Thread(unused => SaveReservoirFile(dlg.FileName, option));
                thread.Start();
            }
            else
            {
                this.Hint = "请选择一个文件名!";
            }
        }

        private void SaveReservoirFile(object fileName, object option)
        {
            SaveReservoirFile((string)fileName, (OutPutSelectOption)option);
        }

        private void SaveReservoirFile(string fileName, OutPutSelectOption option)
        {
            this.Hint = "保存中...";
            FetchReservoirRecord frr = new FetchReservoirRecord();

            if (option.IsSetTimeRange == true)
            {
                frr.SetDuration(option.From, option.To);
            }

            if (option.IsSetStationRange == true)
            {
                frr.SetStationId(option.StationIdList);
            }

            frr.FecthData();

            if (option.FileType == OutPutSelectOption.FILE_TYPE_EXCEL)
            {
                frr.SaveToExcelFile(fileName);
            }

            if (option.FileType == OutPutSelectOption.FILE_TYPE_TXT)
            {
                frr.SaveToTxTFile(fileName);
            }

            this.Hint = "保存完成!";
        }

        public void InitStationInfo()
        {
            this.reservoirList = GetAllReservoirStationInfo();            
            foreach (ReservoirHydrologyRecord rhr in this.reservoirList)
            {
                HydroStationOutOption s = new HydroStationOutOption();
                s.StationId = rhr.StationId;
                s.StationName = rhr.StationName;
                s.Address = rhr.StationLocation;
                s.RiverName = rhr.RiverName;
                s.IsSelected = false;
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    this.StationOption.Add(s);
                }));
               
            }
        }

        public void DataSearch()
        {
            if (this.SearchContent == null || this.SearchContent.Trim().Equals(string.Empty))
            {
                this.Hint = "搜索内容不能为空！";
                return;
            }

            int index = Search(this.SearchContent.Trim());
            if (index < 0)
            {
                this.Hint = "没有搜索到相关站点！";
                return;
            }
            else
            {
                SelectRowByIndex(ReservoirDataGrid, index);
                return;
            }
        }

        private List<ReservoirHydrologyRecord> reservoirList;

        private int listCurrent = 0;

        public int Search(string content)
        {
            int hasSearchCount = 0;

            while (hasSearchCount < this.reservoirList.Count)
            {
                if (this.reservoirList[listCurrent].StationId != null && this.reservoirList[listCurrent].StationId.Contains(content))
                {
                    return listCurrent++;
                }

                if (this.reservoirList[listCurrent].StationName != null && this.reservoirList[listCurrent].StationName.Contains(content))
                {
                    return listCurrent++;
                }

                if (this.reservoirList[listCurrent].StationLocation != null && this.reservoirList[listCurrent].StationLocation.Contains(content))
                {
                    return listCurrent++;
                }

                if (this.reservoirList[listCurrent].RiverName != null && this.reservoirList[listCurrent].RiverName.Contains(content))
                {
                    return listCurrent++;
                }

                listCurrent++;
                if (this.listCurrent >= this.reservoirList.Count)
                {
                    listCurrent = 0;
                }
                hasSearchCount++;
            }

            return -1;  //搜索一圈仍无匹配
        }

        public DataGrid ReservoirDataGrid;

        public void SelectRowByIndex(DataGrid dataGrid, int rowIndex)
        {
            if (!dataGrid.SelectionUnit.Equals(DataGridSelectionUnit.FullRow))
                throw new ArgumentException("The SelectionUnit of the DataGrid must be set to FullRow.");

            if (rowIndex < 0)
            {
                throw new Exception("无此站点！");
            }

            if (rowIndex > (dataGrid.Items.Count - 1))
                throw new ArgumentException(string.Format("{0} is an invalid row index.", rowIndex));

            dataGrid.SelectedItems.Clear();
            /* set the SelectedItem property */
            object item = dataGrid.Items[rowIndex]; // = Product X
            dataGrid.SelectedItem = item;

            DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
            if (row == null)
            {
                /* bring the data item (Product object) into view
                 * in case it has been virtualized away */
                dataGrid.ScrollIntoView(item);
                row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
            }
            //TODO: Retrieve and focus a DataGridCell object
        }

        private List<ReservoirHydrologyRecord> GetAllReservoirStationInfo()
        {
            List<ReservoirHydrologyRecord> list = new List<ReservoirHydrologyRecord>();
            try
            {
                FetchReservoirRecord frr = new FetchReservoirRecord();
                frr.GetAllReservoirStation();
                list = frr.ReservoirList;
            }
            catch (MySqlException )
            {
                this.Hint = "无法连接至数据库!";
            }
            return list;
        }
    }
}
