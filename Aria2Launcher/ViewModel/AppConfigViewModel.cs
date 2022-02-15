using Aria2Launcher.Services;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aria2Launcher.ViewModel
{
    public class AppConfigViewModel : ViewModelBase
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

            GenerateConfFileCommand = new RelayCommand(() => File.WriteAllText(Aria2.GetConfPath(), "enable-rpc=true"),
                () => !HasAria2Path || Aria2.CheckConfExist());
        }

        public Aria2Service Aria2 { get; }
        public ConfigurationService Configuration { get; }

        public RelayCommand GenerateConfFileCommand { get; }

        public bool HasAria2Path
        {
            get { return _hasAria2Path; }
            set { Set(ref _hasAria2Path, value); }
        }

        public bool FalledExe
        {
            get { return _falledExe; }
            set { Set(ref _falledExe, value); }
        }

        public bool FalledConf
        {
            get { return _falledConf; }
            set { Set(ref _falledConf, value); }
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
