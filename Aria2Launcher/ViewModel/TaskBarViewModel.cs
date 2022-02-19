using System;
using System.Diagnostics;
using System.IO;
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

            AutoStartCommand = new RelayCommand<bool>(Configuration.SwitchAutoStart);
            
            Aria2.Aria2Exited += Aria2Service_OnAria2Exited;
        }

        public Aria2Service Aria2 { get; } = Aria2Service.Current;
        public ConfigurationService Configuration { get; } = ConfigurationService.Current;

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