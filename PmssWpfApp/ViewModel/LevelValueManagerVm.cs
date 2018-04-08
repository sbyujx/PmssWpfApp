using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using PmssWpfApp.Dialogs;
using Pmss.Micaps.Render.Config;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace PmssWpfApp.ViewModel
{
    public class LevelValueManagerVm : INotifyPropertyChanged
    {
        public LevelValueManagerVm()
        {
            InitializeFromSettings();

            OpenLevelSettingDialogCommand = new DelegateCommand(OpenLevelSettingDialog);
            AddNewValueCommand = new DelegateCommand(AddNewValue);
            DeleteValueCommand = new DelegateCommand(DeleteValue);
            OnSelectedChangedCommand = new DelegateCommand<LevelSetting>(OnSelectedChanged);
            SelectedName = Names.First();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public Dictionary<string, LevelSettingList> Settings { get; set; } = LevelValueManager.Settings;
        public List<string> Names { get; set; } = new List<string>();
        public DelegateCommand OpenLevelSettingDialogCommand { get; set; }
        public DelegateCommand AddNewValueCommand { get; set; }
        public DelegateCommand DeleteValueCommand { get; set; }
        public DelegateCommand<LevelSetting> OnSelectedChangedCommand { get; set; }
        // Selected name changing will update the SelectedSettingList
        public string SelectedName
        {
            get
            {
                return this.selectedName;
            }
            set
            {
                this.selectedName = value;
                this.SelectedSettingList = Settings[value];
            }
        }
        // Have control logic of the list of LevelSetting
        public LevelSettingList SelectedSettingList
        {
            get
            {
                return this.selectedSettingList;
            }
            set
            {
                this.selectedSettingList = value;
                SelectedSettingVmList = this.selectedSettingList.LevelSettingVmList;
            }
        }
        // List of LevelSetting to be shown
        public ObservableCollection<LevelSetting> SelectedSettingVmList
        {
            get
            {
                return this.selectedSettingVmList;
            }
            set
            {
                this.selectedSettingVmList = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectedSettingVmList)));
                }
            }
        }
        public string InputValue { get; set; }

        private void OpenLevelSettingDialog()
        {
            var dialog = new LevelValueManagerWindow();
            dialog.ShowDialog();

            SaveBackToSettings();

        }
        private void AddNewValue()
        {
            if (string.IsNullOrWhiteSpace(InputValue))
                return;
            int value;
            if (int.TryParse(InputValue, out value))
            {
                SelectedSettingList.AddOrUpdateLevel(value);
            }
        }
        private void DeleteValue()
        {
            if (selectedSettingVm != null)
            {
                SelectedSettingList.RemoveLevel(selectedSettingVm.EndValue);
            }
        }
        private void OnSelectedChanged(LevelSetting setting)
        {
            selectedSettingVm = setting;
        }
        private void InitializeFromSettings()
        {
            var settings = Properties.Settings.Default;
            AddOneSetting(LevelValueManager.temperatureName, settings.TemperatureLevels, settings.TemperatureColors);
            AddOneSetting(LevelValueManager.RainName1, settings.Rain01Levels, settings.Rain01Colors);
            AddOneSetting(LevelValueManager.RainName3, settings.Rain03Levels, settings.Rain03Colors);
            AddOneSetting(LevelValueManager.RainName6, settings.Rain06Levels, settings.Rain06Colors);
            AddOneSetting(LevelValueManager.RainName12, settings.Rain12Levels, settings.Rain12Colors);
            AddOneSetting(LevelValueManager.RainName24, settings.Rain24Levels, settings.Rain24Colors);
            AddOneSetting(LevelValueManager.LevelNameFlood, settings.FloodLevels, settings.FloodColors);
            AddOneSetting(LevelValueManager.LevelNameDisaster, settings.DisasterLevels, settings.DisasterColors);
            AddOneSetting(LevelValueManager.RainName05Days, settings.Rain05DaysLevels, settings.Rain05DaysColors);
            AddOneSetting(LevelValueManager.RainName14Days, settings.Rain14DaysLevels, settings.Rain14DaysColors);
        }
        private void SaveBackToSettings()
        {
            var settings = Properties.Settings.Default;

            var setting = Settings[LevelValueManager.temperatureName];
            settings.TemperatureLevels = setting.GetLevelsString();
            settings.TemperatureColors = setting.GetColorsString();

            setting = Settings[LevelValueManager.RainName1];
            settings.Rain01Levels = setting.GetLevelsString();
            settings.Rain01Colors = setting.GetColorsString();

            setting = Settings[LevelValueManager.RainName3];
            settings.Rain03Levels = setting.GetLevelsString();
            settings.Rain03Colors = setting.GetColorsString();

            setting = Settings[LevelValueManager.RainName6];
            settings.Rain06Levels = setting.GetLevelsString();
            settings.Rain06Colors = setting.GetColorsString();

            setting = Settings[LevelValueManager.RainName12];
            settings.Rain12Levels = setting.GetLevelsString();
            settings.Rain12Colors = setting.GetColorsString();

            setting = Settings[LevelValueManager.RainName24];
            settings.Rain24Levels = setting.GetLevelsString();
            settings.Rain24Colors = setting.GetColorsString();

            setting = Settings[LevelValueManager.LevelNameFlood];
            settings.FloodLevels = setting.GetLevelsString();
            settings.FloodColors = setting.GetColorsString();

            setting = Settings[LevelValueManager.LevelNameDisaster];
            settings.DisasterLevels = setting.GetLevelsString();
            settings.DisasterColors = setting.GetColorsString();

            setting = Settings[LevelValueManager.RainName05Days];
            settings.Rain05DaysLevels = setting.GetLevelsString();
            settings.Rain05DaysColors = setting.GetColorsString();

            setting = Settings[LevelValueManager.RainName14Days];
            settings.Rain14DaysLevels = setting.GetLevelsString();
            settings.Rain14DaysColors = setting.GetColorsString();

            settings.Save();
        }
        private void AddOneSetting(string key, string levels, string colors)
        {
            Names.Add(key);
            var tempSetting = new LevelSettingList(key, levels, colors);
            Settings.Add(key, tempSetting);
        }

        private string selectedName;
        private LevelSettingList selectedSettingList;
        private ObservableCollection<LevelSetting> selectedSettingVmList;
        private LevelSetting selectedSettingVm;
    }
}
