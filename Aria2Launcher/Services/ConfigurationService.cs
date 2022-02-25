using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;

using CommunityToolkit.Mvvm.ComponentModel;

using Newtonsoft.Json;

namespace Aria2Launcher.Services
{
    public partial class ConfigurationService : ObservableObject
    {
        private const string ConfFileName = "Aria2Launcher.json";
        private const string LnkFileName = "Aria2 Launcher.lnk";

        private bool _isLoaded;

        private string _aria2DirPath;
        private string _trackerSource;
        private bool _isAutoStart;
        private bool _isOldAutoStartRemoved;

        public ConfigurationService()
        {
            _aria2DirPath = "";
            _trackerSource = "https://trackerslist.com/all.txt";
        }

        public string Aria2DirPath
        {
            get => _aria2DirPath;
            set => SetSetting(ref _aria2DirPath, value.Trim().TrimEnd('\\'));
        }

        public string TrackerSource
        {
            get => _trackerSource;
            set => SetSetting(ref _trackerSource, value);
        }

        public bool IsAutoStart
        {
            get => _isAutoStart;
            set => SetSetting(ref _isAutoStart, value);
        }

        public bool IsOldAutoStartRemoved
        {
            get => _isOldAutoStartRemoved;
            set => SetSetting(ref _isOldAutoStartRemoved, value);
        }

        private void SetSetting<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return;
            
            field = newValue;
            base.OnPropertyChanged(propertyName);

            if (_isLoaded)
                Save();
        }

        public void Save()
        {
            var confPath = GetConfPath();

            var text = JsonConvert.SerializeObject(this);
            File.WriteAllText(confPath, text);
        }

        public static string GetConfPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfFileName);
        }

        public static ConfigurationService GetService()
        {
            var confPath = GetConfPath();
            ConfigurationService service;

            if (File.Exists(confPath))
            {
                var text = File.ReadAllText(confPath);
                service = JsonConvert.DeserializeObject<ConfigurationService>(text);

                if (!service.IsOldAutoStartRemoved)
                {
                    service.RemoveOldAutoStart();
                    service.IsOldAutoStartRemoved = true;
                    service.Save();

                    if (service.IsAutoStart)
                        service.SwitchAutoStart(true);
                }
            }
            else
            {
                service = new ConfigurationService();
                service.Save();
            }

            service._isLoaded = true;
            return service;
        }
    }
}
