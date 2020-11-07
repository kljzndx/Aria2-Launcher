using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using Aria2Launcher.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace Aria2Launcher.ViewModel
{
    public class TaskBarViewModel : ViewModelBase
    {
        private const string LnkFileName = "Aria2Launcher.lnk";
        
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

            AutoStartCommand = new RelayCommand<bool>(SwitchAutoStart);
        }

        public Aria2Service Aria2 { get; } = Aria2Service.Current;
        public ConfigurationService Configuration { get; } = ConfigurationService.Current;

        public RelayCommand StartAria2Command { get; }
        public RelayCommand StopAria2Command { get; }

        public RelayCommand ShowWindowCommand { get; }
        public RelayCommand ExitAppCommand { get; }
        
        public RelayCommand<bool> AutoStartCommand { get; }



        /// <summary>
        /// 为本程序创建一个快捷方式。
        /// </summary>
        /// <param name="lnkFilePath">快捷方式的完全限定路径。</param>
        /// <param name="args">快捷方式启动程序时需要使用的参数。</param>
        private void CreateShortcut(string lnkFilePath, string args)
        {
            var dllPath = Assembly.GetEntryAssembly()?.Location;

            if (string.IsNullOrWhiteSpace(dllPath))
                throw new Exception("无法获取程序的路径");

            var pathWithoutExtension = dllPath.Remove(dllPath.LastIndexOf('.') + 1);
            var exePath = pathWithoutExtension + "exe";

            var shell = new IWshRuntimeLibrary.WshShell();
            var shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(lnkFilePath);
            shortcut.TargetPath = exePath;
            shortcut.Arguments = args;
            shortcut.WorkingDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            shortcut.Save();
        }
        
        private void SwitchAutoStart(bool needAutoStart)
        {
            var lnkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), LnkFileName);
            if (File.Exists(lnkPath))
                File.Delete(lnkPath);

            if (!needAutoStart)
                return;

            CreateShortcut(lnkPath, "-q");
        }
    }
}