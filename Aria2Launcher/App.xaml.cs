using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Aria2Launcher.Services;
using GalaSoft.MvvmLight.Threading;
using Hardcodet.Wpf.TaskbarNotification;

namespace Aria2Launcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ConfigurationService.Current.Load();

            MainWindow = new MainWindow();
            MainWindow.Show();
        }
    }
}
