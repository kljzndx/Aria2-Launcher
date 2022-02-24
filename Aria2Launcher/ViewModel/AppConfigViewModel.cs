using Aria2Launcher.Services;

using CommunityToolkit.Mvvm.ComponentModel;

using HappyStudio.Mvvm.Input.Wpf;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aria2Launcher.ViewModel
{
    public class AppConfigViewModel : ObservableRecipient
    {
        private bool _falledExe;
        private bool _falledConf;
        private bool _hasAria2Path;

        public AppConfigViewModel(ConfigurationService appConf, Aria2Service a2)
        {
            Configuration = appConf;
            Aria2 = a2;

            CheckAria2Path();
            Configuration.PropertyChanged += Configuration_PropertyChanged;

            BrowseAria2DirCommand = new RelayCommand(Configuration.BrowseAria2Folder);
            ViewOfficialSiteCommand = new RelayCommand(Configuration.ViewAria2OfficialSite);
            GenerateConfFileCommand = new RelayCommand(() =>
            {
                File.WriteAllText(Aria2.GetConfPath(), "enable-rpc=true");
                FalledConf = false;
            },
            () => !HasAria2Path || FalledConf);
            SwitchAutoStartCommand = new RelayCommand<bool>(Configuration.SwitchAutoStart);
        }

        public Aria2Service Aria2 { get; }
        public ConfigurationService Configuration { get; }

        public RelayCommand BrowseAria2DirCommand { get; }
        public RelayCommand ViewOfficialSiteCommand { get; }
        public RelayCommand GenerateConfFileCommand { get; }
        public RelayCommand<bool> SwitchAutoStartCommand { get; set; }

        public bool HasAria2Path
        {
            get { return _hasAria2Path; }
            set { SetProperty(ref _hasAria2Path, value); }
        }

        public bool FalledExe
        {
            get { return _falledExe; }
            set { SetProperty(ref _falledExe, value); }
        }

        public bool FalledConf
        {
            get { return _falledConf; }
            set { SetProperty(ref _falledConf, value); }
        }

        public void CheckAria2Path()
        {
            HasAria2Path = !string.IsNullOrWhiteSpace(Configuration.Aria2DirPath);
            FalledExe = !Aria2.CheckExeExist();
            FalledConf = !Aria2.CheckConfExist();
        }

        private void Configuration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Configuration.Aria2DirPath))
                CheckAria2Path();
        }
    }
}
