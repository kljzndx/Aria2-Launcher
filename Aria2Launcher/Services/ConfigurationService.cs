using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using Newtonsoft.Json;

namespace Aria2Launcher.Services
{
    public class ConfigurationService : ObservableObject
    {
        private const string ConfFileName = "Aria2Launcher.json";
        private const string LnkFileName = "Aria2Launcher.lnk";

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

        /// <summary>
        /// 为本程序创建一个快捷方式。
        /// </summary>
        /// <param name="lnkFilePath">快捷方式的完全限定路径。</param>
        /// <param name="args">快捷方式启动程序时需要使用的参数。</param>
        private void CreateShortcut(string lnkFilePath, string args = "")
        {
            var dllPath = Assembly.GetEntryAssembly()?.Location;

            if (string.IsNullOrWhiteSpace(dllPath))
                throw new Exception("无法获取程序的路径");

            var pathWithoutExtension = dllPath.Remove(dllPath.LastIndexOf('.') + 1);
            var exePath = pathWithoutExtension + "exe";

            var shell = new IWshRuntimeLibrary.WshShell();
            var shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(lnkFilePath);
            shortcut.TargetPath = exePath;
            shortcut.Arguments = args;
            shortcut.WorkingDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            shortcut.Save();
        }

        public void SwitchAutoStart(bool needAutoStart)
        {
            var lnkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), LnkFileName);
            if (File.Exists(lnkPath))
                File.Delete(lnkPath);

            if (!needAutoStart)
                return;

            CreateShortcut(lnkPath);
        }

        private void SetSetting<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return;
            
            field = newValue;
            base.OnPropertyChanged(propertyName);

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
