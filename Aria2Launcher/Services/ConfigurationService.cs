using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight;

namespace Aria2Launcher.Services
{
    public class ConfigurationService : ObservableObject
    {
        public static ConfigurationService Current { get; }

        static ConfigurationService()
        {
            Current = new ConfigurationService();
        }
        
        private string _aria2DirPath;
        private string _trackerSource;

        public string Aria2DirPath
        {
            get => _aria2DirPath;
            set => Set(ref _aria2DirPath, value.Trim().TrimEnd('\\'));
        }

        public string TrackerSource
        {
            get => _trackerSource;
            set => Set(ref _trackerSource, value);
        }
    }
}
