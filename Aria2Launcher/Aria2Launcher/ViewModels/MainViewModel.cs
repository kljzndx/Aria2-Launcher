using Aria2Launcher.Services;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aria2Launcher.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel(IAppConfigService appConfig, Aria2Service aria2)
        {
            AppConfig = appConfig;
            Aria2 = aria2;

            StartAria2Command = new RelayCommand(() => ExecuteCommandCore(Aria2.StartAria2, StartAria2Command, StopAria2Command), () => !Aria2.IsStarting);
            StopAria2Command = new RelayCommand(() => ExecuteCommandCore(Aria2.StopAria2, StartAria2Command, StopAria2Command), () => Aria2.IsStarting);
        }

        public IAppConfigService AppConfig { get; }
        public Aria2Service Aria2 { get; }
        public ObservableCollection<string> Logs { get; } = new ObservableCollection<string>();

        public RelayCommand StartAria2Command { get; }
        public RelayCommand StopAria2Command { get; }

        private void ExecuteCommandCore(Action action, params RelayCommand?[] commands)
        {
            action();

            foreach (var item in commands)
                item?.NotifyCanExecuteChanged();
        }

        public void AddLog(string content)
        {
            if (Logs.Count > 50)
                Logs.RemoveAt(0);

            Logs.Add(content);
        }
    }
}
