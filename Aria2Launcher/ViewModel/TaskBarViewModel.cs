using System.Windows;
using Aria2Launcher.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace Aria2Launcher.ViewModel
{
    public class TaskBarViewModel : ViewModelBase
    {
        public TaskBarViewModel()
        {
            StartAria2Command = new RelayCommand(Aria2.StartAria2, () => !Aria2.IsRunning);
            StopAria2Command = new RelayCommand(Aria2.StopAria2, () => Aria2.IsRunning);

            ShowWindowCommand = new RelayCommand(() =>
            {
                Application.Current.MainWindow = new MainWindow();
                Application.Current.MainWindow.Show();
            }, () => Application.Current.MainWindow == null);

            ExitAppCommand = new RelayCommand(() =>
            {
                Application.Current.Shutdown();
            });
        }
        
        public Aria2Service Aria2 { get; } = Aria2Service.Current;

        public RelayCommand StartAria2Command { get; }
        public RelayCommand StopAria2Command { get; }

        public RelayCommand ShowWindowCommand { get; }
        
        public RelayCommand ExitAppCommand { get; }
    }
}