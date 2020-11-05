using System.Linq;
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

            var tb = (TaskbarIcon) FindResource("taskbar");

            ConfigurationService.Current.Load();

            if (e.Args.Any(s => s == "-q"))
            {
                Aria2Service.Current.StartAria2();
            }
            else
            {
                MainWindow = new MainWindow();
                MainWindow.Show();
            }

            Aria2Service.Current.ErrorDataReceived += Aria2Service_ErrorDataReceived;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (Aria2Service.Current.IsRunning)
                Aria2Service.Current.StopAria2();

            base.OnExit(e);
        }

        private void Aria2Service_ErrorDataReceived(object sender, string e)
        {
            if (MainWindow == null)
            {
                MainWindow = new MainWindow();
                MainWindow.Show();
            }
        }
    }
}
