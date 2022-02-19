using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Threading;

using Aria2Launcher.Resources;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Aria2Launcher.Services
{
    public class Aria2Service : ObservableObject
    {
        private const string ExeFileName = "aria2c.exe";
        private const string ConfFileName = "aria2.conf";

        public static Aria2Service Current { get; }
        
        private bool _isRunning;
        private bool _isRestarting;
        private readonly Process _aria2Process;

        public event EventHandler<string> ErrorDataReceived;
        public event EventHandler Aria2Exited;

        public event EventHandler CheckExeFalled;
        public event EventHandler CheckConfFalled;

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
            set => SetProperty(ref _isRunning, value);
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

        public string GetExePath()
        {
            return Path.Combine(Configuration.Aria2DirPath, ExeFileName);
        }

        public string GetConfPath()
        {
            return Path.Combine(Configuration.Aria2DirPath, ConfFileName);
        }

        public bool CheckExeExist()
        {
            string exePath = GetExePath();

            if (!File.Exists(exePath))
            {
                //if (MessageBox.Show(StringResource.Err_DownloadAria2, StringResource.Err_NoAria2Exe, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                //{
                //    using (var process = new Process())
                //    {
                //        process.StartInfo.FileName = "https://github.com/aria2/aria2/releases/latest";
                //        process.StartInfo.UseShellExecute = true;
                //        process.Start();
                //    }
                //}
                return false;
            }

            return true;
        }

        public bool CheckConfExist()
        {
            string confPath = GetConfPath();

            if (!File.Exists(confPath))
            {
                //if (MessageBox.Show(StringResource.Err_BuildConf, StringResource.Err_NoAria2Conf, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                //{
                //    File.WriteAllText(confPath, "enable-rpc=true");
                //    return true;
                //}
                return false;
            }

            return true;
        }

        private bool SetupProcess()
        {
            string exePath = Path.Combine(Configuration.Aria2DirPath, ExeFileName);
            string confPath = Path.Combine(Configuration.Aria2DirPath, ConfFileName);

            if (CheckExeExist())
            {
                _aria2Process.StartInfo.FileName = exePath;
                _aria2Process.StartInfo.WorkingDirectory = Configuration.Aria2DirPath;

                Log(StringResource.Log_FindAria2Exe);
            }
            else
            {
                CheckExeFalled?.Invoke(this, EventArgs.Empty);
                LogError(StringResource.Err_NoAria2Exe);
                return false;
            }

            if (CheckConfExist())
            {
                _aria2Process.StartInfo.Arguments = $"--conf-path {confPath}";
                Log(StringResource.Log_FindAria2Conf);
            }
            else
            {
                _aria2Process.StartInfo.Arguments = $"--enable-rpc";
                CheckConfFalled?.Invoke(this, EventArgs.Empty);
                LogError(StringResource.Err_NoAria2Conf + ", " + StringResource.Log_Aria2RpcMode);
            }

            _aria2Process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            _aria2Process.StartInfo.StandardErrorEncoding = Encoding.UTF8;
            
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

        public void RestartAria2()
        {
            _isRestarting = true;
            if (IsRunning)
                StopAria2();

            StartAria2();
            _isRestarting = false;
        }

        private void Aria2Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(() => Log(e.Data));
        }

        private void Aria2Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(() => LogError(e.Data));
        }

        private void Aria2Process_Exited(object sender, EventArgs e)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(() => 
            {
                _aria2Process.CancelOutputRead();
                _aria2Process.CancelErrorRead();

                IsRunning = false;
                Log("Aria2 已退出");
                if (!_isRestarting)
                    Aria2Exited?.Invoke(this, EventArgs.Empty);
            });
        }
    }
}