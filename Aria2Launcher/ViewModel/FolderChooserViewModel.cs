using Aria2Launcher.Services;

using CommunityToolkit.Mvvm.ComponentModel;

using HappyStudio.Mvvm.Input.Wpf;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aria2Launcher.ViewModel
{
    public class FolderChooserViewModel : ObservableRecipient
    {
        private Aria2Service _aria2Service;

        public FolderChooserViewModel(ConfigurationService appConf, Aria2Service aria2Service)
        {
            AppConf = appConf;
            AppConf.PropertyChanged += AppConf_PropertyChanged;

            _aria2Service = aria2Service;

            BrowseCommand = new RelayCommand(AppConf.BrowseAria2Folder);
            DownloadCommand = new RelayCommand(AppConf.ViewAria2OfficialSite);
        }

        public ConfigurationService AppConf { get; }

        public bool IsNoExe => !string.IsNullOrEmpty(AppConf.Aria2DirPath) && !_aria2Service.CheckExeExist();

        public RelayCommand BrowseCommand { get; }
        public RelayCommand DownloadCommand { get; }

        public bool CheckAria2ExeExist() => _aria2Service.CheckExeExist();

        private void AppConf_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppConf.Aria2DirPath))
                OnPropertyChanged(nameof(IsNoExe));
        }
    }
}
