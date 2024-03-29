﻿using System.IO;
using System.Linq;
using System.Windows;

using Aria2Launcher.Services;
using Aria2Launcher.ViewModel;
using Aria2Launcher.ViewModel.Extensions;
using Aria2Launcher.Views;

using CommunityToolkit.Mvvm.DependencyInjection;

using Hardcodet.Wpf.TaskbarNotification;

using Microsoft.Extensions.DependencyInjection;

namespace Aria2Launcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                .AddSingleton<ConfigurationService>(ioc => ConfigurationService.GetService())
                .AddSingleton<Aria2Service>()

                .AddSingleton<TaskBarViewModel>()
                .AddSingleton<MainViewModel>()
                .AddSingleton<AppConfigViewModel>()
                .AddSingleton<Aria2ConfigureViewModel>()
                .AddSingleton<FolderChooserViewModel>()

                .BuildServiceProvider());

            var tb = (TaskbarIcon) FindResource("taskbar");

            var a2 = Ioc.Default.GetRequiredService<Aria2Service>();
            a2.ErrorDataReceived += Aria2Service_ErrorDataReceived;

            bool isExeExisted = a2.CheckExeExist();
            if (e.Args.Contains("--quiet") && isExeExisted)
            {
                a2.StartAria2();
                return;
            }
            if (!isExeExisted)
            {
                new FolderChooserWindow().Show();
                return;
            }
            
            new MainWindow().Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var a2 = Ioc.Default.GetRequiredService<Aria2Service>();
            if (a2.IsRunning)
                a2.StopAria2();

            base.OnExit(e);
        }

        private void Aria2Service_ErrorDataReceived(object sender, string e)
        {
            if (!Windows.ContainsWindow<MainWindow>())
                new MainWindow().Show();
        }
    }
}
