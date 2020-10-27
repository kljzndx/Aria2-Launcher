using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Aria2Launcher.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;

namespace Aria2Launcher.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private bool _isRunning;
        private Process _aria2Process;

        public MainViewModel()
        {
            _aria2Process = new Process();

            _aria2Process.StartInfo.CreateNoWindow = true;
            _aria2Process.EnableRaisingEvents = true;

            _aria2Process.StartInfo.RedirectStandardOutput = true;
            _aria2Process.StartInfo.RedirectStandardError = true;

            _aria2Process.OutputDataReceived += Aria2Process_OutputDataReceived;
            _aria2Process.ErrorDataReceived += Aria2Process_ErrorDataReceived;
            _aria2Process.Exited += Aria2Process_Exited;

            BrowseAria2DirCommand = new RelayCommand(() => Configuration.Aria2DirPath = BrowseFolder());

            StartAria2Command = new RelayCommand(StartAria2, () => !IsRunning);
            StopAria2Command = new RelayCommand(StopAria2, () => IsRunning);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => Set(ref _isRunning, value);
        }

        public ConfigurationService Configuration { get; } = ConfigurationService.Current;

        public ObservableCollection<string> OutputList { get; } = new ObservableCollection<string>();

        public RelayCommand BrowseAria2DirCommand { get; }
        public RelayCommand UpgradeTrackerCommand { get; }

        public RelayCommand StartAria2Command { get; }
        public RelayCommand StopAria2Command { get; }

        private void Log(string content)
        {
            if (String.IsNullOrWhiteSpace(content))
                return;

            OutputList.Add(content);

            if (OutputList.Count > 50)
                OutputList.RemoveAt(0);
        }

        private bool SetupProcess()
        {
            string exePath = Configuration.Aria2DirPath + "\\aria2c.exe";
            string confPath = Configuration.Aria2DirPath + "\\aria2.conf";

            if (File.Exists(exePath))
                Log("已找到 aria2c.exe");
            else
            {
                Log("未找到 aria2c.exe");
                return false;
            }
            
            if (File.Exists(confPath))
                Log("已找到 aria2.conf");
            else
            {
                Log("未找到 aria2.conf");
                return false;
            }
            
            _aria2Process.StartInfo.FileName = exePath;
            _aria2Process.StartInfo.Arguments = $"--conf-path {confPath}";
            _aria2Process.StartInfo.WorkingDirectory = Configuration.Aria2DirPath;
            
            return true;
        }

        private void StartAria2()
        {
            OutputList.Clear();

            if (!SetupProcess())
                return;

            _aria2Process.Start();

            _aria2Process.BeginOutputReadLine();
            _aria2Process.BeginErrorReadLine();

            IsRunning = true;
        }

        private void StopAria2()
        {
            _aria2Process.Kill();
        }

        private string BrowseFolder()
        {
            using (var picker = new FolderBrowserDialog())
            {
                picker.RootFolder = Environment.SpecialFolder.MyComputer;
                var dialogResult = picker.ShowDialog();
                return dialogResult == DialogResult.OK ? picker.SelectedPath : Configuration.Aria2DirPath;
            }
        }

        private void Aria2Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            DispatcherHelper.RunAsync(() => Log(e.Data));
        }

        private void Aria2Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            DispatcherHelper.RunAsync(() => Log(e.Data));
        }

        private void Aria2Process_Exited(object sender, EventArgs e)
        {
            DispatcherHelper.RunAsync(() => 
            {
                _aria2Process.CancelOutputRead();
                _aria2Process.CancelErrorRead();

                IsRunning = false;
                Log("Aria2 已退出");
            });
        }
    }
}
