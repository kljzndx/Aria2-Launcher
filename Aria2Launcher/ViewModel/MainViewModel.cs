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
            BrowseAria2DirCommand = new RelayCommand(() => Configuration.Aria2DirPath = BrowseFolder());
            StartAria2Command = new RelayCommand(Aria2.StartAria2, () => !Aria2.IsRunning);
            StopAria2Command = new RelayCommand(Aria2.StopAria2, () => Aria2.IsRunning);
            ShowConfigureCommand = new RelayCommand(() => new Aria2ConfigureWindow().Show());
            
            Aria2.Aria2Exited += Aria2Service_OnAria2Exited;
            Messenger.Default.Register<object>(this, "RestartAria2", m =>
            {
                if (!Aria2.IsRunning)
                    return;

                _needRestart = true;
                Aria2.StopAria2();
            });
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

        private void Aria2Service_OnAria2Exited(object sender, EventArgs e)
        {
            if (_needRestart)
            {
                _needRestart = false;
                Aria2.StartAria2();
                return;
            }
            
            StartAria2Command.RaiseCanExecuteChanged();
            StopAria2Command.RaiseCanExecuteChanged();
        }
    }
}
