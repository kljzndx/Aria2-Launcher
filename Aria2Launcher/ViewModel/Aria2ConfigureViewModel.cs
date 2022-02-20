using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using Aria2Launcher.Models.SettingModels;
using Aria2Launcher.Services;

using CommunityToolkit.Mvvm.ComponentModel;

using HappyStudio.Mvvm.Input.Wpf;

namespace Aria2Launcher.ViewModel
{
    public class Aria2ConfigureViewModel : ObservableRecipient
    {
        private Aria2Service _aria2Service;
        private Aria2ConfService _aria2ConfService;
        private SettingGroup _selectGroup;

        public Aria2ConfigureViewModel(ConfigurationService appConf, Aria2Service a2)
        {
            AppConfService = appConf;
            _aria2Service = a2;

            string json = File.ReadAllText("./Aria2ConfDoc.json");
            _aria2ConfService = new Aria2ConfService(json);
            
            UpdateTrackerCommand = new AsyncRelayCommand<string>(_aria2ConfService.UpdateTracker,
                s => {
                    if (s != null)
                        return s.StartsWith("http");
                    return false;
                });
        }

        public ConfigurationService AppConfService { get; }

        public SettingGroup SelectGroup
        {
            get => _selectGroup;
            set => SetProperty(ref _selectGroup, value);
        }

        public ObservableCollection<SettingGroup> SettingGroupList => _aria2ConfService.SettingGroupList;

        public AsyncRelayCommand<string> UpdateTrackerCommand { get; }

        public bool Load()
        {
            if (!_aria2Service.CheckConfExist())
                return false;

            _aria2ConfService.Load(_aria2Service.GetConfPath());
            SelectGroup = SettingGroupList.FirstOrDefault();

            return true;
        }

        public void Save()
        {
            _aria2ConfService.Save();
            _aria2Service.RestartAria2();
        }
    }
}
