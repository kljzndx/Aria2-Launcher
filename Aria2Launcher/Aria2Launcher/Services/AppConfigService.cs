using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;

namespace Aria2Launcher.Services
{
    public class AppConfigService : ObservableObject, IAppConfigService
    {
        private ApplicationDataContainer _settings;

        private string _programDir;
        private string _btTrackerSource;
        private bool _useCustomDocFile;
        private bool _autoStartWhenSystemLogin;

        public AppConfigService()
        {
            _settings = ApplicationData.Current.LocalSettings;

            _programDir = GetSetting(nameof(ProgramDir), "");
            _btTrackerSource = GetSetting(nameof(BtTrackerSource), "");
            _useCustomDocFile = GetSetting(nameof(UseCustomDocFile), false);
            _autoStartWhenSystemLogin = GetSetting(nameof(AutoStartWhenSystemLogin), false);
        }

        public string ProgramDir
        {
            get { return _programDir; }
            set { SetSetting(ref _programDir, value); }
        }

        public string BtTrackerSource
        {
            get => _btTrackerSource;
            set => SetSetting(ref _btTrackerSource, value);
        }

        public bool UseCustomDocFile
        {
            get => _useCustomDocFile;
            set => SetSetting(ref _useCustomDocFile, value);
        }

        public bool AutoStartWhenSystemLogin
        {
            get => _autoStartWhenSystemLogin;
            set => SetSetting(ref _autoStartWhenSystemLogin, value);
        }

        private T GetSetting<T>(string settingName, T defaultValue)
        {
            if (_settings.Values.ContainsKey(settingName))
                return (T)_settings.Values[settingName];

            _settings.Values.Add(settingName, defaultValue);
            return defaultValue;
        }

        private void SetSetting<TProp>(ref TProp field, TProp newValue, string? settingName = null, [CallerMemberName] string? propertyName = null)
        {
            if (propertyName == null || Object.ReferenceEquals(field, newValue))
                return;
            
            SetProperty(ref field, newValue, propertyName);
            _settings.Values[settingName ?? propertyName] = newValue;
        }
    }
}
