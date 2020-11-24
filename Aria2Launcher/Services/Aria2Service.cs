using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
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

        public bool CheckExeExist()
        {
            string exePath = Configuration.Aria2DirPath + "\\aria2c.exe";

            if (!File.Exists(exePath))
            {
                if (MessageBox.Show("是否需要下载 Aria2", "找不到 aria2c.exe", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using (var process = new Process())
                    {
                        process.StartInfo.FileName = "https://github.com/aria2/aria2/releases/latest";
                        process.StartInfo.UseShellExecute = true;
                        process.Start();
                    }
                }
                return false;
            }

            return true;
        }

        public bool CheckConfExist()
        {
            string confPath = Configuration.Aria2DirPath + "\\aria2.conf";
            
            if (!File.Exists(confPath))
            {
                if (MessageBox.Show("是否需要创建 aria2.conf", "找不到 aria2.conf", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    File.WriteAllText(confPath, "enable-rpc=true");
                    return true;
                }
                return false;
            }

            return true;
        }

        private bool SetupProcess()
        {
            string exePath = Configuration.Aria2DirPath + "\\aria2c.exe";
            string confPath = Configuration.Aria2DirPath + "\\aria2.conf";

            if (CheckExeExist())
            {
                _aria2Process.StartInfo.FileName = exePath;
                _aria2Process.StartInfo.WorkingDirectory = Configuration.Aria2DirPath;

                Log("已找到 aria2c.exe");
            }
            else
            {
                LogError("未找到 aria2c.exe");
                return false;
            }

            if (CheckConfExist())
            {
                _aria2Process.StartInfo.Arguments = $"--conf-path {confPath}";
                Log("已找到 aria2.conf");
            }
            else
            {
                _aria2Process.StartInfo.Arguments = $"--enable-rpc";
                LogError("未找到 aria2.conf，正在手动启用 rpc 服务");
            }
            
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