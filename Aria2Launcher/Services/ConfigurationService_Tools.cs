using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aria2Launcher.Services
{
    public partial class ConfigurationService
    {

        #region 自动启动

        /// <summary>
        /// 为本程序创建一个快捷方式。
        /// </summary>
        /// <param name="lnkFilePath">快捷方式的完全限定路径。</param>
        /// <param name="args">快捷方式启动程序时需要使用的参数。</param>
        private void CreateShortcut(string lnkFilePath, string args = "")
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

        public void RemoveOldAutoStart()
        {
            var lnkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "Aria2Launcher.lnk");
            if (File.Exists(lnkPath))
                File.Delete(lnkPath);
        }

        public void SwitchAutoStart(bool needAutoStart)
        {
            var lnkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), LnkFileName);
            if (File.Exists(lnkPath))
                File.Delete(lnkPath);

            if (!needAutoStart)
                return;

            CreateShortcut(lnkPath, "--quiet");
        }

        #endregion
        #region Aria2

        public void BrowseAria2Folder()
        {
            using (var picker = new FolderBrowserDialog())
            {
                picker.RootFolder = Environment.SpecialFolder.MyComputer;
                var dialogResult = picker.ShowDialog();
                Aria2DirPath = dialogResult == DialogResult.OK ? picker.SelectedPath : Aria2DirPath;
            }
        }

        public void ViewAria2OfficialSite()
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = "https://github.com/aria2/aria2/releases/latest";
                process.StartInfo.UseShellExecute = true;
                process.Start();
            }
        }

        #endregion

    }
}
