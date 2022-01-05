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
    public class AppConfigService : ObservableObject
    {
        private ApplicationDataContainer _settings;

        private string _workingDir;

        public AppConfigService()
        {
            _settings = ApplicationData.Current.LocalSettings;

            _workingDir = GetSetting(nameof(WorkingDir), "");
        }

        public string WorkingDir
        {
            get { return _workingDir; }
            set { SetSetting(ref _workingDir, value); }
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
