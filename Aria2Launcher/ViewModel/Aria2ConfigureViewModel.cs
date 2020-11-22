using System.Collections.Generic;
using System.IO;
using Aria2Launcher.Models.SettingModels;
using Aria2Launcher.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace Aria2Launcher.ViewModel
{
    public class Aria2ConfigureViewModel : ViewModelBase
    {
        public Aria2ConfigureViewModel()
        {
            string json = File.ReadAllText("./Assets/Aria2ConfDoc.json");
            Aria2ConfService = new Aria2ConfService(json);
        }
        
        public Aria2ConfService Aria2ConfService { get; }
        public ConfigurationService AppConfService { get; } = ConfigurationService.Current;

        private SettingGroup _selectGroup;

        public SettingGroup SelectGroup
        {
            get => _selectGroup;
            set => Set(ref _selectGroup, value);
        }
    }
}
