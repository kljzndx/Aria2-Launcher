using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using Aria2Launcher.Models.SettingModels;
using Aria2Launcher.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace Aria2Launcher.ViewModel
{
    public class Aria2ConfigureViewModel : ViewModelBase
    {
        private Aria2Service _aria2Service;
        private Aria2ConfService _aria2ConfService;
        private SettingGroup _selectGroup;

        public Aria2ConfigureViewModel()
        {
            _aria2Service = Aria2Service.Current;

            string json = File.ReadAllText("./Aria2ConfDoc.json");
            _aria2ConfService = new Aria2ConfService(json);
            
            UpdateTrackerCommand = new RelayCommand<string>(async s => await _aria2ConfService.UpdateTracker(s),
                s => {
                    if (s != null)
                        return s.StartsWith("http");
                    return false;
                });
        }

        public ConfigurationService AppConfService => ConfigurationService.Current;

        public SettingGroup SelectGroup
        {
            get => _selectGroup;
            set => Set(ref _selectGroup, value);
        }

        public ObservableCollection<SettingGroup> SettingGroupList => _aria2ConfService.SettingGroupList;

        public RelayCommand<string> UpdateTrackerCommand { get; }

        public void Load(string confPath)
        {
            _aria2ConfService.Load(confPath);
            SelectGroup = SettingGroupList.FirstOrDefault();
        }

        public void Save()
        {
            _aria2ConfService.Save();
            _aria2Service.RestartAria2();
        }
    }
}
