using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Aria2Launcher.Services;
using Aria2Launcher.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace Aria2Launcher.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private bool _needRestart;
        
        public MainViewModel()
        {
            StartAria2Command = new RelayCommand(Aria2.StartAria2, () => !Aria2.IsRunning);
            StopAria2Command = new RelayCommand(Aria2.StopAria2, () => Aria2.IsRunning);
            ShowAria2ConfWindowCommand = new RelayCommand(() => new Aria2ConfigureWindow().Show());
            ShowAppConfWindowCommand = new RelayCommand(() => new AppConfigWindow().Show());

            Aria2.Aria2Exited += Aria2Service_OnAria2Exited;
        }

        public ConfigurationService Configuration { get; } = ConfigurationService.Current;
        public Aria2Service Aria2 { get; } = Aria2Service.Current;

        public ObservableCollection<string> OutputList => Aria2.OutputList;
        
        public RelayCommand StartAria2Command { get; }
        public RelayCommand StopAria2Command { get; }
        public RelayCommand ShowAria2ConfWindowCommand { get; }
        public RelayCommand ShowAppConfWindowCommand { get; }

        private void Aria2Service_OnAria2Exited(object sender, EventArgs e)
        {
            StartAria2Command.RaiseCanExecuteChanged();
            StopAria2Command.RaiseCanExecuteChanged();
        }
    }
}
