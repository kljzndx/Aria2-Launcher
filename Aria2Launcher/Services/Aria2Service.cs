using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;

namespace Aria2Launcher.Services
{
    public class Aria2Service : ObservableObject
    {
        public static Aria2Service Current { get; }
        
        private bool _isRunning;
        private readonly Process _aria2Process;

        public event EventHandler<string> ErrorDataReceived;

        static Aria2Service()
        {
            Current = new Aria2Service();
        }

        private Aria2Service()
        {
            OutputList = new ObservableCollection<string>();
            _aria2Process = new Process();

            _aria2Process.StartInfo.CreateNoWindow = true;
            _aria2Process.EnableRaisingEvents = true;

            _aria2Process.StartInfo.RedirectStandardOutput = true;
            _aria2Process.StartInfo.RedirectStandardError = true;

            _aria2Process.OutputDataReceived += Aria2Process_OutputDataReceived;
            _aria2Process.ErrorDataReceived += Aria2Process_ErrorDataReceived;
            _aria2Process.Exited += Aria2Process_Exited;
        }
        
        public bool IsRunning
        {
            get => _isRunning;
            set => Set(ref _isRunning, value);
        }

        public ConfigurationService Configuration { get; } = ConfigurationService.Current;

        public ObservableCollection<string> OutputList { get; }

        private void Log(string content)
        {
            if (String.IsNullOrWhiteSpace(content))
                return;

            OutputList.Add(content);

            if (OutputList.Count > 50)
                OutputList.RemoveAt(0);
        }

        private void LogError(string content)
        {
            if (String.IsNullOrWhiteSpace(content))
                return;

            Log(content);
            
            ErrorDataReceived?.Invoke(this, content);
        }

        private bool SetupProcess()
        {
            string exePath = Configuration.Aria2DirPath + "\\aria2c.exe";
            string confPath = Configuration.Aria2DirPath + "\\aria2.conf";

            if (File.Exists(exePath))
                Log("已找到 aria2c.exe");
            else
            {
                LogError("未找到 aria2c.exe");
                return false;
            }
            
            if (File.Exists(confPath))
                Log("已找到 aria2.conf");
            else
            {
                LogError("未找到 aria2.conf");
                return false;
            }
            
            _aria2Process.StartInfo.FileName = exePath;
            _aria2Process.StartInfo.Arguments = $"--conf-path {confPath}";
            _aria2Process.StartInfo.WorkingDirectory = Configuration.Aria2DirPath;
            
            return true;
        }

        public void StartAria2()
        {
            OutputList.Clear();

            if (!SetupProcess())
                return;

            _aria2Process.Start();

            _aria2Process.BeginOutputReadLine();
            _aria2Process.BeginErrorReadLine();

            IsRunning = true;
        }

        public void StopAria2()
        {
            _aria2Process.Kill();
        }

        private void Aria2Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            DispatcherHelper.RunAsync(() => Log(e.Data));
        }

        private void Aria2Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            DispatcherHelper.RunAsync(() => LogError(e.Data));
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