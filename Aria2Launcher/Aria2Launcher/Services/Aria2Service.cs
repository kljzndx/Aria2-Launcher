using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aria2Launcher.Services
{
    public class Aria2Service : ObservableObject
    {
        public const string ExeName = "aria2c.exe";
        public const string ConfName = "aria2.conf";

        private IAppConfigService _appConfig;
        private Process? _aria2Process;

        private bool _isStarting;

        public event EventHandler<string>? Outputed;

        public Aria2Service(IAppConfigService appConfig)
        {
            _appConfig = appConfig;
        }

        public bool IsStarting
        {
            get { return _isStarting; }
            set { SetProperty(ref _isStarting, value); }
        }

        private string GetPath(string fileName)
        {
            return Path.Combine(_appConfig.ProgramDir, fileName);
        }

        private void Log(string? content)
        {
            Outputed?.Invoke(this, content ?? "");
        }

        private void LogWithTime(string? content)
        {
            Log($"{DateTime.Now.ToShortTimeString} {content ?? ""}");
        }

        public bool CheckExeExist()
        {
            return File.Exists(GetPath(ExeName));
        }

        public bool CheckConfExist()
        {
            return File.Exists(GetPath(ConfName));
        }

        public void StartAria2()
        {
            _aria2Process = new Process();
            _aria2Process.StartInfo.WorkingDirectory = _appConfig.ProgramDir;
            _aria2Process.StartInfo.FileName = GetPath(ExeName);
            _aria2Process.StartInfo.Arguments = "--conf-path ./" + ConfName;
            _aria2Process.EnableRaisingEvents = true;
            _aria2Process.StartInfo.UseShellExecute = false;

            _aria2Process.StartInfo.RedirectStandardOutput = true;
            _aria2Process.StartInfo.RedirectStandardError = true;
            _aria2Process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            _aria2Process.StartInfo.StandardErrorEncoding = Encoding.UTF8;

            _aria2Process.OutputDataReceived += Aria2Process_OutputDataReceived;
            _aria2Process.ErrorDataReceived += Aria2Process_ErrorDataReceived;

            _aria2Process.Exited += Aria2Process_Exited;

            _aria2Process.Start();
            _aria2Process.BeginOutputReadLine();
            _aria2Process.BeginErrorReadLine();

            IsStarting = true;
            LogWithTime("Aria2 已启动");
        }

        public void StopAria2()
        {
            if (_aria2Process == null)
                return;

            _aria2Process.Kill();
            _aria2Process.Dispose();
            _aria2Process = null;
            IsStarting = false;
        }

        private void Aria2Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Log(e.Data);
        }

        private void Aria2Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Log(e.Data);
        }

        private void Aria2Process_Exited(object? sender, EventArgs e)
        {
            LogWithTime("Aria2 已停止");
        }
    }
}
