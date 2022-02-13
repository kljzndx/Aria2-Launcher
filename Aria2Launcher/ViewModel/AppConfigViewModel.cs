using Aria2Launcher.Services;

using GalaSoft.MvvmLight;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aria2Launcher.ViewModel
{
    public class AppConfigViewModel : ViewModelBase
    {
        private bool _falledExe;
        private bool _falledConf;
        private bool _isEmptyPath;

        public AppConfigViewModel()
        {
            Aria2 = Aria2Service.Current;
            Configuration = ConfigurationService.Current;

            CheckAria2Path();
            Configuration.PropertyChanged += Configuration_PropertyChanged;
        }

        public Aria2Service Aria2 { get; }
        public ConfigurationService Configuration { get; }

        public bool IsEmptyPath
        {
            get { return _isEmptyPath; }
            set { Set(ref _isEmptyPath, value); }
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
            IsEmptyPath = string.IsNullOrWhiteSpace(Configuration.Aria2DirPath);
            FalledExe = Aria2.CheckExeExist();
            FalledConf = Aria2.CheckConfExist();
        }

        private void Configuration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Configuration.Aria2DirPath))
                CheckAria2Path();
        }
    }
}
