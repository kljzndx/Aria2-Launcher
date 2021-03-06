﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

using Aria2Launcher.Models;
using Aria2Launcher.Resources;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;

using ObservableObject = GalaSoft.MvvmLight.ObservableObject;

namespace Aria2Launcher.Services
{
    public class Aria2Service : ObservableObject
    {
        public static Aria2Service Current { get; }
        
        private bool _isRunning;
        private readonly Process _aria2Process;

        public event EventHandler<string> ErrorDataReceived;
        public event EventHandler Aria2Exited;

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
                if (MessageBox.Show(StringResource.Err_DownloadAria2, StringResource.Err_NoAria2Exe, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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
                if (MessageBox.Show(StringResource.Err_BuildConf, StringResource.Err_NoAria2Conf, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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

                Log(StringResource.Log_FindAria2Exe);
            }
            else
            {
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
                LogError(StringResource.Err_NoAria2Conf + ", " + StringResource.Log_Aria2RpcMode);
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
                Aria2Exited?.Invoke(this, EventArgs.Empty);
            });
        }
    }
}