using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace Aria2Launcher.Services
{
    public class ConfigurationService : ObservableObject
    {
        private const string ConfFileName = "Aria2Launcher.json";

        public static ConfigurationService Current { get; }

        static ConfigurationService()
        {
            Current = new ConfigurationService();
        }

        private bool _isUpdating;

        private string _aria2DirPath;
        private string _trackerSource;
        private bool _isAutoStart;

        private ConfigurationService()
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

        private void SetSetting<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return;
            
            field = newValue;
            base.RaisePropertyChanged(propertyName);

            if (!_isUpdating)
                Save(propertyName);
        }

        private void Update(ConfigurationService newService)
        {
            _isUpdating = true;

            var props = this.GetType().GetProperties();

            foreach (var info in props)
            {
                if (!info.CanWrite || info.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                    continue;

                var newVal = info.GetValue(newService);
                info.SetValue(this, newVal);
            }

            _isUpdating = false;
        }

        public void Load()
        {
            string dirPath = AppDomain.CurrentDomain.BaseDirectory;
            var confPath = dirPath + "\\" + ConfFileName;
            
            if (File.Exists(confPath))
            {
                var text = File.ReadAllText(confPath);
                var service = JsonConvert.DeserializeObject<ConfigurationService>(text);
                Update(service);
            }
            else
                Save();
        }

        public void Save([CallerMemberName] string propertyName = null)
        {
            string dirPath = AppDomain.CurrentDomain.BaseDirectory;
            var confPath = dirPath + "\\" + ConfFileName;

            var text = JsonConvert.SerializeObject(this);
            File.WriteAllText(confPath, text);
        }
    }
}
