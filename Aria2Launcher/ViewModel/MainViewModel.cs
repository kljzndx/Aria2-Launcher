using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Aria2Launcher.Services;
using Aria2Launcher.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace Aria2Launcher.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            BrowseAria2DirCommand = new RelayCommand(() => Configuration.Aria2DirPath = BrowseFolder());
            StartAria2Command = new RelayCommand(Aria2.StartAria2, () => !Aria2.IsRunning);
            StopAria2Command = new RelayCommand(Aria2.StopAria2, () => Aria2.IsRunning);
            ShowConfigureCommand = new RelayCommand(() => new Aria2ConfigureWindow().Show());
        }

        public ConfigurationService Configuration { get; } = ConfigurationService.Current;
        public Aria2Service Aria2 { get; } = Aria2Service.Current;

        public ObservableCollection<string> OutputList => Aria2.OutputList;
        
        public RelayCommand BrowseAria2DirCommand { get; }
        public RelayCommand StartAria2Command { get; }
        public RelayCommand StopAria2Command { get; }
        public RelayCommand ShowConfigureCommand { get; }

        private string BrowseFolder()
        {
            using (var picker = new FolderBrowserDialog())
            {
                picker.RootFolder = Environment.SpecialFolder.MyComputer;
                var dialogResult = picker.ShowDialog();
                return dialogResult == DialogResult.OK ? picker.SelectedPath : Configuration.Aria2DirPath;
            }
        }
    }
}
