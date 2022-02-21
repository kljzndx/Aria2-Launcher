using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Aria2Launcher.Services;
using Aria2Launcher.Views;

using CommunityToolkit.Mvvm.ComponentModel;

using HappyStudio.Mvvm.Input.Wpf;

namespace Aria2Launcher.ViewModel
{
    public class TaskBarViewModel : ObservableRecipient
    {
        public TaskBarViewModel(ConfigurationService appConf, Aria2Service a2)
        {
            Configuration = appConf;
            Aria2 = a2;

            StartAria2Command = new RelayCommand(Aria2.StartAria2, () => !Aria2.IsRunning);
            StopAria2Command = new RelayCommand(Aria2.StopAria2, () => Aria2.IsRunning);

            ShowWindowCommand = new RelayCommand(() => new MainWindow().Show(), 
                () => Application.Current.Windows.Cast<Window>().All(w => w is not MainWindow));

            ExitAppCommand = new RelayCommand(() =>
            {
                Application.Current.Shutdown();
            });

            AutoStartCommand = new RelayCommand<bool>(Configuration.SwitchAutoStart);
            
            Aria2.Aria2Exited += Aria2Service_OnAria2Exited;
        }

        public Aria2Service Aria2 { get; }
        public ConfigurationService Configuration { get; }

        public RelayCommand StartAria2Command { get; }
        public RelayCommand StopAria2Command { get; }

        public RelayCommand ShowWindowCommand { get; }
        public RelayCommand ExitAppCommand { get; }
        
        public RelayCommand<bool> AutoStartCommand { get; }

        private void Aria2Service_OnAria2Exited(object sender, EventArgs e)
        {
            StartAria2Command.NotifyCanExecuteChanged();
            StopAria2Command.NotifyCanExecuteChanged();
        }
    }
}