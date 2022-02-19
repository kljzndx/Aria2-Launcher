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

        public AppConfigViewModel()
        {
            Aria2 = Aria2Service.Current;
            Configuration = ConfigurationService.Current;

            CheckAria2Path();
            Configuration.PropertyChanged += Configuration_PropertyChanged;

            BrowseAria2DirCommand = new RelayCommand(() => Configuration.Aria2DirPath = BrowseFolder());
            ViewOfficialSiteCommand = new RelayCommand(ViewOfficialSite);
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

        private string BrowseFolder()
        {
            using (var picker = new FolderBrowserDialog())
            {
                picker.RootFolder = Environment.SpecialFolder.MyComputer;
                var dialogResult = picker.ShowDialog();
                return dialogResult == DialogResult.OK ? picker.SelectedPath : Configuration.Aria2DirPath;
            }
        }

        public void CheckAria2Path()
        {
            HasAria2Path = !string.IsNullOrWhiteSpace(Configuration.Aria2DirPath);
            FalledExe = !Aria2.CheckExeExist();
            FalledConf = !Aria2.CheckConfExist();
        }

        public void ViewOfficialSite()
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = "https://github.com/aria2/aria2/releases/latest";
                process.StartInfo.UseShellExecute = true;
                process.Start();
            }
        }

        private void Configuration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Configuration.Aria2DirPath))
                CheckAria2Path();
        }
    }
}
